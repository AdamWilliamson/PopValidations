using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;

namespace PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3
{
    public class TypeToOpenApiMappingResult
    {
        public string Route { get; set; }
        public string ResultPropertyHeirarchy => PropertyMapping?.ResultPropertyName ?? string.Empty;
        public string OpenApiObjHeirarchy { get; set; }
        public string OpenApiPropertyName { get; set; }
        public PropertyMapping? PropertyMapping { get; set; }
        public OpenApiSchema? PropertySchema { get; set; }
        public OpenApiSchema[]? PropertySecondarySchemas { get; set; }
        public IDictionary<string, IOpenApiExtension> ParentPropertyExtensions { get; set; }
        public OpenApiParameter? Parameter { get; set; }
        public OpenApiRequestBody? RequestBody { get; set; }
        public OpenApiSchema[] RequestBodyContentSchemas => RequestBody?.Content.Select(c => c.Value.Schema)?.ToArray() ?? new OpenApiSchema[0];
        public bool IsArray { get; set; }
        public ParameterMapping ParameterMapping { get; set; } // Added to store the method parameter index
        public OpenApiResponseMapping? ResponseMapping { get; set; }
        public ReturnMapping? ReturnMapping { get; set; }
    }

    public class OpenApiToTypeMapper
    {
        public List<TypeToOpenApiMappingResult> MapOpenApiOperationToFunction(
            OpenApiOperationMapping operationMapping,
            FunctionMapping functionMapping)
        {
            var results = new List<TypeToOpenApiMappingResult>();

            // Process the parameters of the OpenApiOperationMapping
            foreach (var parameter in operationMapping.Parameters)
            {
                var parameterMapping = FindFunction(functionMapping, parameter.Name);

                if (parameter.PropertyMappings?.Any() == true)
                {
                    // Process complex object properties for Body parameters
                    results.AddRange(ProcessComplexObjectProperties(
                        operationMapping.Path,
                        operationMapping.HttpMethod,
                        string.Empty,
                        null,
                        functionMapping,
                        parameter,
                        parameterMapping // Pass the parameter index
                    ));
                }
                else
                {
                    // Process the other parameters directly
                    results.AddRange(ProcessSimpleOpenApiParameterMapping(
                        operationMapping.Path,
                        operationMapping.HttpMethod,
                        string.Empty,
                        parameter,
                        parameterMapping // Pass the parameter index
                    ));
                }
            }

            // Process the request body
            if (operationMapping.RequestBody?.ContentSchemas?.Any() == true)
            {
                var parameterMapping = FindFunctionForRequestBody(functionMapping);
                var isArray = false;

                foreach (var bodySchema in operationMapping.RequestBody.ContentSchemas.Take(1))
                {
                    isArray = bodySchema.Value.Items != null;

                    if (isArray)
                    {
                        results.Add(new TypeToOpenApiMappingResult
                        {
                            Route = operationMapping.Path,
                            OpenApiObjHeirarchy = "RequestBody[n]",
                            OpenApiPropertyName = "RequestBody[n]",
                            PropertyMapping = null,
                            PropertySchema = bodySchema.Value,
                            ParentPropertyExtensions = operationMapping.RequestBody.ParentPropertyExtensions,
                            IsArray = true,
                            RequestBody = operationMapping.RequestBody.RequestBody,
                            //RequestBodyContentSchemas = operationMapping.RequestBody.RequestBody.Content.Select(x => x.Value.Schema).ToArray(),
                            ParameterMapping = parameterMapping // No specific parameter, so set to -1
                        });
                    }

                    results.Add(new TypeToOpenApiMappingResult
                    {
                        Route = operationMapping.Path,
                        OpenApiObjHeirarchy = "RequestBody",
                        OpenApiPropertyName = "RequestBody",
                        PropertyMapping = null,
                        PropertySchema = bodySchema.Value,
                        ParentPropertyExtensions = operationMapping.RequestBody.ParentPropertyExtensions,
                        IsArray = false,
                        RequestBody = operationMapping.RequestBody.RequestBody,
                        //RequestBodyContentSchemas = operationMapping.RequestBody.RequestBody.Content.Select(x => x.Value.Schema).ToArray(),
                        ParameterMapping = parameterMapping // No specific parameter, so set to -1
                    });
                }

                foreach (var property in operationMapping.RequestBody.PropertyMappings)
                {
                    results.AddRange(ProcessComplexObjectProperties(
                        operationMapping.Path,
                        operationMapping.HttpMethod,
                        "RequestBody" + (isArray ? "[n]" : null),
                        "RequestBody" + (isArray? "[n]": string.Empty),
                        functionMapping,
                        operationMapping.RequestBody,
                        parameterMapping, // No parameter index for the body directly
                        property
                    ));
                }
            }

            if (operationMapping.Responses?.Any() == true)
            {
                foreach (var response in operationMapping.Responses.Where(x => x.StatusCode == "200" && x.Schema != null))
                {
                    var isArray = response.Schema.Items != null;

                    if (isArray)
                    {
                        results.Add(new TypeToOpenApiMappingResult
                        {
                            Route = operationMapping.Path,
                            OpenApiObjHeirarchy = "[n]",
                            OpenApiPropertyName = "Response[n]",
                            PropertyMapping = null,
                            PropertySchema = null,
                            ParentPropertyExtensions = operationMapping.Extensions,
                            IsArray = true,
                            RequestBody = null,
                            ParameterMapping = null,
                            ResponseMapping = response,
                            ReturnMapping = functionMapping.Return.First(),
                        });
                    }

                    results.Add(new TypeToOpenApiMappingResult
                    {
                        Route = operationMapping.Path,
                        OpenApiObjHeirarchy = null,
                        OpenApiPropertyName = "Response",
                        PropertyMapping = null,
                        PropertySchema = null,
                        ParentPropertyExtensions = operationMapping.Extensions,
                        IsArray = false,
                        RequestBody = null,
                        ParameterMapping = null,
                        ResponseMapping = response,
                        ReturnMapping = functionMapping.Return.First(),
                    });

                    foreach (var property in response.PropertyMappings)
                    {
                        results.AddRange(ProcessComplexObjectProperties(
                           operationMapping.Path,
                           operationMapping.HttpMethod,
                           (isArray ? "[n]" : string.Empty),
                           "Response" + (isArray ? "[n]" : string.Empty),
                           functionMapping,
                           functionMapping.Return.First(),
                           response,
                           property
                       ));
                    }
                }
            }

            return results;
        }

        public ParameterMapping FindFunction(FunctionMapping functionMapping, string parameterName)
        {
            parameterName = parameterName.Replace("[n]", "").Replace("[n.Key]", "").Replace("[n.Value]", "");

            return
                functionMapping.Parameters.FirstOrDefault(x => x.OpenApiParameterName == parameterName) 
                ?? functionMapping.Parameters
                    .First(x => 
                        x.GetOpenApiPropertyNames()
                            .Any(y => string.Equals(y.Item1, parameterName, StringComparison.InvariantCultureIgnoreCase)
                        )
                    );
        }

        //public PropertyMapping? FindPropertyMapping(FunctionMapping functionMapping, string parameterName)
        //{
        //    parameterName = parameterName.Replace("[n]", "").Replace("[n.Key]", "").Replace("[n.Value]", "");

        //    return functionMapping.Parameters.SelectMany(x => x.GetOpenApiPropertyNames())
        //        .FirstOrDefault(y => string.Equals(y.Item1, parameterName, StringComparison.InvariantCultureIgnoreCase))
        //        .Item2;
        //}

        public PropertyMapping? FindPropertyMapping(ReturnMapping returnMapping, string parameterName)
        {
            parameterName = parameterName.Replace("[n]", "").Replace("[n.Key]", "").Replace("[n.Value]", "");

            return returnMapping.GetOpenApiPropertyNames()
                .FirstOrDefault(y => string.Equals(y.Item1, parameterName, StringComparison.InvariantCultureIgnoreCase))
                .Item2;
        }

        public PropertyMapping? FindPropertyMapping(ParameterMapping parameterMapping, string parameterName)
        {
            parameterName = parameterName.Replace("[n]", "").Replace("[n.Key]", "").Replace("[n.Value]", "");

            return parameterMapping.GetOpenApiPropertyNames()
                .FirstOrDefault(y => string.Equals(y.Item1, parameterName, StringComparison.InvariantCultureIgnoreCase))
                .Item2;
        }

        public ParameterMapping FindFunctionForRequestBody(FunctionMapping functionMapping)
        {
            return functionMapping.Parameters.OrderBy(x => x.ParameterInfo.Position).First(x => x.IsOpenApiRequestBody);
        }



        private List<TypeToOpenApiMappingResult> ProcessSimpleOpenApiParameterMapping(
            string route,
            string httpMethod,
            string? prefix,
            OpenApiParameterMapping parameter,
            ParameterMapping parameterMapping) // Take the parameter index as an argument
        {
            var results = new List<TypeToOpenApiMappingResult>();
            //var newPrefix = string.IsNullOrWhiteSpace(prefix) ? parameter.Name : prefix + '.' + parameter.Name;
            // If it's an array, treat it as a list of items
            if (parameter.IsArray)
            {
                results.Add(new TypeToOpenApiMappingResult
                {
                    Route = route,
                    OpenApiObjHeirarchy = parameter.Name + "[n]",
                    OpenApiPropertyName = parameter.Name + "[n]",
                    PropertyMapping = null,
                    PropertySchema = parameter.Schema,
                    IsArray = true,
                    Parameter = parameter.Parameter,
                    ParameterMapping = parameterMapping, // Set the parameter index
                    ParentPropertyExtensions = parameter.ParentPropertyExtensions,
                });
            }
            
            results.Add(new TypeToOpenApiMappingResult
            {
                Route = route,
                OpenApiObjHeirarchy = parameter.Name,
                OpenApiPropertyName = parameter.Name,
                PropertyMapping = null,
                PropertySchema = parameter.Schema,
                IsArray = false,
                Parameter = parameter.Parameter,
                ParameterMapping = parameterMapping, // Set the parameter index
                ParentPropertyExtensions = parameter.ParentPropertyExtensions,
            });

            return results;
        }

        private List<TypeToOpenApiMappingResult> ProcessComplexObjectProperties(
            string route,
            string httpMethod,
            string? prefix,
            string? openApiPrefix,
            FunctionMapping functionMapping,
            OpenApiParameterMapping parameter,
            ParameterMapping parameterMapping) // Take the parameter index as an argument
        {
            var results = new List<TypeToOpenApiMappingResult>();

            var newPrefix = string.IsNullOrWhiteSpace(prefix) ? parameter.Name : prefix + '.' + parameter.Name;
            var newOpenApiPrefix = string.IsNullOrWhiteSpace(openApiPrefix) ? parameter.Name : openApiPrefix + '.' + parameter.Name;
            var isArray = parameter.Schema.Items != null;

            if (parameter.IsArray)
            {
                results.Add(new TypeToOpenApiMappingResult
                {
                    Route = route,
                    OpenApiObjHeirarchy = newPrefix + "[n]",
                    OpenApiPropertyName = parameter.Name,
                    PropertySchema = parameter.Schema,
                    PropertyMapping = FindPropertyMapping(parameterMapping, newOpenApiPrefix),
                    IsArray = parameter.IsArray,
                    Parameter = parameter?.Parameter,
                    ParameterMapping = parameterMapping, // Set the parameter index
                    ParentPropertyExtensions = parameter.ParentPropertyExtensions
                });
                Debug.Assert(results.Last().PropertyMapping != null || results.Last().Parameter != null);
            }


            results.Add(new TypeToOpenApiMappingResult
            {
                Route = route,
                OpenApiObjHeirarchy = newPrefix,
                OpenApiPropertyName = parameter.Name,
                PropertySchema = parameter.Schema,
                PropertyMapping = FindPropertyMapping(parameterMapping, newOpenApiPrefix),
                IsArray = false,
                Parameter = parameter?.Parameter,
                ParameterMapping = parameterMapping, // Set the parameter index
                ParentPropertyExtensions = parameter.ParentPropertyExtensions
            });
            Debug.Assert(results.Last().PropertyMapping != null || results.Last().Parameter != null);

            foreach (var property in parameter.PropertyMappings)
            {
                // Flatten the properties recursively
                results.AddRange(ProcessComplexObjectProperties(
                    route,
                    httpMethod,
                    newPrefix+ (isArray? "[n]" : null),
                    newOpenApiPrefix + (isArray ? "[n]" : string.Empty),
                    functionMapping,
                    parameter,
                    parameterMapping, // Pass the parameter index
                    property
                ));
            }

            return results;
        }

        private List<TypeToOpenApiMappingResult> ProcessComplexObjectProperties(
            string route,
            string httpMethod,
            string? prefix,
            string? openApiPrefix,
            FunctionMapping functionMapping,
            OpenApiParameterMapping? parameter,
            ParameterMapping parameterMapping, // Take the parameter index as an argument
            OpenApiPropertyMapping property)
        {
            var results = new List<TypeToOpenApiMappingResult>();

            // Prefix the property hierarchy with the parent (route + operation + property name)
            var newPrefix = string.IsNullOrWhiteSpace(prefix) ? property.PropertyName : prefix + '.' + property.PropertyName;
            var newOpenApiPrefix = string.IsNullOrWhiteSpace(openApiPrefix) ? property.PropertyName : openApiPrefix + '.' + property.PropertyName;

            if (property.IsArray)
            {
                results.Add(new TypeToOpenApiMappingResult
                {
                    Route = route,
                    OpenApiObjHeirarchy = newPrefix + "[n]",
                    OpenApiPropertyName = property.PropertyName,
                    PropertySchema = property.PropertySchema,
                    PropertyMapping = FindPropertyMapping(parameterMapping, newOpenApiPrefix),
                    IsArray = true,
                    Parameter = parameter?.Parameter,
                    ParameterMapping = parameterMapping, // Set the parameter index
                    RequestBody = null,
                    ParentPropertyExtensions = property.ParentPropertyExtensions,
                    PropertySecondarySchemas = property.SecondarySchemas
                });
                Debug.Assert(results.Last().PropertyMapping != null || results.Last().Parameter != null);
            }

            results.Add(new TypeToOpenApiMappingResult
            {
                Route = route,
                OpenApiObjHeirarchy = newPrefix,
                OpenApiPropertyName = property.PropertyName,
                PropertySchema = property.PropertySchema,
                PropertyMapping = FindPropertyMapping(parameterMapping, newOpenApiPrefix),
                IsArray = false,
                Parameter = parameter?.Parameter,
                ParameterMapping = parameterMapping, // Set the parameter index
                RequestBody = null,
                ParentPropertyExtensions = property.ParentPropertyExtensions,
                PropertySecondarySchemas = property.SecondarySchemas
            });
            Debug.Assert(results.Last().PropertyMapping != null || results.Last().Parameter != null);

            // Process nested properties recursively
            foreach (var nestedProperty in property.NestedProperties)
            {
                results.AddRange(ProcessComplexObjectProperties(
                    route,
                    httpMethod,
                    newPrefix + (property.IsArray ? "[n]" : string.Empty),
                    newOpenApiPrefix + (property.IsArray ? "[n]" : string.Empty),
                    functionMapping,
                    parameter,
                    parameterMapping, // Pass the parameter index
                    nestedProperty
                ));
            }

            return results;
        }

        // New method for handling complex request body properties
        private List<TypeToOpenApiMappingResult> ProcessComplexObjectProperties(
            string route,
            string httpMethod,
            string prefix,
            string? openApiPrefix,
            FunctionMapping functionMapping,
            OpenApiRequestBodyMapping requestBody,
            ParameterMapping parameterMapping, // Take the parameter index as an argument
            OpenApiPropertyMapping property)
        {
            var results = new List<TypeToOpenApiMappingResult>();

            // Prefix the property hierarchy with the parent (route + operation + property name)
            var newPrefix = string.IsNullOrWhiteSpace(prefix) ? property.PropertyName : prefix + '.' + property.PropertyName;
            var newOpenApiPrefix = string.IsNullOrWhiteSpace(openApiPrefix) ? property.PropertyName: openApiPrefix + '.' + property.PropertyName;

            if (property.IsArray)
            {
                results.Add(new TypeToOpenApiMappingResult
                {
                    Route = route,
                    OpenApiObjHeirarchy = newPrefix + "[n]",
                    OpenApiPropertyName = property.PropertyName + "[n]",
                    PropertySchema = property.PropertySchema,
                    PropertyMapping = FindPropertyMapping(parameterMapping, newOpenApiPrefix),
                    IsArray = true,
                    Parameter = null, // No parameter for request body
                    ParameterMapping = parameterMapping, // No parameter index for request body directly
                    RequestBody = requestBody?.RequestBody,
                    ParentPropertyExtensions = property.ParentPropertyExtensions,
                    PropertySecondarySchemas = property.SecondarySchemas
                });
                Debug.Assert(results.Last().PropertyMapping != null);
            }

            results.Add(new TypeToOpenApiMappingResult
            {
                Route = route,
                OpenApiObjHeirarchy = newPrefix,
                OpenApiPropertyName = property.PropertyName,
                PropertySchema = property.PropertySchema,
                PropertyMapping = FindPropertyMapping(parameterMapping, newOpenApiPrefix),
                IsArray = false,
                Parameter = null, // No parameter for request body
                ParameterMapping = parameterMapping, // No parameter index for request body directly
                RequestBody = requestBody?.RequestBody,
                ParentPropertyExtensions = property.ParentPropertyExtensions,
                PropertySecondarySchemas = property.SecondarySchemas
            });
            Debug.Assert(results.Last().PropertyMapping != null);

            // Process nested properties recursively
            foreach (var nestedProperty in property.NestedProperties)
            {
                results.AddRange(ProcessComplexObjectProperties(
                    route,
                    httpMethod,
                    newPrefix + (property.IsArray ? "[n]" : string.Empty),
                    newOpenApiPrefix + (property.IsArray ? "[n]" : string.Empty),
                    functionMapping,
                    requestBody,
                    parameterMapping, // Pass the parameter index
                    nestedProperty
                ));
            }

            return results;
        }

        private List<TypeToOpenApiMappingResult> ProcessComplexObjectProperties(
            string route,
            string httpMethod,
            string prefix,
            string? openApiPrefix,
            FunctionMapping functionMapping,
            ReturnMapping returnMapping,
            OpenApiResponseMapping returnResponse,
            OpenApiPropertyMapping property)
        {
            var results = new List<TypeToOpenApiMappingResult>();

            // Prefix the property hierarchy with the parent (route + operation + property name)
            var newPrefix = string.IsNullOrWhiteSpace(prefix) ? property.PropertyName : prefix + '.' + property.PropertyName;
            var newOpenApiPrefix = string.IsNullOrWhiteSpace(openApiPrefix) ? property.PropertyName : openApiPrefix + '.' + property.PropertyName;

            if (property.IsArray)
            {
                results.Add(new TypeToOpenApiMappingResult
                {
                    Route = route,
                    OpenApiObjHeirarchy = newPrefix + "[n]",
                    OpenApiPropertyName = property.PropertyName + "[n]",
                    PropertySchema = property.PropertySchema,
                    PropertyMapping = FindPropertyMapping(returnMapping, newOpenApiPrefix),
                    IsArray = true,
                    Parameter = null,
                    ParameterMapping = null,
                    RequestBody = null,
                    ParentPropertyExtensions = property.ParentPropertyExtensions,
                    PropertySecondarySchemas = property.SecondarySchemas,
                    ResponseMapping = returnResponse,
                    ReturnMapping = returnMapping,
                });
                Debug.Assert(results.Last().PropertyMapping != null);
            }

            results.Add(new TypeToOpenApiMappingResult
            {
                Route = route,
                OpenApiObjHeirarchy = newPrefix,
                OpenApiPropertyName = property.PropertyName,
                PropertySchema = property.PropertySchema,
                PropertyMapping = FindPropertyMapping(returnMapping, newOpenApiPrefix),
                IsArray = false,
                Parameter = null,
                ParameterMapping = null, 
                RequestBody = null,
                ParentPropertyExtensions = property.ParentPropertyExtensions,
                PropertySecondarySchemas = property.SecondarySchemas,
                ResponseMapping = returnResponse,
                ReturnMapping = returnMapping,
            });
            Debug.Assert(results.Last().PropertyMapping != null);

            // Process nested properties recursively
            foreach (var nestedProperty in property.NestedProperties)
            {
                results.AddRange(ProcessComplexObjectProperties(
                    route,
                    httpMethod,
                    newPrefix + (property.IsArray ? "[n]" : string.Empty),
                    newOpenApiPrefix + (property.IsArray ? "[n]" : string.Empty),
                    functionMapping,
                    returnMapping,
                    returnResponse,
                    nestedProperty
                ));
            }

            return results;
        }
    }
}
