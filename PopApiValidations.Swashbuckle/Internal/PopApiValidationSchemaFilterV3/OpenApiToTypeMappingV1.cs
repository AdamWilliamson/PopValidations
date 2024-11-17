//using Microsoft.AspNetCore.Mvc.ModelBinding;
//using Microsoft.AspNetCore.Routing;
//using Microsoft.OpenApi.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection.Metadata;
//using System.Text;
//using System.Threading.Tasks;

//namespace PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3;

//public class TypeToOpenApiMappingResult
//{
//    public string Route { get; set; }
//    public string ObjHeirarchy { get; set; }
//    public OpenApiSchema? PropertySchema { get; set; }
//    public OpenApiParameter? Parameter { get; set; }
//    public OpenApiRequestBody? RequestBody { get; set; }
//    public OpenApiSchema[] RequestBodyContentSchemas { get; set; }
//    public bool IsArray { get; set; }
//    public int ParameterIndex { get; set; }
//}

//public class OpenApiToTypeMapper
//{
//    public List<TypeToOpenApiMappingResult> MapOpenApiOperationToFunction(
//        OpenApiOperationMapping operationMapping,
//        FunctionMapping functionMapping)
//    {
//        var results = new List<TypeToOpenApiMappingResult>();

//        // Process the parameters of the OpenApiOperationMapping
//        foreach (var parameter in operationMapping.Parameters)
//        {
//            if (parameter.PropertyMappings?.Any() == true)
//            {
//                // Process complex object properties for Body parameters
//                results.AddRange(ProcessComplexObjectProperties(
//                    operationMapping.Path,
//                    operationMapping.HttpMethod,
//                    null,
//                    functionMapping,
//                    parameter
//                ));
//            }
//            else
//            {
//                // Process the other parameters directly
//                results.AddRange(ProcessSimpleOpenApiParameterMapping(
//                    operationMapping.Path,
//                    operationMapping.HttpMethod,
//                    null,
//                    parameter
//                ));
//            }
//        }

//        // Process the request body
//        if (operationMapping.RequestBody != null)
//        {
//            // Only do the first one.  it will stand in for all of them.
//            foreach (var bodySchema in operationMapping.RequestBody.ContentSchemas.Take(1))
//            {
//                results.Add(new TypeToOpenApiMappingResult
//                {
//                    Route = operationMapping.Path,
//                    ObjHeirarchy = "RequestBody",
//                    PropertySchema = bodySchema.Value,
//                    IsArray = bodySchema.Value.Items != null,
//                    RequestBody = operationMapping.RequestBody.RequestBody,
//                    RequestBodyContentSchemas = operationMapping.RequestBody.RequestBody.Content.Select(x => x.Value.Schema).ToArray()
//                });
//            }

//            foreach (var property in operationMapping.RequestBody.PropertyMappings)
//            {
//                results.AddRange(ProcessComplexObjectProperties(
//                    operationMapping.Path,
//                    operationMapping.HttpMethod,
//                    "RequestBody",
//                    functionMapping,
//                    null,
//                    operationMapping.RequestBody,
//                    property
//                ));
//            }
//        }

//        return results;
//    }

//    private List<TypeToOpenApiMappingResult> ProcessSimpleOpenApiParameterMapping(
//        string route,
//        string httpMethod,
//        string? prefix,
//        OpenApiParameterMapping parameter)
//    {
//        var results = new List<TypeToOpenApiMappingResult>();
//        var newPrefix = string.IsNullOrWhiteSpace(prefix) ? parameter.Name : prefix + '.' + parameter.Name;

//        // If it's an array, treat it as a list of items
//        if (parameter.IsArray)
//        {
//            results.Add(new TypeToOpenApiMappingResult
//            {
//                Route = route,
//                ObjHeirarchy = newPrefix,
//                PropertySchema = parameter.Schema,
//                IsArray = true,
//                Parameter = parameter.Parameter
//            });
//        }
//        else
//        {
//            results.Add(new TypeToOpenApiMappingResult
//            {
//                Route = route,
//                ObjHeirarchy = newPrefix,
//                PropertySchema = parameter.Schema,
//                IsArray = false,
//                Parameter = parameter.Parameter
//            });
//        }

//        return results;
//    }

//    private List<TypeToOpenApiMappingResult> ProcessComplexObjectProperties(
//        string route,
//        string httpMethod,
//        string? prefix,
//        FunctionMapping functionMapping,
//        OpenApiParameterMapping parameter)
//    {
//        var results = new List<TypeToOpenApiMappingResult>();

//        if (parameter.IsArray)
//        {
//            var newPrefix = string.IsNullOrWhiteSpace(prefix) ? parameter.Name: prefix + '.' + parameter.Name;

//            results.Add(new TypeToOpenApiMappingResult
//            {
//                Route = route,
//                ObjHeirarchy = newPrefix,
//                PropertySchema = parameter.Schema,
//                IsArray = parameter.IsArray,
//                Parameter = parameter?.Parameter,
//                RequestBody = null,
//            });
//        }

//        foreach (var property in parameter.PropertyMappings)
//        {
//            // Flatten the properties recursively
//            results.AddRange(ProcessComplexObjectProperties(
//                route,
//                httpMethod,
//                //prefix,
//                parameter.Name,
//                functionMapping,
//                parameter,
//                null,
//                property
//            ));
//        }

//        return results;
//    }

//    private List<TypeToOpenApiMappingResult> ProcessComplexObjectProperties(
//        string route,
//        string httpMethod,
//        string prefix,
//        FunctionMapping functionMapping,
//        OpenApiParameterMapping? parameter,
//        OpenApiRequestBodyMapping? requestBody,
//        OpenApiPropertyMapping property)
//    {
//        var results = new List<TypeToOpenApiMappingResult>();

//        // Prefix the property hierarchy with the parent (route + operation + property name)
//        var newPrefix = string.IsNullOrWhiteSpace(prefix) ? property.PropertyName : prefix + '.' + property.PropertyName;

//        results.Add(new TypeToOpenApiMappingResult
//        {
//            Route = route,
//            ObjHeirarchy = newPrefix,
//            PropertySchema = property.PropertySchema,
//            IsArray = property.IsArray,
//            Parameter = parameter?.Parameter,
//            RequestBody = requestBody?.RequestBody
//        });

//        // Process nested properties recursively
//        foreach (var nestedProperty in property.NestedProperties)
//        {
//            results.AddRange(ProcessComplexObjectProperties(
//                route,
//                httpMethod,
//                newPrefix,
//                functionMapping,
//                parameter,
//                requestBody,
//                nestedProperty
//            ));
//        }

//        return results;
//    }
//}