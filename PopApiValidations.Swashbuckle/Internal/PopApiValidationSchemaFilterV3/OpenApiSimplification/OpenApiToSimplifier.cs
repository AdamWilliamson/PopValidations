using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Interfaces;
using PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.Helpers;

namespace PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.OpenApiSimplification;

public class OpenApiToSimplifier
{
    public OpenApiOperationMapping MapOpenApiOperation(OpenApiOperation operation, SchemaRepository schemaRepository, MethodInfo methodInfo)
    {
        var operationMapping = new OpenApiOperationMapping
        {
            Path = operation.OperationId,
            HttpMethod = MethodHelper.GetOpenApiOperation(methodInfo),
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
            if (parameterMapping.Schema != null && !SchemaHelper.IsSimpleType(parameterMapping.Schema))
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
            if (schema != null && !SchemaHelper.IsSimpleType(schema))
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

                if (responseMapping.Schema != null && !SchemaHelper.IsSimpleType(responseMapping.Schema))
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
                DirectParentSchema = schema,
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
            else if (propertyMapping.PropertySchema != null && !SchemaHelper.IsSimpleType(propertyMapping.PropertySchema))
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

