using Microsoft.OpenApi.Models;
using PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.MethodSimplification;
using PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.OpenApiSimplification;
using System.Diagnostics;

namespace PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.OpenApiToMethodMapping;

public class OpenApiToTypeMapper
{
    private const string OpenApiPrefix = "Response";

    public List<TypeToOpenApiMappingResult> MapOpenApiOperationToFunction(
        OpenApiOperationMapping operationMapping,
        FunctionMapping functionMapping)
    {
        var results = new List<TypeToOpenApiMappingResult>();

        // Process the parameters of the OpenApiOperationMapping
        foreach (var parameter in operationMapping.Parameters)
        {
            results.AddRange(CreateParameter(operationMapping, functionMapping, parameter));
        }

        // Process the request body
        if (operationMapping.RequestBody?.ContentSchemas?.Any() == true)
        {
            results.AddRange(CreateRequestBody(operationMapping, functionMapping));
        }

        foreach (var response in operationMapping.Responses?.Where(x => x.StatusCode == "200" && x.Schema != null) ?? [])
        {
            results.AddRange(CreateResponse(operationMapping, functionMapping, response));       
        }

        return results;
    }

    private List<TypeToOpenApiMappingResult> CreateParameter(OpenApiOperationMapping operationMapping, FunctionMapping functionMapping, OpenApiParameterMapping parameter)
    {
        var results = new List<TypeToOpenApiMappingResult>();

        var parameterMapping = FindFunction(functionMapping, parameter.Name);
        var property = FindPropertyMapping(parameterMapping, parameter.Name);

        results.Add(TypeToOpenApiMappingResult.ForParameter(
                    asArrayNotation: false,
                    route: operationMapping.Path,
                    //prefix: string.Empty,//parameter.Name,
                    //property: parameter,
                    //parentSchema: parameter.Schema,
                    
                    parameter: parameter,
                    parameterMapping: parameterMapping,
                    foundProperty: property
                //,parameterMapping: parameterMapping
                ));

        //results.Add(new TypeToOpenApiMappingResult(
        //            asArrayNotation: false,
        //            route: operationMapping.Path,
        //            prefix: string.Empty,//parameter.Name,
        //            property: parameter,
        //            parentSchema: parameter.Schema,
        //            foundProperty: null,
        //            parameter: parameter,
        //            parameterMapping: parameterMapping
        //        ));

        if (parameter.IsArray)
        {
            results.Add(TypeToOpenApiMappingResult.ForParameter(
                   asArrayNotation: true,
                   route: operationMapping.Path,
                   //prefix: string.Empty,//parameter.Name,
                   //property: parameter,
                   //parentSchema: parameter.Schema,

                   parameter: parameter,
                   parameterMapping: parameterMapping,
                   foundProperty: property
               //,parameterMapping: parameterMapping
               ));

            //results.Add(new TypeToOpenApiMappingResult(
            //        asArrayNotation: true,
            //        route: operationMapping.Path,
            //        prefix: "[n]",
            //        property: parameter,
            //        parentSchema: parameter.Schema,
            //        foundProperty: null,
            //        parameter: parameter,
            //        parameterMapping: parameterMapping
            //    ));
        }

        // loop through the "schemas" of the parameter, an array will be an array, the others may be sub properties.
        foreach (var prop in parameter.PropertyMappings)
        {
            results.AddRange(ProcessComplexObjectProperties(
                route: operationMapping.Path,
                httpMethod: operationMapping.HttpMethod,
                prefix: parameter.Name + (parameter.IsArray ? "[n]" : string.Empty),
                openApiPrefix: parameter.Name,
                functionMapping: functionMapping,
                parameter: parameter,
                functionItemMapping: parameterMapping,
                property: prop,
                parent: null
            ));
        }
        //ProcessSimpleOpenApiParameterMapping(
        //        operationMapping.Path,
        //        operationMapping.HttpMethod,
        //        string.Empty,
        //        parameter,
        //        parameterMapping // Pass the parameter index
        //    )
        //if (parameter.PropertyMappings?.Any() == true)
        //{
        //    // Process complex object properties for Body parameters

        //    results.Add(new TypeToOpenApiMappingResult
        //    {
        //        Route = operationMapping.Path,
        //        OpenApiObjHeirarchy = parameter.Name + ((parameter.IsArray) ? "[n]" : String.Empty),
        //        OpenApiPropertyName = parameter.Name + ((parameter.IsArray) ? "[n]" : String.Empty),
        //        PropertyMapping = null,
        //        DirectParentSchema = null,
        //        PropertySchema = parameter.Schema,
        //        IsArray = true,
        //        Parameter = parameter.Parameter,
        //        ParameterMapping = parameterMapping, // Set the parameter index
        //        ParentPropertyExtensions = parameter.ParentPropertyExtensions,
        //        ResultPropertyHeirarchy = ((parameter.IsArray) ? "[n]" : String.Empty)// parameter.Name + ((parameter.IsArray) ? "[n]" : String.Empty)
        //    }) ;

        //}
        //else
        //{
        //    // Process the other parameters directly
        //    results.AddRange(ProcessSimpleOpenApiParameterMapping(
        //        operationMapping.Path,
        //        operationMapping.HttpMethod,
        //        string.Empty,
        //        parameter,
        //        parameterMapping // Pass the parameter index
        //    ));
        //}

        return results;
    }

    private List<TypeToOpenApiMappingResult> CreateRequestBody(OpenApiOperationMapping operationMapping, FunctionMapping functionMapping)
    {
        var results = new List<TypeToOpenApiMappingResult>();

        var parameterMapping = FindFunctionForRequestBody(functionMapping);
        var isArray = operationMapping.RequestBody.IsArray;

        const string requestBodyString = "RequestBody";

        results.Add(TypeToOpenApiMappingResult.ForRequestBody(
            asArrayNotation: false,
            route: operationMapping.Path,
        //prefix: string.Empty,//parameter.Name,
        //property: parameter,
        //parentSchema: parameter.Schema,
            
            requestBody: operationMapping.RequestBody,
            parameterMapping: parameterMapping,
            foundProperty: null
        //,parameterMapping: parameterMapping
        ));

        if (isArray)
        {
            results.Add(TypeToOpenApiMappingResult.ForRequestBody(
            asArrayNotation: true,
            route: operationMapping.Path,
            //prefix: string.Empty,//parameter.Name,
            //property: parameter,
            //parentSchema: parameter.Schema,

            requestBody: operationMapping.RequestBody,
            parameterMapping: parameterMapping,
            foundProperty: null
        //,parameterMapping: parameterMapping
        ));

            //results.AddRange(ProcessComplexObjectProperties(
            //        operationMapping.Path,
            //        operationMapping.HttpMethod,
            //        requestBodyString + "[n]",
            //        requestBodyString + "[n]",
            //        functionMapping,
            //        operationMapping.RequestBody,
            //        parameterMapping, // No parameter index for the body directly
            //        operationMapping.RequestBody//,
            //       // null
            //    ));
        }

        //results.AddRange(ProcessComplexObjectProperties(
        //        operationMapping.Path,
        //        operationMapping.HttpMethod,
        //        requestBodyString + (isArray ? "[n]" : string.Empty),
        //        requestBodyString + (isArray ? "[n]" : string.Empty),
        //        functionMapping,
        //        operationMapping.RequestBody,
        //        parameterMapping, // No parameter index for the body directly
        //        operationMapping.RequestBody//,
        //                                    //null
        //    ));

        foreach (var prop in operationMapping.RequestBody.PropertyMappings)
        {
            results.AddRange(ProcessComplexObjectProperties(
                route: operationMapping.Path,
                httpMethod: operationMapping.HttpMethod,
                prefix: (isArray ? "[n]" : string.Empty),
                openApiPrefix: requestBodyString + (isArray ? "[n]" : string.Empty),
                functionMapping: functionMapping,
                parameter: operationMapping.RequestBody,
                functionItemMapping: parameterMapping,
                property: prop,
                parent: prop.DirectParentSchema
            ));
        }


        //foreach (var bodySchema in operationMapping.RequestBody.ContentSchemas.Take(1))
        //{
        //    isArray = bodySchema.Value.Items != null;


        //    if (isArray)
        //    {
        //        results.Add(new TypeToOpenApiMappingResult
        //        {
        //            Route = operationMapping.Path,
        //            OpenApiObjHeirarchy = "RequestBody[n]",
        //            OpenApiPropertyName = "RequestBody[n]",
        //            PropertyMapping = null,
        //            DirectParentSchema = operationMapping.RequestBody.ContentSchemas.Values.First(),
        //            PropertySchema = bodySchema.Value,
        //            ParentPropertyExtensions = operationMapping.RequestBody.ParentPropertyExtensions,
        //            ResultPropertyHeirarchy = "[n]",
        //            IsArray = true,
        //            RequestBody = operationMapping.RequestBody.RequestBody,
        //            //RequestBodyContentSchemas = operationMapping.RequestBody.RequestBody.Content.Select(x => x.Value.Schema).ToArray(),
        //            ParameterMapping = parameterMapping // No specific parameter, so set to -1
        //        });
        //    }

        //    results.Add(new TypeToOpenApiMappingResult
        //    {
        //        Route = operationMapping.Path,
        //        OpenApiObjHeirarchy = "RequestBody",
        //        OpenApiPropertyName = "RequestBody",
        //        PropertyMapping = null,
        //        DirectParentSchema = operationMapping.RequestBody.ContentSchemas.Values.First(),
        //        PropertySchema = bodySchema.Value,
        //        ParentPropertyExtensions = operationMapping.RequestBody.ParentPropertyExtensions,
        //        IsArray = false,
        //        RequestBody = operationMapping.RequestBody.RequestBody,
        //        //RequestBodyContentSchemas = operationMapping.RequestBody.RequestBody.Content.Select(x => x.Value.Schema).ToArray(),
        //        ParameterMapping = parameterMapping, // No specific parameter, so set to -1
        //        ResultPropertyHeirarchy = string.Empty
        //    });
        //}

        //if (!TypeHelper.IsSimpleType(parameterMapping.ParameterInfo.ParameterType))
        //{
        //    foreach (var property in operationMapping.RequestBody.PropertyMappings)
        //    {
        //        results.AddRange(ProcessComplexObjectProperties(
        //            operationMapping.Path,
        //            operationMapping.HttpMethod,
        //            "RequestBody" + (isArray ? "[n]" : null),
        //            "RequestBody" + (isArray ? "[n]" : string.Empty),
        //            functionMapping,
        //            operationMapping.RequestBody,
        //            parameterMapping, // No parameter index for the body directly
        //            property, 
        //            property.DirectParentSchema
        //        ));
        //    }
        //}

        return results;
    }

    private List<TypeToOpenApiMappingResult> CreateResponse(OpenApiOperationMapping operationMapping, FunctionMapping functionMapping, OpenApiResponseMapping response)
    {
        var results = new List<TypeToOpenApiMappingResult>();

        var isArray = response.Schema.Items != null;
        var responeFunctionItem = functionMapping.Return.First();

        results.Add(TypeToOpenApiMappingResult.ForResponse(
            asArrayNotation: false,
            route: operationMapping.Path,
            response: response,
            functionItemMapping: responeFunctionItem,
            foundProperty: null
        ));


        results.AddRange(ProcessComplexObjectProperties(
               route: operationMapping.Path,
               httpMethod: operationMapping.HttpMethod,
               prefix: string.Empty,
               openApiPrefix: OpenApiPrefix,
               functionMapping: functionMapping,
               parameter: response,
               functionItemMapping: responeFunctionItem,
               property: response,
               parent: null
           ));

        if (isArray)
        {
            results.Add(TypeToOpenApiMappingResult.ForResponse(
                asArrayNotation: true,
                route: operationMapping.Path,
                response: response,
                functionItemMapping: responeFunctionItem,
                foundProperty: null
            ));

            results.AddRange(ProcessComplexObjectProperties(
               route: operationMapping.Path,
               httpMethod: operationMapping.HttpMethod,
               prefix: "[n]",
               openApiPrefix: "Response[n]",
               functionMapping: functionMapping,
               parameter: response,
               functionItemMapping: responeFunctionItem,
               property: response,
               parent: null
           ));
        }


        //if (isArray)
        //{
        //    results.Add(new TypeToOpenApiMappingResult
        //    {
        //        Route = operationMapping.Path,
        //        OpenApiObjHeirarchy = "[n]",
        //        OpenApiPropertyName = "Response[n]",
        //        ResultPropertyHeirarchy = "[n]",
        //        PropertyMapping = null,
        //        DirectParentSchema = null,
        //        PropertySchema = null,
        //        ParentPropertyExtensions = operationMapping.Extensions,
        //        IsArray = true,
        //        RequestBody = null,
        //        ParameterMapping = null,
        //        ResponseMapping = response,
        //        ReturnMapping = functionMapping.Return.First(),
        //    });
        //}

        //results.Add(new TypeToOpenApiMappingResult
        //{
        //    Route = operationMapping.Path,
        //    OpenApiObjHeirarchy = null,
        //    OpenApiPropertyName = "Response",
        //    PropertyMapping = null,
        //    DirectParentSchema = null,
        //    PropertySchema = null,
        //    ParentPropertyExtensions = operationMapping.Extensions,
        //    IsArray = false,
        //    RequestBody = null,
        //    ParameterMapping = null,
        //    ResponseMapping = response,
        //    ReturnMapping = functionMapping.Return.First(),
        //    ResultPropertyHeirarchy = string.Empty
        //});

        //foreach (var property in response.PropertyMappings)
        //{
        //    results.AddRange(ProcessComplexObjectProperties(
        //       operationMapping.Path,
        //       operationMapping.HttpMethod,
        //       isArray ? "[n]" : string.Empty,
        //       "Response" + (isArray ? "[n]" : string.Empty),
        //       functionMapping,
        //       response,
        //       functionMapping.Return.First(),
        //       property,
        //       response.Schema
        //   ));
        //}

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

    public PropertyMapping? FindPropertyMapping(IGeneralMapping generalMapping, string parameterName)
    {
        parameterName = parameterName.Replace("[n]", "").Replace("[n.Key]", "").Replace("[n.Value]", "");

        return generalMapping.GetOpenApiPropertyNames()
            .FirstOrDefault(y => string.Equals(y.Item1, parameterName, StringComparison.InvariantCultureIgnoreCase))
            .Item2;
    }

    //public PropertyMapping? FindPropertyMapping(IGeneralMapping parameterMapping, string parameterName)
    //{
    //    parameterName = parameterName.Replace("[n]", "").Replace("[n.Key]", "").Replace("[n.Value]", "");

    //    return parameterMapping.GetOpenApiPropertyNames()
    //        .FirstOrDefault(y => string.Equals(y.Item1, parameterName, StringComparison.InvariantCultureIgnoreCase))
    //        .Item2;
    //}

    public ParameterMapping FindFunctionForRequestBody(FunctionMapping functionMapping)
    {
        return functionMapping.Parameters.OrderBy(x => x.ParameterInfo.Position).First(x => x.IsOpenApiRequestBody);
    }

    //private List<TypeToOpenApiMappingResult> ProcessSimpleOpenApiParameterMapping(
    //    string route,
    //    string httpMethod,
    //    string? prefix,
    //    IOpenApiParameterMapping parameter,
    //    IGeneralMapping parameterMapping) // Take the parameter index as an argument
    //{
    //    var results = new List<TypeToOpenApiMappingResult>();

    //    var newPrefix = string.IsNullOrWhiteSpace(prefix) ? property.Name : prefix + '.' + property.Name;

    //    //var newPrefix = string.IsNullOrWhiteSpace(prefix) ? parameter.Name : prefix + '.' + parameter.Name;
    //    // If it's an array, treat it as a list of items
    //    if (parameter.IsArray)
    //    {
    //        results.Add(new TypeToOpenApiMappingResult
    //        {
    //            Route = route,
    //            OpenApiObjHeirarchy = parameter.Name + "[n]",
    //            OpenApiPropertyName = parameter.Name + "[n]",
    //            PropertyMapping = null,
    //            DirectParentSchema = null,
    //            PropertySchema = parameter.Schema,
    //            IsArray = true,
    //            Parameter = parameter.Parameter,
    //            ParameterMapping = parameterMapping, // Set the parameter index
    //            ParentPropertyExtensions = parameter.ParentPropertyExtensions,
    //            ResultPropertyHeirarchy = parameter.Name + "[n]"
    //        });
    //    }

    //    results.Add(new TypeToOpenApiMappingResult
    //    {
    //        Route = route,
    //        OpenApiObjHeirarchy = parameter.Name,
    //        OpenApiPropertyName = parameter.Name,
    //        PropertyMapping = null,
    //        DirectParentSchema = null,
    //        PropertySchema = parameter.Schema,
    //        IsArray = false,
    //        Parameter = parameter.Parameter,
    //        ParameterMapping = parameterMapping, // Set the parameter index
    //        ParentPropertyExtensions = parameter.ParentPropertyExtensions,
    //        ResultPropertyHeirarchy = parameter.Name
    //    });

    //    results.Add(new TypeToOpenApiMappingResult(
    //        asArrayNotation: true,
    //        route: route,
    //        prefix: newPrefix,
    //        property: property,
    //        parentSchema: parent,
    //        foundProperty: foundProperty,
    //        parameter: parameter,
    //        parameterMapping: parameterMapping
    //    ));

    //    return results;
    //}

    //private List<TypeToOpenApiMappingResult> ProcessComplexObjectProperties(
    //    string route,
    //    string httpMethod,
    //    string? prefix,
    //    string? openApiPrefix,
    //    FunctionMapping functionMapping,
    //    IOpenApiParameterMapping parameter,
    //    IGeneralMapping parameterMapping) // Take the parameter index as an argument
    //{
    //    var results = new List<TypeToOpenApiMappingResult>();

    //    var newPrefix = string.IsNullOrWhiteSpace(prefix) ? parameter.Name : prefix + '.' + parameter.Name;
    //    var newOpenApiPrefix = string.IsNullOrWhiteSpace(openApiPrefix) ? parameter.Name : openApiPrefix + '.' + parameter.Name;
    //    var isArray = parameter.Schema.Items != null;

    //    var foundProperty = FindPropertyMapping(parameterMapping, newOpenApiPrefix);
    //    if (foundProperty != null)
    //    {
    //        //if (parameter.IsArray)
    //        //{
    //        //    Debug.Assert(!foundProperty.ResultPropertyName.EndsWith("[n]"));
    //        //    results.Add(new TypeToOpenApiMappingResult
    //        //    {
    //        //        Route = route,
    //        //        OpenApiObjHeirarchy = newPrefix + "[n]",
    //        //        OpenApiPropertyName = parameter.Name,
    //        //        DirectParentSchema = null,
    //        //        PropertySchema = parameter.Schema,
    //        //        PropertyMapping = foundProperty,
    //        //        ResultPropertyHeirarchy = (foundProperty.ResultPropertyName ?? string.Empty) + "[n]",
    //        //        IsArray = parameter.IsArray,
    //        //        Parameter = parameter?.Parameter,
    //        //        ParameterMapping = parameterMapping, // Set the parameter index
    //        //        ParentPropertyExtensions = parameter.ParentPropertyExtensions
    //        //    });
    //        //    Debug.Assert(results.Last().PropertyMapping != null || results.Last().Parameter != null);
    //        //}

    //        results.Add(new TypeToOpenApiMappingResult
    //        {
    //            Route = route,
    //            OpenApiObjHeirarchy = newPrefix,
    //            OpenApiPropertyName = parameter.Name,
    //            DirectParentSchema = null,
    //            PropertySchema = parameter.Schema,
    //            PropertyMapping = foundProperty,
    //            ResultPropertyHeirarchy = (foundProperty.ResultPropertyName ?? string.Empty),
    //            IsArray = false,
    //            Parameter = parameter?.Parameter,
    //            ParameterMapping = parameterMapping, // Set the parameter index
    //            ParentPropertyExtensions = parameter.ParentPropertyExtensions
    //        });
    //        Debug.Assert(results.Last().PropertyMapping != null || results.Last().Parameter != null);
    //    }

    //    foreach (var property in parameter.PropertyMappings)
    //    {
    //        // Flatten the properties recursively
    //        results.AddRange(ProcessComplexObjectProperties(
    //            route,
    //            httpMethod,
    //            newPrefix + (isArray ? "[n]" : null),
    //            newOpenApiPrefix + (isArray ? "[n]" : string.Empty),
    //            functionMapping,
    //            parameter,
    //            parameterMapping, // Pass the parameter index
    //            property,
    //            parameter.Schema
    //        ));
    //    }

    //    return results;
    //}

    private List<TypeToOpenApiMappingResult> ProcessComplexObjectProperties(
        string route,
        string httpMethod,
        string? prefix,
        string? openApiPrefix,
        FunctionMapping functionMapping,
        IOpenApiParameterMapping? parameter,
        IGeneralMapping functionItemMapping, // Take the parameter index as an argument
        IOpenApiPropertyMapping property,
        OpenApiSchema? parent
        )
    {
        var results = new List<TypeToOpenApiMappingResult>();
        foreach (var schema in property.Schemas)
        {
            // Prefix the property hierarchy with the parent (route + operation + property name)
            var newPrefix = string.IsNullOrWhiteSpace(prefix) ? property.Name : prefix + '.' + property.Name;

            // if the openApiPrefix is null, then it hasn't yet been called reciprocally.
            //  if it IS NOT null, then we are in the child of a parent.
            //  EXCEPT in the case of Named items, like RequestBody.
            // Then we are In the parent, not in the child, even though it has a "parent name"

            var newOpenApiPrefix = openApiPrefix switch
            {
                "RequestBody" => "RequestBody."+ property.Name,
                "" or null => property.Name,
                _ when (property.Name is not "" or null) => openApiPrefix + '.' + property.Name,
                _ => openApiPrefix,
            };
                //(openApiPrefix)
                
                //(string.IsNullOrWhiteSpace(openApiPrefix) || !string.IsNullOrWhiteSpace(property.Name)) 
                //? property.Name 
                //: openApiPrefix + '.' + property.Name;

            var foundProperty = FindPropertyMapping(functionItemMapping, newOpenApiPrefix);

            if (foundProperty != null)
            {
                if (property.IsArray)
                {
                    //    Debug.Assert(!foundProperty.ResultPropertyName.EndsWith("[n]"));
                    //    results.Add(new TypeToOpenApiMappingResult
                    //    {
                    //        Route = route,
                    //        OpenApiObjHeirarchy = newPrefix + "[n]",
                    //        OpenApiPropertyName = property.PropertyName,
                    //        DirectParentSchema = parent,
                    //        PropertySchema = property.PropertySchema,
                    //        PropertyMapping = foundProperty,
                    //        ResultPropertyHeirarchy = (foundProperty.ResultPropertyName ?? string.Empty) + "[n]",
                    //        IsArray = true,
                    //        Parameter = parameter?.Parameter,
                    //        ParameterMapping = parameterMapping, // Set the parameter index
                    //        RequestBody = null,
                    //        ParentPropertyExtensions = property.ParentPropertyExtensions,
                    //        PropertySecondarySchemas = property.SecondarySchemas
                    //    });
                    //    Debug.Assert(results.Last().PropertyMapping != null || results.Last().Parameter != null);

                results.Add(TypeToOpenApiMappingResult.ForProperty(
                   asArrayNotation: true,
                   route: route,
                   parentResultPrefix: prefix ?? string.Empty + "[n]",
                   parameter: parameter,
                   functionItemMapping: functionItemMapping,
                   foundProperty: foundProperty,
                   propertySchema: schema,
                   parentProeprtyExtensions: property.ParentPropertyExtensions,
                   directParentSchema: parent
               //prefix: string.Empty,//parameter.Name,
               //property: parameter,
               //parentSchema: parameter.Schema,

               //parameter: parameter,
               //foundProperty: property
               //,parameterMapping: parameterMapping
               ));

                    //results.Add(new TypeToOpenApiMappingResult(
                    //    asArrayNotation: true,
                    //    route: route,
                    //    prefix: newPrefix,
                    //    property: property,
                    //    parentSchema: parent,
                    //    foundProperty: foundProperty,
                    //    parameter: parameter,
                    //    parameterMapping: parameterMapping
                    //));
                }

                results.Add(TypeToOpenApiMappingResult.ForProperty(
                   asArrayNotation: false,
                   route: route,
                   parentResultPrefix: prefix ?? string.Empty,
                   parameter: parameter,
                   functionItemMapping: functionItemMapping,
                   foundProperty: foundProperty,
                   propertySchema: schema,
                   parentProeprtyExtensions: property.ParentPropertyExtensions,
                   directParentSchema: parent
               //prefix: string.Empty,//parameter.Name,
               //property: parameter,
               //parentSchema: parameter.Schema,

               //parameter: parameter,
               //foundProperty: property
               //,parameterMapping: parameterMapping
               ));

                //results.Add(new TypeToOpenApiMappingResult(
                //    asArrayNotation: false,
                //    route: route,
                //    prefix: newPrefix,
                //    property: property,
                //    parentSchema: parent,
                //    foundProperty: foundProperty,
                //    parameter: parameter,
                //    parameterMapping: parameterMapping
                //));
                //{
                //    Route = route,
                //    OpenApiObjHeirarchy = newPrefix,
                //    OpenApiPropertyName = property.PropertyName,
                //    DirectParentSchema = parent,
                //    PropertySchema = property.PropertySchema,
                //    PropertyMapping = foundProperty,
                //    ResultPropertyHeirarchy = (foundProperty.ResultPropertyName ?? string.Empty),
                //    IsArray = false,
                //    Parameter = parameter?.Parameter,
                //    ParameterMapping = parameterMapping, // Set the parameter index
                //    RequestBody = null,
                //    ParentPropertyExtensions = property.ParentPropertyExtensions,
                //    PropertySecondarySchemas = property.SecondarySchemas
                //}

                Debug.Assert(results.Last().PropertyMapping != null || results.Last().Parameter != null);

                // Process nested properties recursively
                foreach (var nestedProperty in property.PropertyMappings)
                {
                    results.AddRange(ProcessComplexObjectProperties(
                        route: route,
                        httpMethod: httpMethod,
                        prefix: newPrefix + (property.IsArray ? "[n]" : string.Empty),
                        openApiPrefix: newOpenApiPrefix + (property.IsArray ? "[n]" : string.Empty),
                        functionMapping: functionMapping,
                        parameter: parameter,
                        functionItemMapping: functionItemMapping, // Pass the parameter index
                        property: nestedProperty,//,
                        parent: schema
                        //null
                    ));
                }
            }
        }

        return results;
    }

    // New method for handling complex request body properties
    //private List<TypeToOpenApiMappingResult> ProcessComplexObjectProperties(
    //    string route,
    //    string httpMethod,
    //    string prefix,
    //    string? openApiPrefix,
    //    FunctionMapping functionMapping,
    //    IOpenApiParameterMapping requestBody,
    //    IGeneralMapping parameterMapping, // Take the parameter index as an argument
    //    OpenApiPropertyMapping property,
    //    OpenApiSchema? parent = null)
    //{
    //    var results = new List<TypeToOpenApiMappingResult>();

    //    // Prefix the property hierarchy with the parent (route + operation + property name)
    //    var newPrefix = string.IsNullOrWhiteSpace(prefix) ? property.PropertyName : prefix + '.' + property.PropertyName;
    //    var newOpenApiPrefix = string.IsNullOrWhiteSpace(openApiPrefix) ? property.PropertyName : openApiPrefix + '.' + property.PropertyName;

    //    var foundProperty = FindPropertyMapping(parameterMapping, newOpenApiPrefix);

    //    //if (property.IsArray)
    //    //{
    //    //    Debug.Assert(!foundProperty.ResultPropertyName.EndsWith("[n]"));
    //    //    results.Add(new TypeToOpenApiMappingResult
    //    //    {
    //    //        Route = route,
    //    //        OpenApiObjHeirarchy = newPrefix + "[n]",
    //    //        OpenApiPropertyName = property.PropertyName + "[n]",
    //    //        DirectParentSchema = parent,
    //    //        PropertySchema = property.PropertySchema,
    //    //        PropertyMapping = foundProperty,
    //    //        ResultPropertyHeirarchy = (foundProperty.ResultPropertyName ?? string.Empty) + "[n]",
    //    //        IsArray = true,
    //    //        Parameter = null, // No parameter for request body
    //    //        ParameterMapping = parameterMapping, // No parameter index for request body directly
    //    //        RequestBody = requestBody?.RequestBody,
    //    //        ParentPropertyExtensions = property.ParentPropertyExtensions,
    //    //        PropertySecondarySchemas = property.SecondarySchemas
    //    //    });
    //    //    Debug.Assert(results.Last().PropertyMapping != null || results.Last().Parameter != null);
    //    //}

    //    results.Add(new TypeToOpenApiMappingResult
    //    {
    //        Route = route,
    //        OpenApiObjHeirarchy = newPrefix,
    //        OpenApiPropertyName = property.PropertyName,
    //        DirectParentSchema = parent,
    //        PropertySchema = property.PropertySchema,
    //        PropertyMapping = foundProperty,
    //        ResultPropertyHeirarchy = (foundProperty.ResultPropertyName ?? string.Empty),
    //        IsArray = false,
    //        Parameter = null, // No parameter for request body
    //        ParameterMapping = parameterMapping, // No parameter index for request body directly
    //        RequestBody = requestBody?.RequestBody,
    //        ParentPropertyExtensions = property.ParentPropertyExtensions,
    //        PropertySecondarySchemas = property.SecondarySchemas
    //    });
    //    Debug.Assert(results.Last().PropertyMapping != null || results.Last().Parameter != null);

    //    // Process nested properties recursively
    //    foreach (var nestedProperty in property.NestedProperties)
    //    {
    //        results.AddRange(ProcessComplexObjectProperties(
    //            route,
    //            httpMethod,
    //            newPrefix + (property.IsArray ? "[n]" : string.Empty),
    //            newOpenApiPrefix + (property.IsArray ? "[n]" : string.Empty),
    //            functionMapping,
    //            requestBody,
    //            parameterMapping, // Pass the parameter index
    //            nestedProperty,
    //            property.PropertySchema
    //        ));
    //    }

    //    return results;
    //}

    //private List<TypeToOpenApiMappingResult> ProcessComplexObjectProperties(
    //    string route,
    //    string httpMethod,
    //    string prefix,
    //    string? openApiPrefix,
    //    FunctionMapping functionMapping,
    //    IGeneralMapping returnMapping,
    //    IOpenApiParameterMapping returnResponse,
    //    OpenApiPropertyMapping property,
    //    OpenApiSchema? parent = null)
    //{
    //    var results = new List<TypeToOpenApiMappingResult>();

    //    // Prefix the property hierarchy with the parent (route + operation + property name)
    //    var newPrefix = string.IsNullOrWhiteSpace(prefix) ? property.PropertyName : prefix + '.' + property.PropertyName;
    //    var newOpenApiPrefix = string.IsNullOrWhiteSpace(openApiPrefix) ? property.PropertyName : openApiPrefix + '.' + property.PropertyName;

    //    var foundProperty = FindPropertyMapping(returnMapping, newOpenApiPrefix);

    //    //if (property.IsArray)
    //    //{
    //    //    Debug.Assert(!foundProperty.ResultPropertyName.EndsWith("[n]"));
    //    //    results.Add(new TypeToOpenApiMappingResult
    //    //    {
    //    //        Route = route,
    //    //        OpenApiObjHeirarchy = newPrefix + "[n]",
    //    //        OpenApiPropertyName = property.PropertyName + "[n]",
    //    //        DirectParentSchema = parent,
    //    //        PropertySchema = property.PropertySchema,
    //    //        PropertyMapping = foundProperty,
    //    //        ResultPropertyHeirarchy = (foundProperty.ResultPropertyName ?? string.Empty) + "[n]",
    //    //        IsArray = true,
    //    //        Parameter = null,
    //    //        ParameterMapping = null,
    //    //        RequestBody = null,
    //    //        ParentPropertyExtensions = property.ParentPropertyExtensions,
    //    //        PropertySecondarySchemas = property.SecondarySchemas,
    //    //        ResponseMapping = returnResponse,
    //    //        ReturnMapping = returnMapping,
    //    //    });
    //    //    Debug.Assert(results.Last().PropertyMapping != null || results.Last().Parameter != null);
    //    //}

    //    results.Add(new TypeToOpenApiMappingResult
    //    {
    //        Route = route,
    //        OpenApiObjHeirarchy = newPrefix,
    //        OpenApiPropertyName = property.PropertyName,
    //        DirectParentSchema = parent,
    //        PropertySchema = property.PropertySchema,
    //        PropertyMapping = foundProperty,
    //        ResultPropertyHeirarchy = (foundProperty.ResultPropertyName ?? string.Empty),
    //        IsArray = false,
    //        Parameter = null,
    //        ParameterMapping = null,
    //        RequestBody = null,
    //        ParentPropertyExtensions = property.ParentPropertyExtensions,
    //        PropertySecondarySchemas = property.SecondarySchemas,
    //        ResponseMapping = returnResponse,
    //        ReturnMapping = returnMapping,
    //    });
    //    Debug.Assert(results.Last().PropertyMapping != null || results.Last().Parameter != null);

    //    // Process nested properties recursively
    //    foreach (var nestedProperty in property.NestedProperties)
    //    {
    //        results.AddRange(ProcessComplexObjectProperties(
    //            route,
    //            httpMethod,
    //            newPrefix + (property.IsArray ? "[n]" : string.Empty),
    //            newOpenApiPrefix + (property.IsArray ? "[n]" : string.Empty),
    //            functionMapping,
    //            returnMapping,
    //            returnResponse,
    //            nestedProperty,
    //            property?.PropertySchema
    //        ));
    //    }

    //    return results;
    //}
}
