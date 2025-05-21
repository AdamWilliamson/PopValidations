using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections;
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
            OperationId = GetUrlFromMethodInfo(methodInfo),
            Tags = new List<OpenApiTag>
            {
                new OpenApiTag { Name = methodInfo.DeclaringType?.Name }
            },
            Parameters = new List<OpenApiParameter>(),
            RequestBody = null// = new OpenApiRequestBody()
        };

        foreach (var parameter in methodInfo.GetParameters())
        {
            bool isComplexType = IsComplexType(parameter.ParameterType);

            // Check for FromBody or FromForm attributes
            if (parameter.GetCustomAttribute<FromBodyAttribute>() != null ||
                parameter.GetCustomAttribute<FromFormAttribute>() != null ||
                (isComplexType && !IsParameter(parameter))) // Treat complex types as FromBody by default
            {
                // Handle as request body
                var requestBodySchema = CreateSchemaForParameter(parameter, schemaRepository, useReferences);
                operation.RequestBody = new OpenApiRequestBody();
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
                if (IsGenericList(parameter.ParameterType))
                {
                    var paramName = GetParameterName(parameter);
                    var paramLocation = GetParameterLocation(parameter);
                    var param = new OpenApiParameter
                    {
                        Name = paramName,
                        In = paramLocation,
                        Required = !parameter.IsOptional,
                        Schema = new OpenApiSchema()
                        {
                            Type = "array",
                            Items = CreateSchemaForType(GetListGenericType(parameter.ParameterType), schemaRepository, useReferences)
                        }
                    };

                    operation.Parameters.Add(param);
                }
                else if (IsDictionaryType(parameter.ParameterType))
                {
                    var paramName = GetParameterName(parameter);
                    var paramLocation = GetParameterLocation(parameter);
                    var param = new OpenApiParameter
                    {
                        Name = paramName,
                        In = paramLocation,
                        Required = !parameter.IsOptional,
                        Schema = new OpenApiSchema()
                        {
                            Type = "object"
                        }
                    };

                    operation.Parameters.Add(param);
                }
                else if (!IsGenericList(parameter.ParameterType) && isComplexType)
                {
                    ExpandComplexTypeProperties(parameter, operation, null, schemaRepository, useReferences);
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

    // Method to convert a MethodInfo to a URL
    public static string GetUrlFromMethodInfo(MethodInfo methodInfo)
    {
        // First, try to get the RouteAttribute or HttpMethodAttributes (GET, POST, etc.)
        var httpMethodAttributes = methodInfo.GetCustomAttributes(true)
            .OfType<HttpMethodAttribute>()
            .ToList();

        // If no HTTP method attribute found, we can't generate the URL.
        if (httpMethodAttributes.Count == 0)
        {
            throw new InvalidOperationException("No HTTP method attribute (GET, POST, etc.) found on the method.");
        }

        // Get the route template from the RouteAttribute on the method or controller
        var routeTemplate = GetRouteTemplate(methodInfo, httpMethodAttributes);

        // Substitute route parameters with their method parameter names
        var methodParameters = methodInfo.GetParameters();
        foreach (var parameter in methodParameters)
        {
            // Substitute any route parameters like {id} with the parameter's name
            routeTemplate = routeTemplate.Replace($"{{{parameter.Name}}}", $"{{{parameter.Name}}}");
        }

        // Return the final route without a base URL
        return routeTemplate;
    }

    // Helper method to get the route template from the method's Route or HTTP method attributes
    private static string GetRouteTemplate(MethodInfo methodInfo, List<HttpMethodAttribute> httpMethodAttributes)
    {
        // Look for the RouteAttribute on the method
        var routeAttribute = methodInfo.GetCustomAttributes(true)
            .OfType<RouteAttribute>()
            .FirstOrDefault();

        // If found, return the template from RouteAttribute
        if (routeAttribute != null)
        {
            return routeAttribute.Template;
        }

        // Look for the RouteAttribute on the controller class
        var controllerType = methodInfo.DeclaringType;
        var controllerRouteAttribute = controllerType?.GetCustomAttributes(true)
            .OfType<RouteAttribute>()
            .FirstOrDefault();

        // If the controller has a RouteAttribute, return the combined route template
        if (controllerRouteAttribute != null)
        {
            var controllerRoute = controllerRouteAttribute.Template;

            // Replace the [controller] token with the controller's name (minus "Controller" suffix)
            var controllerName = controllerType.Name.Replace("Controller", "").ToLower();
            controllerRoute = controllerRoute.Replace("[controller]", controllerName);

            // If no specific template in the HttpMethodAttributes, return only the controller route
            var httpMethodTemplate = httpMethodAttributes.FirstOrDefault()?.Template;

            if (string.IsNullOrEmpty(httpMethodTemplate))
            {
                return controllerRoute;  // Just use the controller's route
            }

            // Otherwise, return the combined route (controller route + HTTP method route)
            return $"{controllerRoute}/{httpMethodTemplate}";
        }

        // If no RouteAttribute found, check for common HTTP method attributes (GET, POST, etc.)
        var httpMethodAttribute = httpMethodAttributes.FirstOrDefault();
        if (httpMethodAttribute != null)
        {
            // Use the HTTP method attributes' template if available
            return httpMethodAttribute.Template ?? $"/{methodInfo.Name}";
        }

        // Default route if nothing is found
        return $"/{methodInfo.Name}";
    }

    private static void ExpandComplexTypeProperties(ParameterInfo parameter, OpenApiOperation operation, string prefix, SchemaRepository schemaRepository, bool useReferences)
    {
        var location = GetParameterLocation(parameter);
        var properties = GetProperties(parameter.ParameterType);//.GetProperties();

        foreach (var prop in properties ?? [])
        {
            //var propPrefix = string.IsNullOrEmpty(prefix) ? prop.Name : $"{prefix}.{prop.Name}";

            if (IsGenericList(prop.PropertyType))
            {
                var paramName = GetParameterName(prop);
                var paramLocation = GetParameterLocation(parameter);
                var param = new OpenApiParameter
                {
                    Name = paramName,
                    In = paramLocation,
                    Required = !parameter.IsOptional,
                    Schema = new OpenApiSchema()
                    {
                        Type = "array",
                        Items = CreateSchemaForType(GetListGenericType(prop.PropertyType), schemaRepository, useReferences)
                    }
                };

                operation.Parameters.Add(param);
            }
            else if (IsDictionaryType(prop.PropertyType))
            {
                var paramName = GetParameterName(prop);
                var paramLocation = GetParameterLocation(parameter);
                var param = new OpenApiParameter
                {
                    Name = paramName,
                    In = paramLocation,
                    Required = !parameter.IsOptional,
                    Schema = new OpenApiSchema()
                    {
                        Type = "object"
                    }
                };

                operation.Parameters.Add(param);
            }
            else if (IsComplexType(prop.PropertyType))
            {
                // Recursively expand properties of complex types
                ExpandComplexTypeProperties(prop, operation, GetParameterName(prop), location, schemaRepository, useReferences);
                //operation.Parameters.Add(new OpenApiParameter()
                //{
                //    Name = GetParameterName(prop),
                //    In = GetParameterLocation(parameter),
                //    Schema = CreateSchemaForProperty(prop, schemaRepository, useReferences)
                //});
            }
            else
            {
                var schema = CreateSchemaForProperty(prop, schemaRepository, useReferences);
                //var schema = CreateSchemaForParameter(prop, schemaRepository, useReferences);
                var param = new OpenApiParameter
                {
                    Name = GetParameterName(prop, prefix),
                    In = location,//GetParameterLocation(prop),
                    Required = !prop.PropertyType.IsGenericType || !prop.PropertyType.IsValueType,
                    Schema = schema
                };

                operation.Parameters.Add(param);
            } 
        }
    }

    public static string GetTypeName(Type type)
    {
        if (IsGenericList(type))
        {
            return type.GetGenericArguments()[0].Name;
        }

        return type.Name;
    }

    public static Type GetListGenericType(Type type)
    {
        if (IsGenericList(type))
        {
            return type.GetGenericArguments()[0];
        }

        return type;
    }

    public static IEnumerable<PropertyInfo> GetProperties(Type type)
    {
        if (type == null)
            throw new ArgumentNullException(nameof(type));


        if (IsGenericList(type))
        {
            // Get the generic type argument of the list
            var genericArgument = type.GetGenericArguments()[0];

            // Get properties of the generic type
            if (!IsComplexType(genericArgument))
            {
                yield break;
            }

            var genericProperties = genericArgument.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var genericProperty in genericProperties)
            {
                // You can modify the output as needed, here I'm just yielding the properties
                yield return genericProperty;
            }
        }
        else if (!IsComplexType(type))
        {
            yield break;
        }
        else if (IsDictionaryType(type))
        {
            yield break;
        }
        else
        {
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var property in properties)
            {
                    yield return property;
            }
        }
    }

    private static bool IsDictionaryType(Type type)
    {
        // Check if the type is a generic type and if it's a Dictionary<TKey, TValue>
        if (type.IsGenericType)
        {
            var genericDefinition = type.GetGenericTypeDefinition();
            return genericDefinition == typeof(Dictionary<,>) ||
                   genericDefinition == typeof(IDictionary<,>);
        }

        // Check if the type is a non-generic IDictionary
        return typeof(IDictionary).IsAssignableFrom(type);
    }

    private static bool IsGenericList(Type type)
    {
        return PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.Helpers.TypeHelper.IsArrayType(type);
        // return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
    }

    private static void ExpandComplexTypeProperties(PropertyInfo property, OpenApiOperation operation, string prefix, ParameterLocation? location, SchemaRepository schemaRepository, bool useReferences)
    {
        //var properties = property.PropertyType.GetProperties(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public);
        var properties = GetProperties(property.PropertyType);
        foreach (var prop in properties)
        {
            var propPrefix = GetParameterName(prop, prefix);// string.IsNullOrEmpty(prefix) ? prop.Name : $"{prefix}.{prop.Name}";

            if (IsDictionaryType(prop.PropertyType))
            {
                var schema = new OpenApiSchema
                {
                    Type = "object"
                };

                var param = new OpenApiParameter
                {
                    Name = propPrefix,//GetParameterName(prop, propPrefix),
                    In = location,
                    Schema = schema
                };

                operation.Parameters.Add(param);
            }
            else if (!IsGenericList(prop.PropertyType) && IsComplexType(prop.PropertyType))
            {
                // Recursively expand properties of complex types
                ExpandComplexTypeProperties(prop, operation, propPrefix, location, schemaRepository, useReferences);
            }
            else if (IsGenericList(prop.PropertyType))
            {
                // Recursively expand properties of complex types
                //ExpandComplexTypeProperties(prop, operation, propPrefix, location, schemaRepository, useReferences);

                var schema = new OpenApiSchema
                {
                    Type = "array",
                    Items = CreateSchemaForType(GetListGenericType(prop.PropertyType), schemaRepository, useReferences)
                };

                var param = new OpenApiParameter
                {
                    Name = propPrefix,//GetParameterName(prop, propPrefix),
                    In = location,
                    Schema = schema
                };

                operation.Parameters.Add(param);
            }
            else
            {
                var schema = CreateSchemaForProperty(prop, schemaRepository, useReferences);

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

    private static bool IsParameter(ParameterInfo parameter)
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
        var paramName = jsonProperty?.PropertyName ?? property.Name;

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
        if (useReferences && schemaRepository.Schemas.ContainsKey(parameter.ParameterType.Name))
        {
            return new OpenApiSchema { Reference = new OpenApiReference { Type = ReferenceType.Schema, Id = GetTypeName(parameter.ParameterType) } };
        }

        if (IsGenericList(parameter.ParameterType))
        {
            var listType = GetListGenericType(parameter.ParameterType);
            var schema = new OpenApiSchema
            {
                Type = "array",// "object",
                Properties = new Dictionary<string, OpenApiSchema>(),
                Items = CreateSchemaForType(listType, schemaRepository, useReferences)
            };
            return schema;
        }
        else if (IsComplexType(parameter.ParameterType))
        {
            var schema = new OpenApiSchema
            {
                Type = "object",
                Properties = new Dictionary<string, OpenApiSchema>()
            };

            foreach (var prop in GetProperties(parameter.ParameterType))//.GetProperties(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
            {
                var propSchema = CreateSchemaForProperty(prop, schemaRepository, useReferences);
                schema.Properties[GetParameterName(prop)] = propSchema;
            }

            if (useReferences)
            {
                schemaRepository.Schemas[GetTypeName(parameter.ParameterType)] = schema;
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


        return new OpenApiSchema { Reference = new OpenApiReference { Type = ReferenceType.Schema, Id = GetTypeName(parameter.ParameterType) } };
        //return schema;
    }

    private static OpenApiSchema CreateSchemaForProperty(PropertyInfo prop, SchemaRepository schemaRepository, bool useReferences)
    {
        if (useReferences && schemaRepository.Schemas.ContainsKey(GetTypeName(prop.PropertyType)))
        {
            return new OpenApiSchema { Reference = new OpenApiReference { Type = ReferenceType.Schema, Id = GetTypeName(prop.PropertyType) } };
        }

        // Optionally, add nested schema handling for complex types
        if (IsGenericList(prop.PropertyType))
        {
            var listSchema = new OpenApiSchema();

            var schema = new OpenApiSchema
            {
                Type = "array",
                Items = listSchema
            };

            listSchema.Properties = new Dictionary<string, OpenApiSchema>();
            foreach (var subProp in GetProperties(prop.PropertyType))
            {
                var subPropName = GetParameterName(subProp);
                listSchema.Properties[subPropName] = CreateSchemaForProperty(subProp, schemaRepository, useReferences);
            }

            if (useReferences)
            {
                schemaRepository.Schemas[GetTypeName(prop.PropertyType)] = listSchema;
                return new OpenApiSchema { Reference = new OpenApiReference { Type = ReferenceType.Schema, Id = GetTypeName(prop.PropertyType) } };
            }
            return schema;
            //==
            //var listType = GetListGenericType(prop.PropertyType);
            //var schema = new OpenApiSchema
            //{
            //    Type = "array",// "object",
            //    Properties = new Dictionary<string, OpenApiSchema>(),
            //    Items = new OpenApiSchema
            //    {
            //        Type = GetOpenApiType(listType),// "object",
            //        Properties = new Dictionary<string, OpenApiSchema>(),
            //    }
            //};
            //return schema;
        }
        else if (IsComplexType(prop.PropertyType))
        {
            var schema = new OpenApiSchema
            {
                Type = "object"
            };

            schema.Properties = new Dictionary<string, OpenApiSchema>();
            foreach (var subProp in GetProperties(prop.PropertyType))
            {
                var subPropName = GetParameterName(subProp);
                schema.Properties[subPropName] = CreateSchemaForProperty(subProp, schemaRepository, useReferences);
            }

            if (useReferences)
            {
                schemaRepository.Schemas[GetTypeName(prop.PropertyType)] = schema;
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

        return new OpenApiSchema { Reference = new OpenApiReference { Type = ReferenceType.Schema, Id = GetTypeName(prop.PropertyType) } };
    }

    private static OpenApiSchema CreateSchemaForType(Type type, SchemaRepository schemaRepository, bool useReferences)
    {
        if (useReferences && schemaRepository.Schemas.ContainsKey(GetTypeName(type)))
        {
            return new OpenApiSchema { Reference = new OpenApiReference { Type = ReferenceType.Schema, Id = GetTypeName(type) } };
        }

        // Optionally, add nested schema handling for complex types
        if (IsGenericList(type))
        {
            var listSchema = new OpenApiSchema();

            var schema = new OpenApiSchema
            {
                Type = "array",
                Items = listSchema
            };

            listSchema.Properties = new Dictionary<string, OpenApiSchema>();
            foreach (var subProp in GetProperties(type))
            {
                var subPropName = GetParameterName(subProp);
                listSchema.Properties[subPropName] = CreateSchemaForProperty(subProp, schemaRepository, useReferences);
            }

            if (useReferences)
            {
                schemaRepository.Schemas[GetTypeName(type)] = listSchema;
                return new OpenApiSchema { Reference = new OpenApiReference { Type = ReferenceType.Schema, Id = GetTypeName(type) } };
            }

            return schema;
        }
        else if (IsComplexType(type))
        {
            var schema = new OpenApiSchema
            {
                Type = "object"
            };

            schema.Properties = new Dictionary<string, OpenApiSchema>();
            foreach (var subProp in GetProperties(type))
            {
                var subPropName = GetParameterName(subProp);
                schema.Properties[subPropName] = CreateSchemaForProperty(subProp, schemaRepository, useReferences);
            }

            if (useReferences)
            {
                schemaRepository.Schemas[GetTypeName(type)] = schema;
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
                Type = GetOpenApiType(type),
                Properties = new Dictionary<string, OpenApiSchema>()
            };

            return schema;
        }

        return new OpenApiSchema { Reference = new OpenApiReference { Type = ReferenceType.Schema, Id = GetTypeName(type) } };
    }

    private static bool IsComplexType(Type type)
    {
        return !type.IsPrimitive && type != typeof(string) && !type.IsValueType && !IsGenericList(type) && type != typeof(object);
    }

    private static string GetOpenApiType(Type type)
    {
        // Check for Nullable<T> and get the underlying type if it is nullable
        Type underlyingType = Nullable.GetUnderlyingType(type) ?? type;

        return IsNumericType(underlyingType) ? "number" :
               underlyingType == typeof(string) ? "string" :
               underlyingType == typeof(bool) ? "boolean" :
               underlyingType == typeof(DateTime) ? "string" :  // Assuming DateTime is represented as string in OpenAPI
               underlyingType == typeof(Guid) ? "string" :  // Guid is typically represented as string
               "object"; // Default to object for unsupported types
    }

    private static bool IsNumericType(Type type)
    {
        // Handle Nullable types by getting the underlying type
        Type underlyingType = Nullable.GetUnderlyingType(type) ?? type;

        // Check if the type is a numeric type
        return underlyingType.IsPrimitive && (
               underlyingType == typeof(byte) ||
               underlyingType == typeof(sbyte) ||
               underlyingType == typeof(short) ||
               underlyingType == typeof(ushort) ||
               underlyingType == typeof(int) ||
               underlyingType == typeof(uint) ||
               underlyingType == typeof(long) ||
               underlyingType == typeof(ulong) ||
               underlyingType == typeof(float) ||
               underlyingType == typeof(double) ||
               underlyingType == typeof(decimal));
    }
}
