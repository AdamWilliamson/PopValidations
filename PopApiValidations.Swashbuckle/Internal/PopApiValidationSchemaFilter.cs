using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using PopValidations.Execution.Description;
using PopValidations.Execution.Validations;
using PopValidations.Swashbuckle;
using PopValidations.Swashbuckle.Internal;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Collections;
using System.Diagnostics;
using PopValidations.FieldDescriptors.Base;
using Microsoft.AspNetCore.Mvc;

namespace PopApiValidations.Swashbuckle.Internal;

public class OpenApiValidationParam
{
    public OpenApiValidationParam(
        OpenApiOperation operation,
        OpenApiParameter openApiParam,
        MethodInfo methodInfo,
        FunctionParameter parameterInfo,
        OpenApiSchema parameterSchema)
    {
        Operation = operation;
        OpenApiParam = openApiParam;
        MethodInfo = methodInfo;
        ParamType = parameterInfo.ParameterType;
        ParameterInfo = parameterInfo;
        ParameterSchema = parameterSchema;
        ParamName =
            //((openApiParam.In == ParameterLocation.Query && parameterInfo.Name != openApiParam.Name)
            //? string.Join('.', parameterInfo.ParamName, openApiParam.Name)
            //: 
            openApiParam.Name
            //) 
            ?? "RequestBody";
        ParamIndex = parameterInfo.Position;
    }

    public OpenApiValidationParam(
        OpenApiOperation operation,
        MethodInfo? methodInfo,
        FunctionParameter parameterInfo,
        OpenApiSchema parameterSchema,
        OpenApiRequestBody paramRequestBody)
    {
        Operation = operation;
        MethodInfo = methodInfo;
        ParamType = parameterInfo.ParameterType;
        ParameterSchema = parameterSchema;
        ParamRequestBody = paramRequestBody;
        ParamName = "RequestBody";
        ParamIndex = parameterInfo.Position;
    }

    public OpenApiOperation Operation { get; }
    public OpenApiParameter OpenApiParam { get; }
    public MethodInfo? MethodInfo { get; }
    public Type ParamType { get; }
    public FunctionParameter? ParameterInfo { get; }
    public OpenApiSchema ParameterSchema { get; }
    public OpenApiRequestBody? ParamRequestBody { get; }
    public string ParamName { get; }
    public int ParamIndex { get; }

    internal string MakeName(string desc)
    {
        if (ParameterInfo is not null && OpenApiParam.Name != ParameterInfo.Name)
        {
            return string.Join('.', desc, OpenApiParam.Name);
        }

        return desc;
    }
}

public class FunctionParameter(ParameterInfo parameterInfo)
{
    private string? _Name = null;

    public ParameterInfo ParameterInfo { get; } = parameterInfo;
    public string? ParamName => ParameterInfo.Name;

    public string Name 
    { 
        get 
        { 
            if (_Name != null)
            {
                return _Name;
            }

            var queryAttr = ParameterInfo.GetCustomAttribute<FromQueryAttribute>();
            var routeAttr = ParameterInfo.GetCustomAttribute<FromRouteAttribute>();
            var headerAttr = ParameterInfo.GetCustomAttribute<FromHeaderAttribute>();

            _Name = queryAttr?.Name ?? routeAttr?.Name ?? headerAttr?.Name ?? ParameterInfo.Name;

            return _Name!;
        } 
    }

    public Type ParameterType { get => ParameterInfo.ParameterType; }
    public int Position { get => ParameterInfo.Position; }
    public bool Matched { get; set; } = false;

    public bool Matches(OpenApiParameter queryParam)
    {
        if (queryParam.Name == Name)
        {
            return true;
        }

        Type? type = ParameterInfo.ParameterType;

        var properties = queryParam.Name.Split('.');

        foreach (var prop in properties)
        {
            type = type?.GetProperty(prop)?.PropertyType;
        }
        
        return type != null;
    }
}

public class PopApiValidationSchemaFilter : IOperationFilter //ISchemaFilter
{
    private readonly IApiValidationRunnerFactory factory;
    private readonly PopApiOpenApiConfig config;
    private readonly ILogger<PopApiValidationSchemaFilter> logger;

    public PopApiValidationSchemaFilter(
        IApiValidationRunnerFactory factory,
        PopApiOpenApiConfig config,
        ILogger<PopApiValidationSchemaFilter> logger
    )
    {
        this.factory = factory;
        this.config = config;
        this.logger = logger;
    }

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.MethodInfo.DeclaringType is null) return;
        if (config.ValidateEndpoint?.Invoke(context.MethodInfo) == false) 
            return;

        var runner = factory.GetRunner(context.MethodInfo.DeclaringType);

        if (runner == null) return;

        var results = runner.Describe();

        if (!results.Results.Any()) return;

        List<OpenApiValidationParam> newModels = new List<OpenApiValidationParam>();

        List<(Type, int, OpenApiSchema, string)> models = new();
        var parameters = context.MethodInfo.GetParameters().Select(x => new FunctionParameter(x));


        foreach (var p in operation.Parameters.Where(p => p.Schema != null && p.In != ParameterLocation.Query))  // Cannot handle Query Parameters Currently.
        {
            var foundParam = parameters.First(x => string.Equals(x.Name, p.Name, StringComparison.InvariantCultureIgnoreCase));
            models.Add((foundParam.ParameterType, foundParam.Position, p.Schema, p.Name));

            foundParam.Matched = true;
            newModels.Add(new OpenApiValidationParam(
                operation,
                p,
                context.MethodInfo,
                foundParam,
                p.Schema
            ));
        }

        // Query Version
        var unusedParams = parameters
            .Where(x => !x.Matched)
            .ToList();
        var queryOpenApiParameters = operation.Parameters.Where(p => p.Schema != null && p.In == ParameterLocation.Query);

        foreach (var queryParam in queryOpenApiParameters)
        {
            var foundParam = unusedParams.FirstOrDefault(x => x.Name == queryParam.Name)
                ?? unusedParams.FirstOrDefault(x => x.Matches(queryParam));
                //.FirstOrDefault(x => x.ParameterType.GetProperties()
                //    .Any(x => string.Equals(x.Name, GetFirstFromSplit(queryParams.Name, '.'), StringComparison.InvariantCultureIgnoreCase))
                //);

            if (foundParam == null)
            {
                // There is a query parameter that wasn't found... 
                return;
            }

            models.Add((foundParam.ParameterType, foundParam.Position, queryParam.Schema, queryParam.Name));

            newModels.Add(new OpenApiValidationParam(
                operation,
                queryParam,
                context.MethodInfo,
                foundParam,
                queryParam.Schema
            ));
        }

        var missingParameters = parameters.Where(x => !operation.Parameters.Any(p => p.Name == x.Name));

        foreach (var c in operation.RequestBody?.Content.Values.Take(1) ?? [])
        {
            var foundcontentparam =
                c.Schema.Reference == null
                ? missingParameters.FirstOrDefault(x => c.Schema.Properties.All(p => x.ParameterType.GetProperties().Any(g => string.Equals(g.Name,p.Key,StringComparison.InvariantCultureIgnoreCase) )))
                : missingParameters.FirstOrDefault(x => x.ParameterType.Name == c.Schema.Reference.Id);
            
            if (foundcontentparam != null)
            {
                var foundParam = parameters.First(x => x.Name == foundcontentparam.Name);
                models.Add((foundParam.ParameterType, foundParam.Position, 
                    c.Schema.Reference != null? context.SchemaRepository.Schemas[c.Schema.Reference.Id] : c.Schema,
                    foundcontentparam.Name));

                newModels.Add(new OpenApiValidationParam(
                    operation,
                    context.MethodInfo,
                    foundParam,
                    c.Schema.Reference != null ? context.SchemaRepository.Schemas[c.Schema.Reference.Id] : c.Schema,
                    operation.RequestBody!
                ));
            }
        }

        //foreach (var model in models)
        //{
        //    var extensions = PopValidationSchemaFilter.InitExtension(config, model.Item3);
        //    bool isEnumerable = model.Item1.GetInterface(typeof(IEnumerable).Name) == null? false: true;
        //    var desc = ApiValidations.Execution.PopApiValidations.Configuation.DescribeValidatingParam.Invoke(
        //        context.MethodInfo, Math.Max(model.Item2, 0), null
        //    );
        //    var enumerableDesc = ApiValidations.Execution.PopApiValidations.Configuation.DescribeValidatingParam.Invoke(
        //        context.MethodInfo, Math.Max(model.Item2, 0), -1
        //    );

        //    var endPointExtensionRules = new OpenApiArray();
        //    if (!extensionObject.ContainsKey(model.Item4))
        //    {
        //        extensionObject.Add(model.Item4, endPointExtensionRules);
        //    }
        //    else
        //    {
        //        endPointExtensionRules = extensionObject[model.Item4] as OpenApiArray;
        //    }

        //    var allresults = results.Results.Where(x => x.Property.StartsWith(desc));
        //    var allnonenumerableresults = allresults.Where(x => !x.Property.StartsWith(enumerableDesc)).ToList();
        //    var allenumerableresults = allresults.Where(x => x.Property.StartsWith(enumerableDesc)).ToList();

        //    RunParameterRule(

        //        model.Item1,
        //        desc,
        //        allnonenumerableresults,
        //        model.Item3,
        //        model.Item4,
        //        model.Item2,
        //        isEnumerable,
        //        enumerableDesc,
        //        context.SchemaRepository,
        //        extensionObject
        //    );
        //}

        foreach (var model in newModels)
        {
            //var extensions = PopValidationSchemaFilter.InitExtension(config, model.ParameterSchema);
            bool isEnumerable = model.ParamType.GetInterface(typeof(IEnumerable).Name) == null ? false : true;
            var desc = ApiValidations.Execution.PopApiValidations.Configuation.DescribeValidatingParam.Invoke(
                context.MethodInfo, Math.Max(model.ParamIndex, 0), null
            );
            var enumerableDesc = ApiValidations.Execution.PopApiValidations.Configuation.DescribeValidatingParam.Invoke(
                context.MethodInfo, Math.Max(model.ParamIndex, 0), -1
            );


            //var endPointExtensionRules = new OpenApiArray();
            //if (!extensionObject.ContainsKey(model.ParamName))
            //{
            //    extensionObject.Add(model.ParamName, endPointExtensionRules);
            //}
            //else
            //{
            //    endPointExtensionRules = extensionObject[model.ParamName] as OpenApiArray;
            //}

            var allresults = results.Results.Where(x => x.Property.StartsWith(desc));
            var allnonenumerableresults = allresults.Where(x => !x.Property.StartsWith(enumerableDesc)).ToList();
            var allenumerableresults = allresults.Where(x => x.Property.StartsWith(enumerableDesc)).ToList();

            RunParameterRule(
                model,
                model.MakeName(desc),
                allnonenumerableresults,
                context.SchemaRepository,
                ValidationLevel.FullDetails
            );
        }
    }

    //private string GetFirstFromSplit(string input, char delimiter)
    //{
    //    var i = input.IndexOf(delimiter);

    //    return i == -1 ? input : input.Substring(0, i);
    //}

    public void RunParameterRule(
        OpenApiValidationParam paramInfo,
        string currentObjectGraph,
        List<DescriptionItemResult> resultObjectGraph,
        SchemaRepository schemaRepository,
        //OpenApiObject endPointExtensions,
        ValidationLevel validationLevel
    )
    {
        var flattenedOutcomes = FlattenOutcomes(config, resultObjectGraph, currentObjectGraph);

        if (flattenedOutcomes.Any(x => x.GroupTitle == string.Empty))
        {
            //var functionExtensionsArray = InitExtensionsAndArray(config, paramInfo.Operation, paramInfo.ParamName);
            PopValidationArray? validationArray = null;// = new PopValidationArray(functionExtensionsArray);
            PopValidationArray? requestBodyValidationArray = null;

            foreach (var (owner, outcome) in flattenedOutcomes.Where(x => x.Item1 == string.Empty))
            {
                foreach (var converter in config.PopApiConverters)
                {
                    // Check if it supports the descriptor outcome
                    if (converter.Supports(outcome))
                    {
                        if (validationArray is null && paramInfo.ParamRequestBody is null)
                        {
                            var functionExtensionsArray = InitExtensionsAndArray(config, paramInfo.Operation, paramInfo.OpenApiParam.Name);
                            validationArray = new PopValidationArray(functionExtensionsArray);
                        }

                        if (paramInfo.ParamRequestBody is not null && requestBodyValidationArray is null)
                        {
                            var requestBodyExtensionsArray = InitExtensionsAndArray(config, paramInfo.Operation, paramInfo.ParamName);
                            requestBodyValidationArray = new PopValidationArray(requestBodyExtensionsArray);
                        }

                        //Incomplete
                        if (string.IsNullOrWhiteSpace(owner))
                        {
                            if (paramInfo.OpenApiParam != null)
                                converter.UpdateParamSchema(paramInfo.Operation, paramInfo.OpenApiParam, paramInfo.OpenApiParam?.Name, outcome);

                            if (paramInfo.ParamRequestBody != null)
                                converter.UpdateRequestBodySchema(paramInfo.ParamRequestBody, paramInfo.ParameterSchema, paramInfo.ParamName, outcome);
                        }

                        if (paramInfo.ParamRequestBody != null && requestBodyValidationArray is not null)
                            converter.UpdateAttribute(paramInfo.Operation, paramInfo.ParameterSchema, paramInfo.ParamName, outcome, requestBodyValidationArray);
                        else if (!string.IsNullOrEmpty(paramInfo.OpenApiParam?.Name))
                            converter.UpdateAttribute(paramInfo.Operation, paramInfo.ParameterSchema, paramInfo.OpenApiParam?.Name, outcome, validationArray);
                    }
                }
            }

            //ConvertValiatorsToOpenApiDescriptionsForParam(
            //    config,
            //    paramInfo,
            //    //paramInfo.Operation,
            //    //paramInfo.ParameterSchema,
            //    //paramInfo.ParamName,
            //    validationLevel,
            //    flattenedOutcomes
            //);

        }
        flattenedOutcomes = flattenedOutcomes.Where(x => x.GroupTitle != string.Empty).ToList();

        ConvertValiatorsToOpenApiDescriptions(
            config,
            null,
            paramInfo.ParameterSchema,
            paramInfo.ParamName,
            validationLevel,
            flattenedOutcomes
        );

        RunObjectRules(
            paramInfo,
            config,
            currentObjectGraph,
            paramInfo.ParameterSchema,
            resultObjectGraph,
            schemaRepository,
            paramInfo.ParamType,
            null,
            paramInfo.ParamType,
            //endPointExtensions,
            ValidationLevel.FullDetails
        );
    }

    public static void RunObjectRules(
        OpenApiValidationParam paramInfo, // Does not change
        PopApiOpenApiConfig config,         // Does not change
        string currentApiObjectHeirarchy, // changes every call
        OpenApiSchema model,                // changes every call
        List<DescriptionItemResult> resultObjectGraph,
        SchemaRepository schemaRepository, // Does not change.
        Type owner,             // Does not change.
        string? ownedby,        // changes every call
        Type? childType,        // Changes every call
        //OpenApiObject endPointObjectextention, // Does not change.
        ValidationLevel? validationLevelOverride  // Is Updated in call and changes every call
    )
    {
        // Model registers no api input properties, so we dont need to process this child.
        if (model.Properties?.Any() != true)
        {
            return;
        }

        // Loop through the OpenApi model's proeprties
        foreach (var openApiPropName in model.Properties.Keys)
        {
            // Generate a field Name Approximation, from the previous objects's accessor (parent), and the child property key.
            var fieldName = !string.IsNullOrWhiteSpace(currentApiObjectHeirarchy) ? currentApiObjectHeirarchy + config.ChildIndicator + openApiPropName : openApiPropName;

            //Determine if the Property should be shown.
            ValidationLevel PropertyValidationLevel = ValidationLevel.None;
            var propType = config.GetPropertyType.Invoke(config, childType ?? owner, fieldName);
            if (propType != null)
            {
                PropertyValidationLevel = (config.TypeValidationLevel?.Invoke(propType!) ?? ValidationLevel.FullDetails);
            }

            PropertyValidationLevel = CalculateOverride(validationLevelOverride, PropertyValidationLevel);

            if (PropertyValidationLevel == ValidationLevel.None) { return; }

            validationLevelOverride = PropertyValidationLevel;


            var properArrayName = openApiPropName + config.OrdinalIndicator;
            var fullObjectHeirarchyProperArrayName = fieldName + config.OrdinalIndicator;

            // If the field descriptor has something that matches the current field
            if (
                resultObjectGraph.Any(
                    x => x.Property.StartsWith(fieldName, StringComparison.OrdinalIgnoreCase)
                )
            )
            {
                // Find the field descriptor exactly.
                var fieldOutcomes = resultObjectGraph.Where(
                    x => config.ObjectPropertyIsJsonProperty(x.Property, fieldName)
                );
                // Find the field descriptor if its an array
                var arrayOutcomes = resultObjectGraph.Where(
                    x => config.ObjectPropertyIsDescriptorArray(x.Property, fieldName)
                );

                // Execute processes for Field and Array descriptors for the property.
                //foreach (var outcomeSet in fieldOutcomes)
                //{
                    ConvertValiatorsToOpenApiDescriptions(
                        config,
                        model,
                        model.Properties[openApiPropName],
                        openApiPropName,
                        validationLevelOverride.Value,
                        FlattenOutcomes(config, fieldOutcomes.ToList(), fieldName)
                    );

                //}

                if (arrayOutcomes?.Any() == true)
                {
                    //foreach (var outcomeSet in new[] { arrayOutcomes })
                    //{
                    //    if (outcomeSet is null)
                    //        continue;
                        ConvertValiatorsToOpenApiDescriptions(
                            config,
                            model,
                            model.Properties[openApiPropName],
                            fieldName + config.OrdinalIndicator,
                            //arrayOutcomes.ToList()
                            validationLevelOverride.Value,
                            FlattenOutcomes(config, arrayOutcomes.ToList(), fieldName)
                        );

                    //}
                }

                //if (PropertyValidationLevel.HasFlag(ValidationLevel.ValidationAttributeInBase))
                {
                    var newOwner = ownedby ?? "Owned By " + owner.Name;

                    var nonArrayChildObject =
                        (model.Properties[openApiPropName].Reference != null
                        && schemaRepository.Schemas.ContainsKey(
                            model.Properties[openApiPropName].Reference.Id
                        )) ? schemaRepository.Schemas[
                            model.Properties[openApiPropName].Reference.Id
                        ]
                        : model.Properties[openApiPropName];
                    if (
                        nonArrayChildObject != null
                        //model.Properties[openApiPropName].Reference != null
                        //&& schemaRepository.Schemas.ContainsKey(
                        //    model.Properties[openApiPropName].Reference.Id
                        //)
                    )
                    {
                        //var nonArrayChildObject = schemaRepository.Schemas[
                        //    model.Properties[openApiPropName].Reference.Id
                        //];

                        RunObjectRules(
                            paramInfo,
                            config,
                            fieldName,
                            nonArrayChildObject,
                            resultObjectGraph,
                            schemaRepository,
                            owner,
                            newOwner + config.ChildIndicator + (model.Properties[openApiPropName].Reference?.Id ?? openApiPropName),
                            propType,
                            //endPointObjectextention,
                            validationLevelOverride
                        );
                    }

                    var arraychildObject =
                        (
                        model.Properties[openApiPropName].Items?.Reference != null
                        && schemaRepository.Schemas.ContainsKey(
                            model.Properties[openApiPropName].Items.Reference.Id
                        )
                    ) ? schemaRepository.Schemas[
                            model.Properties[openApiPropName].Items.Reference.Id
                        ]
                    : model.Properties[openApiPropName].Items;

                    if (
                        //model.Properties[openApiPropName].Items?.Reference != null
                        //&& schemaRepository.Schemas.ContainsKey(
                        //    model.Properties[openApiPropName].Items.Reference.Id
                        //)
                        arraychildObject != null
                    )
                    {
                        //var arraychildObject = schemaRepository.Schemas[
                        //    model.Properties[openApiPropName].Items.Reference.Id
                        //];

                        RunObjectRules(
                            paramInfo,
                            config,
                            fullObjectHeirarchyProperArrayName,
                            arraychildObject,
                            resultObjectGraph,
                            schemaRepository,
                            owner,
                            newOwner + config.ChildIndicator + (model.Properties[openApiPropName].Items.Reference?.Id ?? openApiPropName),
                            propType.GetGenericArguments()[0],
                            //endPointObjectextention,
                            validationLevelOverride
                        );

                        //RunRules(
                        //    config,
                        //    fieldName + config.OrdinalIndicator,
                        //    childObject,
                        //    fieldDescriptions,
                        //    //context,
                        //    schemaRepository,
                        //    owner,
                        //    newOwner + config.ChildIndicator + model.Properties[openApiPropName].Items.Reference.Id,
                        //    propType,
                        //    endPointObjectextention,
                        //    validationLevelOverride
                        //);
                    }
                }
            }
        }
    }

    #region requestBody
    public static OpenApiArray InitExtensionsAndArrayForRequestBody(
        OpenApiConfig config,
        OpenApiOperation oeprationSchema,
        string key
    )
    {
        return InitExtensionForRequestBody(config, oeprationSchema);

        //return InitArray(config, modelValidations, key);
        //OpenApiArray array;
        //if (modelValidations.ContainsKey(key))
        //{
        //    array = modelValidations[key] as OpenApiArray ?? new OpenApiArray();
        //}
        //else
        //{
        //    array = new OpenApiArray();
        //}

        //modelValidations[key] = array;

        //return array;
    }

    public static OpenApiArray InitExtensionForRequestBody(OpenApiConfig config, OpenApiOperation oeprationSchema)
    {
        var modelValidations = new OpenApiArray();
        if (oeprationSchema.RequestBody.Extensions.ContainsKey(config.CustomValidationAttribute))
        {
            if (oeprationSchema.RequestBody.Extensions[config.CustomValidationAttribute] is OpenApiArray converted)
            {
                modelValidations = converted;
            }
            else
            {
                oeprationSchema.RequestBody.Extensions[config.CustomValidationAttribute] = modelValidations;
            }
        }
        else
        {
            oeprationSchema.RequestBody.Extensions.Add(config.CustomValidationAttribute, modelValidations);
        }

        return modelValidations;
    }

    #endregion




    public static OpenApiArray InitExtensionsAndArray(
        OpenApiConfig config,
        OpenApiOperation oeprationSchema,
        string key
    )
    {
        var modelValidations = InitExtension(config, oeprationSchema);

        return InitArray(config, modelValidations, key);
    }

    public static OpenApiObject InitExtension(OpenApiConfig config, OpenApiOperation oeprationSchema)
    {
        var modelValidations = new OpenApiObject();
        if (oeprationSchema.Extensions.ContainsKey(config.CustomValidationAttribute))
        {
            if (oeprationSchema.Extensions[config.CustomValidationAttribute] is OpenApiObject converted)
            {
                modelValidations = converted;
            }
            else
            {
                oeprationSchema.Extensions[config.CustomValidationAttribute] = modelValidations;
            }
        }
        else
        {
            oeprationSchema.Extensions.Add(config.CustomValidationAttribute, modelValidations);
        }

        return modelValidations;
    }


    private static OpenApiArray InitArray(
        OpenApiConfig config,
        OpenApiObject owningObject,
        string key
    )
    {
        OpenApiArray array;
        if (owningObject.ContainsKey(key))
        {
            array = owningObject[key] as OpenApiArray ?? new OpenApiArray();
        }
        else
        {
            array = new OpenApiArray();
        }

        owningObject[key] = array;

        return array;
    }

    public static OpenApiArray InitExtensionsAndArray(
        OpenApiConfig config,
        OpenApiSchema modelSchema,
        string key
    )
    {
        var modelValidations = InitExtension(config, modelSchema);

        return InitArray(config, modelValidations, key);
    }

    private static ValidationLevel CalculateOverride(ValidationLevel? validationLevelOverride, ValidationLevel objLevel)
    {
        if (validationLevelOverride == null) return objLevel;
        if (validationLevelOverride > objLevel) return objLevel;

        return validationLevelOverride.Value;
    }

    //private static Type? GetPropertyType(OpenApiConfig config, Type? src, string propName)
    //{
    //    if (src == null) throw new ArgumentException("Value cannot be null.", "src");
    //    if (string.IsNullOrWhiteSpace(propName)) return src;

    //    var realProp = propName.Split(new char[] { '.' }).Last();

    //    if (realProp.Contains(config.OrdinalIndicator))
    //    {
    //        var result = config.GetPropertyFromType(src, realProp.Replace(config.OrdinalIndicator, ""));
    //        if (result is not null && (config.IsGenericList?.Invoke(result.PropertyType) ?? false))
    //        {
    //            return result.PropertyType.GetGenericArguments().FirstOrDefault();
    //        }
    //        return null;
    //    }
    //    else
    //    {
    //        var result = config.GetPropertyFromType(src, realProp);

    //        return result?.PropertyType;
    //    }
    //}

    public static OpenApiArray InitParamExtension(OpenApiConfig config, OpenApiSchema modelSchema)
    {
        var modelValidations = new OpenApiArray();
        if (modelSchema.Extensions.ContainsKey(config.CustomValidationAttribute))
        {
            if (modelSchema.Extensions[config.CustomValidationAttribute] is OpenApiArray converted)
            {
                modelValidations = converted;
            }
            else
            {
                modelSchema.Extensions[config.CustomValidationAttribute] = modelValidations;
            }
        }
        else
        {
            modelSchema.Extensions.Add(config.CustomValidationAttribute, modelValidations);
        }

        return modelValidations;
    }

    public static OpenApiObject InitExtension(OpenApiConfig config, OpenApiSchema modelSchema)
    {
        var modelValidations = new OpenApiObject();
        if (modelSchema.Extensions.ContainsKey(config.CustomValidationAttribute))
        {
            if (modelSchema.Extensions[config.CustomValidationAttribute] is OpenApiObject converted)
            {
                modelValidations = converted;
            }
            else
            {
                modelSchema.Extensions[config.CustomValidationAttribute] = modelValidations;
            }
        }
        else
        {
            modelSchema.Extensions.Add(config.CustomValidationAttribute, modelValidations);
        }

        return modelValidations;
    }


    #region Flatten the Descriptions where groups exist, into a flat list.
    private static List<(string GroupTitle, DescriptionOutcome Outcome)> FlattenOutcomes(OpenApiConfig config, List<DescriptionItemResult> descriptionItems, string param)
    {
        var endOutcomes = new List<(string GroupTitle, DescriptionOutcome Outcome)>();

        if (descriptionItems?.Any() != true) return endOutcomes;

        foreach (var descriptionItem in descriptionItems.Where(x => x.Property.Equals(param, StringComparison.OrdinalIgnoreCase)))
        {
            if (descriptionItem.Outcomes?.Any() == true)
            {
                foreach (var outcome in descriptionItem.Outcomes)
                {
                    if (outcome == null) continue;

                    endOutcomes.Add((string.Empty, outcome!));
                }
            }

            if (descriptionItem.ValidationGroups?.Any() == true)
            {
                foreach (var group in descriptionItem.ValidationGroups)
                {
                    endOutcomes.AddRange(FlattenRecurse(config, string.Empty, group));
                }
            }
        }

        return endOutcomes;
    }

    private static List<(string GroupTitle, DescriptionOutcome Outcome)> FlattenRecurse(OpenApiConfig config, string existing, DescriptionGroupResult group)
    {
        var endOutcomes = new List<(string GroupTitle, DescriptionOutcome Outcome)>();
        var additive = string.IsNullOrWhiteSpace(existing) ? group.Description : config.MultiGroupIndicator + group.Description;

        if (group.Outcomes?.Any() == true)
        {
            foreach (var outcome in group.Outcomes)
            {
                if (outcome == null) continue;

                endOutcomes.Add(new(existing + additive, outcome!));
            }
        }

        if (group.Children?.Any() == true)
        {
            foreach (var child in group.Children)
            {
                endOutcomes.AddRange(FlattenRecurse(config, existing + additive, child));
            }
        }

        return endOutcomes;
    }
    #endregion 

    public static void ConvertValiatorsToOpenApiDescriptions(
        PopApiOpenApiConfig config,
        OpenApiSchema parentModel,
        OpenApiSchema propertyModel,
        string openApiPropName,
        ValidationLevel validationLevel,
        List<(string, DescriptionOutcome)> outcomeSet)
    {
        Debug.Assert(outcomeSet != null);
        if (outcomeSet.Count == 0) return;

        PopValidationArray? validationArray = null;

        if (validationLevel.HasFlag(ValidationLevel.ValidationAttribute))
        {
#pragma warning disable CS8604 // Possible null reference argument.
            var customRulesArray = InitExtensionsAndArray(config, parentModel, openApiPropName/*, outcomeSet, ownedby*/);
#pragma warning restore CS8604 // Possible null reference argument.
            validationArray = new PopValidationArray(customRulesArray);
        }

        foreach (var (owner, outcome) in outcomeSet)
        {
            if (validationLevel.HasFlag(ValidationLevel.ValidationAttribute))
            {
                if (!string.IsNullOrWhiteSpace(owner))
                {
                    validationArray!.SetLineHeader(owner + config.GroupResultIndicator);
                }
                else
                {
                    validationArray!.SetLineHeader(string.Empty);
                }
            }

            // for each converter registered in the config
            foreach (var converter in config.Converters)
            {
                // Check if it supports the descriptor outcome
                if (converter.Supports(outcome))
                {
                    //Incomplete
                    if (string.IsNullOrWhiteSpace(owner))
                    {
                        converter.UpdateSchema(parentModel, propertyModel, openApiPropName, outcome);
                    }

                    if (validationLevel.HasFlag(ValidationLevel.ValidationAttribute))
                    {
                        converter.UpdateAttribute(parentModel, propertyModel, openApiPropName, outcome, validationArray!);
                    }
                }
            }
        }
    }

    public static void ConvertValiatorsToOpenApiDescriptionsForParam(
        PopApiOpenApiConfig config,
        OpenApiValidationParam paramInfo,
        //OpenApiOperation parentModel,
        //OpenApiSchema propertyModel,
        //string openApiPropName,
        ValidationLevel validationLevel,
        List<(string, DescriptionOutcome)> outcomeSet)
    {
        Debug.Assert(outcomeSet != null);
        if (outcomeSet.Count == 0) return;

        PopValidationArray? validationArray = null;

        if (validationLevel.HasFlag(ValidationLevel.ValidationAttribute))
        {
#pragma warning disable CS8604 // Possible null reference argument.
            var customRulesArray = InitExtensionsAndArray(config, paramInfo.Operation, paramInfo.ParamName/*, outcomeSet, ownedby*/);
#pragma warning restore CS8604 // Possible null reference argument.
            validationArray = new PopValidationArray(customRulesArray);
        }

        foreach (var (owner, outcome) in outcomeSet)
        {
            if (validationLevel.HasFlag(ValidationLevel.ValidationAttribute))
            {
                if (!string.IsNullOrWhiteSpace(owner))
                {
                    validationArray!.SetLineHeader(owner + config.GroupResultIndicator);
                }
                else
                {
                    validationArray!.SetLineHeader(string.Empty);
                }
            }

            // for each converter registered in the config
            foreach (var converter in config.PopApiConverters)
            {
                // Check if it supports the descriptor outcome
                if (converter.Supports(outcome))
                {
                    //Incomplete
                    if (string.IsNullOrWhiteSpace(owner))
                    {
                        converter.UpdateParamSchema(paramInfo.Operation, paramInfo.OpenApiParam, paramInfo.ParamName, outcome);
                    }

                    if (validationLevel.HasFlag(ValidationLevel.ValidationAttribute))
                    {
                        converter.UpdateAttribute(paramInfo.Operation, paramInfo.ParameterSchema, paramInfo.ParamName, outcome, validationArray!);
                    }
                }
            }
        }
    }
}
