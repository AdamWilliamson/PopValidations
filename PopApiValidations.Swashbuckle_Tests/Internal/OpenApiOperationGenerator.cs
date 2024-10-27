using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;

public static class OpenApiOperationBuilder
{
    public static OpenApiOperation CreateFromAction(MethodInfo methodInfo, SchemaRepository schemaRepository, bool useReferences)
    {
        var operation = new OpenApiOperation
        {
            Summary = methodInfo.Name,
            OperationId = methodInfo.Name,
            Tags = new List<OpenApiTag>
            {
                new OpenApiTag { Name = methodInfo.DeclaringType?.Name }
            },
            Parameters = new List<OpenApiParameter>(),
            RequestBody = new OpenApiRequestBody()
        };

        foreach (var parameter in methodInfo.GetParameters())
        {
            bool isComplexType = IsComplexType(parameter.ParameterType);

            // Check for FromBody or FromForm attributes
            if (parameter.GetCustomAttribute<FromBodyAttribute>() != null ||
                parameter.GetCustomAttribute<FromFormAttribute>() != null ||
                (isComplexType && !HasSpecialAttributes(parameter))) // Treat complex types as FromBody by default
            {
                // Handle as request body
                var requestBodySchema = CreateSchemaForParameter(parameter, schemaRepository, useReferences);
                operation.RequestBody.Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema = requestBodySchema
                    }
                };
            }
            else
            {
                // If the parameter has FromQuery or FromHeader, handle its properties as parameters
                if (isComplexType)
                {
                    ExpandComplexTypeProperties(parameter, operation, GetParameterName(parameter), schemaRepository, useReferences);
                }
                else
                {
                    var paramName = GetParameterName(parameter);
                    var paramLocation = GetParameterLocation(parameter);

                    var schema = CreateSchemaForParameter(parameter, schemaRepository, useReferences);

                    var param = new OpenApiParameter
                    {
                        Name = paramName,
                        In = paramLocation,
                        Required = !parameter.IsOptional,
                        Schema = schema
                    };

                    operation.Parameters.Add(param);
                }
            }
        }

        operation.Responses = new OpenApiResponses
        {
            ["200"] = new OpenApiResponse { Description = "Successful response" }
        };

        // Add more response codes based on the action
        var httpAttributes = methodInfo.GetCustomAttributes<HttpMethodAttribute>();
        foreach (var attribute in httpAttributes)
        {
            operation.Tags.Add(new OpenApiTag { Name = attribute.HttpMethods.FirstOrDefault() });
        }

        return operation;
    }

    private static void ExpandComplexTypeProperties(ParameterInfo parameter, OpenApiOperation operation, string prefix, SchemaRepository schemaRepository, bool useReferences)
    {
        var location = GetParameterLocation(parameter);
        var properties = parameter.ParameterType.GetProperties();
        foreach (var prop in properties ?? [])
        {
            //var propPrefix = string.IsNullOrEmpty(prefix) ? prop.Name : $"{prefix}.{prop.Name}";

            if (IsComplexType(prop.PropertyType))
            {
                // Recursively expand properties of complex types
                ExpandComplexTypeProperties(prop, operation, GetParameterName(prop), location, schemaRepository, useReferences);
            }
            else
            {
                var schema = CreateSchemaForParameter(parameter, schemaRepository, useReferences);
                var param = new OpenApiParameter
                {
                    Name = GetParameterName(prop, null),
                    In = location,//GetParameterLocation(prop),
                    Required = !prop.PropertyType.IsGenericType || !prop.PropertyType.IsValueType,
                    Schema = schema
                };

                operation.Parameters.Add(param);
            } 
        }
    }

    private static void ExpandComplexTypeProperties(PropertyInfo property, OpenApiOperation operation, string prefix, ParameterLocation? location, SchemaRepository schemaRepository, bool useReferences)
    {
        var properties = property.PropertyType.GetProperties();
        foreach (var prop in properties)
        {
            var propPrefix = GetParameterName(prop, prefix);// string.IsNullOrEmpty(prefix) ? prop.Name : $"{prefix}.{prop.Name}";
            
            if (IsComplexType(prop.PropertyType))
            {
                // Recursively expand properties of complex types
                ExpandComplexTypeProperties(prop, operation, propPrefix, location, schemaRepository, useReferences);
            }
            else
            {
                var schema = CreateSchemaForProperty(property, schemaRepository, useReferences);

                var param = new OpenApiParameter
                {
                    Name = propPrefix,//GetParameterName(prop, propPrefix),
                    In = location,
                    Required = !prop.PropertyType.IsGenericType || !prop.PropertyType.IsValueType,
                    Schema = schema
                };

                operation.Parameters.Add(param);
            }
        }
    }

    private static bool HasSpecialAttributes(ParameterInfo parameter)
    {
        return parameter.GetCustomAttribute<FromQueryAttribute>() != null ||
               parameter.GetCustomAttribute<FromHeaderAttribute>() != null ||
               parameter.GetCustomAttribute<FromRouteAttribute>() != null
               ;
    }

    private static string? GetParameterName(ParameterInfo parameter, string? prefix = null)
    {
        var routeAttr= parameter.GetCustomAttribute<FromRouteAttribute>();
        var headerAttr = parameter.GetCustomAttribute<FromHeaderAttribute>();
        var queryAttr = parameter.GetCustomAttribute<FromQueryAttribute>();
        var paramName = routeAttr?.Name ?? headerAttr?.Name ?? queryAttr?.Name ?? parameter.Name;

        return string.IsNullOrEmpty(prefix) ? paramName : $"{prefix}.{paramName}";
    }

    private static string? GetParameterName(PropertyInfo property, string? prefix = null)
    {
        var jsonProperty = property.GetCustomAttribute<JsonPropertyAttribute>();
        var paramName = jsonProperty != null ? jsonProperty.PropertyName : property.Name;

        return string.IsNullOrEmpty(prefix) ? paramName : $"{prefix}.{paramName}";
    }

    private static ParameterLocation GetParameterLocation(ParameterInfo parameter)
    {
        // Check for FromQuery attribute as the default for non-body parameters
        var fromQuery = parameter.GetCustomAttribute<FromQueryAttribute>();
        if (fromQuery != null)
        {
            return ParameterLocation.Query;
        }

        // Check for FromHeader attribute
        var fromHeader = parameter.GetCustomAttribute<FromHeaderAttribute>();
        if (fromHeader != null)
        {
            return ParameterLocation.Header;
        }

        // Check for FromRoute attribute
        var fromRoute = parameter.GetCustomAttribute<FromRouteAttribute>();
        if (fromRoute != null)
        {
            return ParameterLocation.Path;
        }

        // Default to path for non-optional parameters or query for optional ones
        return parameter.IsOptional ? ParameterLocation.Query : ParameterLocation.Path;
    }

    private static OpenApiSchema CreateSchemaForParameter(ParameterInfo parameter, SchemaRepository schemaRepository, bool useReferences)
    {
        if (useReferences && schemaRepository.Schemas.ContainsKey(parameter.ParameterType.FullName))
        {
            return new OpenApiSchema { Reference = new OpenApiReference { Type = ReferenceType.Schema, Id = parameter.ParameterType.FullName } };
        }

        if (IsComplexType(parameter.ParameterType))
        {
            var schema = new OpenApiSchema
            {
                Type = "object",
                Properties = new Dictionary<string, OpenApiSchema>()
            };

            foreach (var prop in parameter.ParameterType.GetProperties())
            {
                var propSchema = CreateSchemaForProperty(prop, schemaRepository, useReferences);
                schema.Properties[GetParameterName(prop)] = propSchema;
            }

            if (useReferences)
            {
                schemaRepository.Schemas[parameter.ParameterType.FullName] = schema;
            }
            else
            {
                return schema;
            }
        }
        else
        {
            var schema = new OpenApiSchema
            {
                Type = GetOpenApiType(parameter.ParameterType),// "object",
                Properties = new Dictionary<string, OpenApiSchema>()
            };

            return schema;
        }


        return new OpenApiSchema { Reference = new OpenApiReference { Type = ReferenceType.Schema, Id = parameter.ParameterType.FullName } };
        //return schema;
    }

    private static OpenApiSchema CreateSchemaForProperty(PropertyInfo prop, SchemaRepository schemaRepository, bool useReferences)
    {
        if (useReferences && schemaRepository.Schemas.ContainsKey(prop.PropertyType.FullName))
        {
            return new OpenApiSchema { Reference = new OpenApiReference { Type = ReferenceType.Schema, Id = prop.PropertyType.FullName } };
        }

        // Optionally, add nested schema handling for complex types
        if (IsComplexType(prop.PropertyType))
        {
            var schema = new OpenApiSchema
            {
                Type = "object"
            };

            schema.Properties = new Dictionary<string, OpenApiSchema>();
            foreach (var subProp in prop.PropertyType.GetProperties())
            {
                var subPropName = GetParameterName(subProp);
                schema.Properties[subPropName] = CreateSchemaForProperty(subProp, schemaRepository, useReferences);
            }

            if (useReferences)
            {
                schemaRepository.Schemas[prop.PropertyType.FullName] = schema;
            }
            else
            {
                return schema;
            }
            
        }
        else
        {
            var schema = new OpenApiSchema
            {
                Type = GetOpenApiType(prop.PropertyType),
                Properties = new Dictionary<string, OpenApiSchema>()
            };

            return schema;
        }

        return new OpenApiSchema { Reference = new OpenApiReference { Type = ReferenceType.Schema, Id = prop.PropertyType.FullName } };
    }

    private static bool IsComplexType(Type type)
    {
        return !type.IsPrimitive && type != typeof(string) && !type.IsValueType;
    }

    private static string GetOpenApiType(Type type)
    {
        return type == typeof(int) ? "integer" :
               type == typeof(string) ? "string" :
               type == typeof(bool) ? "boolean" :
               "string"; // Default to string for simplicity
    }
}
