using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Interfaces;

namespace PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3;

public class OpenApiOperationMapping
{
    public string Path { get; set; }
    public string HttpMethod { get; set; } // GET, POST, PUT, DELETE, etc.
    public MethodInfo MethodInfo { get; set; } // The MethodInfo for the operation
    public List<OpenApiParameterMapping> Parameters { get; set; } = new List<OpenApiParameterMapping>();
    public OpenApiRequestBodyMapping RequestBody { get; set; } // Request body, if applicable
    public List<OpenApiResponseMapping> Responses { get; set; } = new List<OpenApiResponseMapping>(); // Responses for the operation
    public IDictionary<string, IOpenApiExtension> Extensions { get; set; }
}

public class OpenApiParameterMapping
{
    public string Name { get; set; }
    public ParameterLocation In { get; set; }
    public OpenApiParameter Parameter { get; set; }
    public OpenApiSchema Schema { get; set; }

    public bool IsArray { get; set; } // Indicates if the parameter is an array
    public List<OpenApiPropertyMapping> PropertyMappings { get; set; } = new List<OpenApiPropertyMapping>();
    public IDictionary<string, IOpenApiExtension> ParentPropertyExtensions { get; set; }
}

public class OpenApiResponseMapping
{
    public string StatusCode { get; set; }
    public string Content { get; set; }
    public OpenApiSchema? Schema { get; set; }
    public IDictionary<string, IOpenApiExtension> ParentPropertyExtensions { get; set; }
    public List<OpenApiPropertyMapping> PropertyMappings { get; set; } = new ();
}

public class OpenApiRequestBodyMapping
{
    public OpenApiRequestBody RequestBody { get; set; }
    public Dictionary<string, OpenApiSchema> ContentSchemas { get; set; } = new Dictionary<string, OpenApiSchema>();

    public List<OpenApiPropertyMapping> PropertyMappings { get; set; } = new List<OpenApiPropertyMapping>();
    public IDictionary<string, IOpenApiExtension> ParentPropertyExtensions { get; set; }
}

public class OpenApiPropertyMapping
{
    public string PropertyName { get; set; }
    public OpenApiSchema PropertySchema { get; set; }
    public bool IsArray { get; set; } // Indicates if the property is an array
    public List<OpenApiPropertyMapping> NestedProperties { get; set; } = new List<OpenApiPropertyMapping>(); // Nested properties (if any)
    public IDictionary<string, IOpenApiExtension> ParentPropertyExtensions { get; set; }

    public OpenApiSchema[] SecondarySchemas { get; set; }
}


public class OpenApiToMapping
{
    public OpenApiOperationMapping MapOpenApiOperation(OpenApiOperation operation, SchemaRepository schemaRepository, MethodInfo methodInfo)
    {
        var operationMapping = new OpenApiOperationMapping
        {
            Path = operation.OperationId,
            HttpMethod = GetOpenApiOperation(methodInfo),
            MethodInfo = methodInfo,
            Extensions = operation.Extensions,
        };

        // Map the parameters
        foreach (var parameter in operation.Parameters)
        {
            var parameterMapping = new OpenApiParameterMapping
            {
                Name = parameter.Name,
                In = Enum.TryParse<ParameterLocation>(parameter.In.ToString(), true, out var location) ? location : ParameterLocation.Query,
                Schema = ResolveSchema(parameter.Schema, schemaRepository),
                Parameter = parameter,
                IsArray = parameter.Schema?.Type == "array", // Check if the parameter is an array
                ParentPropertyExtensions = operation.Extensions
            };

            // Recursively map properties for complex types
            if (parameterMapping.Schema != null && !IsSimpleType(parameterMapping.Schema))
            {
                parameterMapping.PropertyMappings.AddRange(
                    MapProperties(
                        parameter.Extensions,
                        parameterMapping.Schema, 
                        schemaRepository,
                        []
                    )
                );
            }

            operationMapping.Parameters.Add(parameterMapping);
        }

        // Map the request body (if present)
        if (operation.RequestBody != null)
        {
            var requestBodyMapping = new OpenApiRequestBodyMapping
            {
                RequestBody = operation.RequestBody,
                ParentPropertyExtensions = operation.Extensions
            };

            // Add schemas for different content types (e.g., application/json, text/plain)
            foreach (var item in operation.RequestBody.Content)
            {
                var itemschema = ResolveSchema(item.Value.Schema, schemaRepository);
                if (itemschema is not null)
                {
                    requestBodyMapping.ContentSchemas[item.Key] = itemschema;
                }
            }

            var chosenRequestBody = operation.RequestBody.Content.FirstOrDefault();
            var secondaryBodies = operation.RequestBody.Content.Skip(1).ToList();

            var schema = ResolveSchema(chosenRequestBody.Value.Schema, schemaRepository);
            if (schema != null && !IsSimpleType(schema))
            {
                requestBodyMapping.PropertyMappings.AddRange(
                    MapProperties(
                        operation.Extensions,
                        schema,
                        schemaRepository,
                        secondaryBodies
                            .Select(x => ResolveSchema(x.Value.Schema, schemaRepository))
                            .Where(x => x is not null)
                            .ToArray()
                    )
                );
            }

            //foreach (var content in operation.RequestBody.Content)
            //{
            //    var schema = ResolveSchema(content.Value.Schema, schemaRepository);
            //    requestBodyMapping.ContentSchemas[content.Key] = schema;

            //    // Recursively map properties for the schema
            //    if (schema != null && !IsSimpleType(schema))
            //    {
            //        requestBodyMapping.PropertyMappings.AddRange(
            //            MapProperties(
            //                operation.Extensions,
            //                schema, 
            //                schemaRepository,
            //                []
            //            )
            //        );
            //    }
            //}

            operationMapping.RequestBody = requestBodyMapping;
        }

        // Map the responses
        foreach (var response in operation.Responses)
        {
            foreach (var content in response.Value.Content)
            {
                var responseMapping = new OpenApiResponseMapping
                {
                    StatusCode = response.Key,
                    Content = content.Key,
                    Schema = ResolveSchema(content.Value.Schema, schemaRepository),
                    ParentPropertyExtensions = operation.Extensions
                };

                if (responseMapping.Schema != null && !IsSimpleType(responseMapping.Schema))
                {
                    responseMapping.PropertyMappings.AddRange(
                        MapProperties(
                            operation.Extensions,
                            responseMapping.Schema,
                            schemaRepository,
                            []
                        )
                    );
                }
                operationMapping.Responses.Add(responseMapping);
            }
        }

        return operationMapping;
    }

    private string GetOpenApiOperation(MethodInfo method)
    {
        if (method.GetCustomAttribute<HttpGetAttribute>() != null) return "GET";
        if (method.GetCustomAttribute<HttpPostAttribute>() != null) return "POST";
        if (method.GetCustomAttribute<HttpPutAttribute>() != null) return "PUT";
        if (method.GetCustomAttribute<HttpDeleteAttribute>() != null) return "DELETE";
        if (method.GetCustomAttribute<HttpPatchAttribute>() != null) return "PATCH";
        return "UNKNOWN"; // Default
    }

    private OpenApiSchema? ResolveSchema(OpenApiSchema schema, SchemaRepository schemaRepository)
    {
        if (schema == null) return null;

        // If the schema has a reference, resolve it from the components
        if (!string.IsNullOrEmpty(schema.Reference?.Id))
        {
            return schemaRepository.Schemas.ContainsKey(schema.Reference.Id)
                ? schemaRepository.Schemas[schema.Reference.Id]
                : null;
        }

        // If no reference, return the schema as is
        return schema;
    }

    private bool IsSimpleType(OpenApiSchema schema)
    {
        // Simple types are primitive types or well-known types like string, number, boolean, etc.
        return schema.Type == "string" || schema.Type == "number" || schema.Type == "integer" || schema.Type == "boolean";
    }

    private List<OpenApiPropertyMapping> MapProperties(
        IDictionary<string, IOpenApiExtension> parentExtensions,
        OpenApiSchema schema, 
        SchemaRepository schemaRepository,
        OpenApiSchema[] secondarySchemas)
    {
        var propertyMappings = new List<OpenApiPropertyMapping>();

        if (schema?.Properties == null) return propertyMappings;

        foreach (var property in schema.Properties)
        {
            var propertyMapping = new OpenApiPropertyMapping
            {
                PropertyName = property.Key,
                PropertySchema = ResolveSchema(property.Value, schemaRepository),
                IsArray = property.Value.Type == "array", // Check if the property is an array
                ParentPropertyExtensions = schema.Extensions,
                SecondarySchemas = secondarySchemas
                    .Select(x => x.Properties?.ContainsKey(property.Key) == true ? x.Properties[property.Key] : null)
                    .Where(x => x is not null)
                    .ToArray()!
            };

            // If the property is an array, handle its items recursively
            if (propertyMapping.IsArray && propertyMapping.PropertySchema.Items != null)
            {
                // Recurse through the array items (if they exist) to generate property mappings for the array items
                propertyMapping.NestedProperties.AddRange(
                    MapProperties(
                        property.Value.Extensions,
                        propertyMapping.PropertySchema.Items, 
                        schemaRepository,
                        propertyMapping.SecondarySchemas
                            .Select(x => x.Items)
                            .Where(x => x is not null)
                            .ToArray()!
                    )
                );
            }
            else if (propertyMapping.PropertySchema != null && !IsSimpleType(propertyMapping.PropertySchema))
            {
                // If the property is not a simple type, recurse through its properties
                propertyMapping.NestedProperties.AddRange(
                    MapProperties(
                        property.Value.Extensions,
                        propertyMapping.PropertySchema, 
                        schemaRepository,
                        propertyMapping.SecondarySchemas
                    )
                );
            }

            propertyMappings.Add(propertyMapping);
        }

        if (schema.Items is not null)
        {
            propertyMappings.AddRange(
                MapProperties(
                    parentExtensions,
                    schema.Items, 
                    schemaRepository,
                    secondarySchemas.Select(x => x.Items).ToArray()
                )
            );
        }

        return propertyMappings;
    }
}

