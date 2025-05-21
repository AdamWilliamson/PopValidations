//using Microsoft.OpenApi.Models;
//using PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.Helpers;
//using PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.MethodSimplification;
//using PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.OpenApiSimplification;
//using System.Diagnostics;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.OpenApiToMethodMapping;

//internal class ProcessCOmplexProperties
//{

//    private List<TypeToOpenApiMappingResult> ProcessComplexObjectProperties(
//        string route,
//        string httpMethod,
//        string? prefix,
//        string? openApiPrefix,
//        FunctionMapping functionMapping,
//        IOpenApiParameterMapping parameter,
//        IGeneralMapping parameterMapping) // Take the parameter index as an argument
//    {
//        var results = new List<TypeToOpenApiMappingResult>();

//        foreach (var schema in parameter.Schemas)
//        {

//            var newPrefix = string.IsNullOrWhiteSpace(prefix) ? parameter.Name : prefix + '.' + parameter.Name;
//            var newOpenApiPrefix = string.IsNullOrWhiteSpace(openApiPrefix) ? parameter.Name : openApiPrefix + '.' + parameter.Name;
//            var isArray = schema.Items != null;

//            var foundProperty = FindPropertyMapping(parameterMapping, newOpenApiPrefix);
//            if (foundProperty != null)
//            {
//                //if (parameter.IsArray)
//                //{
//                //    Debug.Assert(!foundProperty.ResultPropertyName.EndsWith("[n]"));
//                //    results.Add(new TypeToOpenApiMappingResult
//                //    {
//                //        Route = route,
//                //        OpenApiObjHeirarchy = newPrefix + "[n]",
//                //        OpenApiPropertyName = parameter.Name,
//                //        DirectParentSchema = null,
//                //        PropertySchema = parameter.Schema,
//                //        PropertyMapping = foundProperty,
//                //        ResultPropertyHeirarchy = (foundProperty.ResultPropertyName ?? string.Empty) + "[n]",
//                //        IsArray = parameter.IsArray,
//                //        Parameter = parameter?.Parameter,
//                //        ParameterMapping = parameterMapping, // Set the parameter index
//                //        ParentPropertyExtensions = parameter.ParentPropertyExtensions
//                //    });
//                //    Debug.Assert(results.Last().PropertyMapping != null || results.Last().Parameter != null);
//                //}

//                results.Add(new TypeToOpenApiMappingResult
//                {
//                    Route = route,
//                    OpenApiObjHeirarchy = newPrefix,
//                    OpenApiPropertyName = parameter.Name,
//                    DirectParentSchema = null,
//                    PropertySchema = schema,
//                    PropertyMapping = foundProperty,
//                    ResultPropertyHeirarchy = (foundProperty.ResultPropertyName ?? string.Empty),
//                    IsArray = false,
//                    Parameter = parameter?.Parameter,
//                    //TypeMapping = parameterMapping, // Set the parameter index
//                    ParentPropertyExtensions = parameter.ParentPropertyExtensions
//                });
//                Debug.Assert(results.Last().PropertyMapping != null || results.Last().Parameter != null);
//            }

//            foreach (var property in parameter.PropertyMappings)
//            {
//                // Flatten the properties recursively
//                results.AddRange(ProcessComplexObjectProperties(
//                    route,
//                    httpMethod,
//                    newPrefix + (isArray ? "[n]" : null),
//                    newOpenApiPrefix + (isArray ? "[n]" : string.Empty),
//                    functionMapping,
//                    parameter,
//                    parameterMapping, // Pass the parameter index
//                    property,
//                    schema
//                ));
//            }
//        }

//        return results;
//    }

//    private List<TypeToOpenApiMappingResult> ProcessComplexObjectProperties(
//        string route,
//        string httpMethod,
//        string? prefix,
//        string? openApiPrefix,
//        FunctionMapping functionMapping,
//        IOpenApiParameterMapping? parameter,
//        IGeneralMapping parameterMapping, // Take the parameter index as an argument
//        OpenApiPropertyMapping property,
//        OpenApiSchema? parent = null)
//    {
//        var results = new List<TypeToOpenApiMappingResult>();

//        // Prefix the property hierarchy with the parent (route + operation + property name)
//        var newPrefix = string.IsNullOrWhiteSpace(prefix) ? property.PropertyName : prefix + '.' + property.PropertyName;
//        var newOpenApiPrefix = string.IsNullOrWhiteSpace(openApiPrefix) ? property.PropertyName : openApiPrefix + '.' + property.PropertyName;

//        var foundProperty = FindPropertyMapping(parameterMapping, newOpenApiPrefix);

//        if (foundProperty != null)
//        {
//            //if (property.IsArray)
//            //{
//            //    Debug.Assert(!foundProperty.ResultPropertyName.EndsWith("[n]"));
//            //    results.Add(new TypeToOpenApiMappingResult
//            //    {
//            //        Route = route,
//            //        OpenApiObjHeirarchy = newPrefix + "[n]",
//            //        OpenApiPropertyName = property.PropertyName,
//            //        DirectParentSchema = parent,
//            //        PropertySchema = property.PropertySchema,
//            //        PropertyMapping = foundProperty,
//            //        ResultPropertyHeirarchy = (foundProperty.ResultPropertyName ?? string.Empty) + "[n]",
//            //        IsArray = true,
//            //        Parameter = parameter?.Parameter,
//            //        ParameterMapping = parameterMapping, // Set the parameter index
//            //        RequestBody = null,
//            //        ParentPropertyExtensions = property.ParentPropertyExtensions,
//            //        PropertySecondarySchemas = property.SecondarySchemas
//            //    });
//            //    Debug.Assert(results.Last().PropertyMapping != null || results.Last().Parameter != null);
//            //}

//            results.Add(new TypeToOpenApiMappingResult
//            {
//                Route = route,
//                OpenApiObjHeirarchy = newPrefix,
//                OpenApiPropertyName = property.PropertyName,
//                DirectParentSchema = parent,
//                PropertySchema = property.PropertySchema,
//                PropertyMapping = foundProperty,
//                ResultPropertyHeirarchy = (foundProperty.ResultPropertyName ?? string.Empty),
//                IsArray = false,
//                Parameter = parameter?.Parameter,
//                ParameterMapping = parameterMapping, // Set the parameter index
//                RequestBody = null,
//                ParentPropertyExtensions = property.ParentPropertyExtensions,
//                PropertySecondarySchemas = property.SecondarySchemas
//            });
//            Debug.Assert(results.Last().PropertyMapping != null || results.Last().Parameter != null);

//            // Process nested properties recursively
//            foreach (var nestedProperty in property.NestedProperties)
//            {
//                results.AddRange(ProcessComplexObjectProperties(
//                    route,
//                    httpMethod,
//                    newPrefix + (property.IsArray ? "[n]" : string.Empty),
//                    newOpenApiPrefix + (property.IsArray ? "[n]" : string.Empty),
//                    functionMapping,
//                    parameter,
//                    parameterMapping, // Pass the parameter index
//                    nestedProperty,
//                    property?.PropertySchema
//                ));
//            }
//        }
//        return results;
//    }

//    // New method for handling complex request body properties
//    private List<TypeToOpenApiMappingResult> ProcessComplexObjectProperties(
//        string route,
//        string httpMethod,
//        string prefix,
//        string? openApiPrefix,
//        FunctionMapping functionMapping,
//        IOpenApiParameterMapping requestBody,
//        ParameterMapping parameterMapping, // Take the parameter index as an argument
//        OpenApiPropertyMapping property,
//        OpenApiSchema? parent = null)
//    {
//        var results = new List<TypeToOpenApiMappingResult>();

//        // Prefix the property hierarchy with the parent (route + operation + property name)
//        var newPrefix = string.IsNullOrWhiteSpace(prefix) ? property.PropertyName : prefix + '.' + property.PropertyName;
//        var newOpenApiPrefix = string.IsNullOrWhiteSpace(openApiPrefix) ? property.PropertyName : openApiPrefix + '.' + property.PropertyName;

//        var foundProperty = FindPropertyMapping(parameterMapping, newOpenApiPrefix);

//        //if (property.IsArray)
//        //{
//        //    Debug.Assert(!foundProperty.ResultPropertyName.EndsWith("[n]"));
//        //    results.Add(new TypeToOpenApiMappingResult
//        //    {
//        //        Route = route,
//        //        OpenApiObjHeirarchy = newPrefix + "[n]",
//        //        OpenApiPropertyName = property.PropertyName + "[n]",
//        //        DirectParentSchema = parent,
//        //        PropertySchema = property.PropertySchema,
//        //        PropertyMapping = foundProperty,
//        //        ResultPropertyHeirarchy = (foundProperty.ResultPropertyName ?? string.Empty) + "[n]",
//        //        IsArray = true,
//        //        Parameter = null, // No parameter for request body
//        //        ParameterMapping = parameterMapping, // No parameter index for request body directly
//        //        RequestBody = requestBody?.RequestBody,
//        //        ParentPropertyExtensions = property.ParentPropertyExtensions,
//        //        PropertySecondarySchemas = property.SecondarySchemas
//        //    });
//        //    Debug.Assert(results.Last().PropertyMapping != null || results.Last().Parameter != null);
//        //}

//        results.Add(new TypeToOpenApiMappingResult
//        {
//            Route = route,
//            OpenApiObjHeirarchy = newPrefix,
//            OpenApiPropertyName = property.PropertyName,
//            DirectParentSchema = parent,
//            PropertySchema = property.PropertySchema,
//            PropertyMapping = foundProperty,
//            ResultPropertyHeirarchy = (foundProperty.ResultPropertyName ?? string.Empty),
//            IsArray = false,
//            Parameter = null, // No parameter for request body
//            ParameterMapping = parameterMapping, // No parameter index for request body directly
//            RequestBody = requestBody?.RequestBody,
//            ParentPropertyExtensions = property.ParentPropertyExtensions,
//            PropertySecondarySchemas = property.SecondarySchemas
//        });
//        Debug.Assert(results.Last().PropertyMapping != null || results.Last().Parameter != null);

//        // Process nested properties recursively
//        foreach (var nestedProperty in property.NestedProperties)
//        {
//            results.AddRange(ProcessComplexObjectProperties(
//                route,
//                httpMethod,
//                newPrefix + (property.IsArray ? "[n]" : string.Empty),
//                newOpenApiPrefix + (property.IsArray ? "[n]" : string.Empty),
//                functionMapping,
//                requestBody,
//                parameterMapping, // Pass the parameter index
//                nestedProperty,
//                property.PropertySchema
//            ));
//        }

//        return results;
//    }

//    private List<TypeToOpenApiMappingResult> ProcessComplexObjectProperties(
//        string route,
//        string httpMethod,
//        string prefix,
//        string? openApiPrefix,
//        FunctionMapping functionMapping,
//        ReturnMapping returnMapping,
//        IOpenApiParameterMapping returnResponse,
//        OpenApiPropertyMapping property,
//        OpenApiSchema? parent = null)
//    {
//        var results = new List<TypeToOpenApiMappingResult>();

//        // Prefix the property hierarchy with the parent (route + operation + property name)
//        var newPrefix = string.IsNullOrWhiteSpace(prefix) ? property.PropertyName : prefix + '.' + property.PropertyName;
//        var newOpenApiPrefix = string.IsNullOrWhiteSpace(openApiPrefix) ? property.PropertyName : openApiPrefix + '.' + property.PropertyName;

//        var foundProperty = FindPropertyMapping(returnMapping, newOpenApiPrefix);

//        //if (property.IsArray)
//        //{
//        //    Debug.Assert(!foundProperty.ResultPropertyName.EndsWith("[n]"));
//        //    results.Add(new TypeToOpenApiMappingResult
//        //    {
//        //        Route = route,
//        //        OpenApiObjHeirarchy = newPrefix + "[n]",
//        //        OpenApiPropertyName = property.PropertyName + "[n]",
//        //        DirectParentSchema = parent,
//        //        PropertySchema = property.PropertySchema,
//        //        PropertyMapping = foundProperty,
//        //        ResultPropertyHeirarchy = (foundProperty.ResultPropertyName ?? string.Empty) + "[n]",
//        //        IsArray = true,
//        //        Parameter = null,
//        //        ParameterMapping = null,
//        //        RequestBody = null,
//        //        ParentPropertyExtensions = property.ParentPropertyExtensions,
//        //        PropertySecondarySchemas = property.SecondarySchemas,
//        //        ResponseMapping = returnResponse,
//        //        ReturnMapping = returnMapping,
//        //    });
//        //    Debug.Assert(results.Last().PropertyMapping != null || results.Last().Parameter != null);
//        //}

//        results.Add(new TypeToOpenApiMappingResult
//        {
//            Route = route,
//            OpenApiObjHeirarchy = newPrefix,
//            OpenApiPropertyName = property.PropertyName,
//            DirectParentSchema = parent,
//            PropertySchema = property.PropertySchema,
//            PropertyMapping = foundProperty,
//            ResultPropertyHeirarchy = (foundProperty.ResultPropertyName ?? string.Empty),
//            IsArray = false,
//            Parameter = null,
//            ParameterMapping = null,
//            RequestBody = null,
//            ParentPropertyExtensions = property.ParentPropertyExtensions,
//            PropertySecondarySchemas = property.SecondarySchemas,
//            ResponseMapping = returnResponse,
//            ReturnMapping = returnMapping,
//        });
//        Debug.Assert(results.Last().PropertyMapping != null || results.Last().Parameter != null);

//        // Process nested properties recursively
//        foreach (var nestedProperty in property.NestedProperties)
//        {
//            results.AddRange(ProcessComplexObjectProperties(
//                route,
//                httpMethod,
//                newPrefix + (property.IsArray ? "[n]" : string.Empty),
//                newOpenApiPrefix + (property.IsArray ? "[n]" : string.Empty),
//                functionMapping,
//                returnMapping,
//                returnResponse,
//                nestedProperty,
//                property?.PropertySchema
//            ));
//        }

//        return results;
//    }

//    public PropertyMapping? FindPropertyMapping(IGeneralMapping mapping, string parameterName)
//    {
//        parameterName = parameterName.Replace("[n]", "").Replace("[n.Key]", "").Replace("[n.Value]", "");

//        return mapping.GetOpenApiPropertyNames()
//            .FirstOrDefault(y => string.Equals(y.Item1, parameterName, StringComparison.InvariantCultureIgnoreCase))
//            .Item2;
//    }
//}
