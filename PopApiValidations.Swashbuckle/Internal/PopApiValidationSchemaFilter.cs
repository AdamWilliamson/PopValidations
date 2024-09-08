using ApiValidations.Execution;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using PopValidations.Execution.Description;
using PopValidations.Execution.Validations;
using PopValidations.Swashbuckle;
using PopValidations.Swashbuckle.Internal;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using ApiValidations.Execution;
using System.Collections;
using System.Diagnostics;
using Microsoft.OpenApi.Interfaces;
namespace PopApiValidations.Swashbuckle.Internal;

public class PopApiValidationSchemaFilter : IOperationFilter //ISchemaFilter
{
    private readonly IApiValidationRunnerFactory factory;
    private readonly OpenApiConfig config;
    private readonly ILogger<PopApiValidationSchemaFilter> logger;

    public PopApiValidationSchemaFilter(
        IApiValidationRunnerFactory factory,
        OpenApiConfig config,
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

        var runner = factory.GetRunner(context.MethodInfo.DeclaringType);

        if (runner == null) return;

        var results = runner.Describe();

        if (!results.Results.Any()) return;

        operation.Description += "New Description";
        operation.Summary += "New Summary";
        var firstParam = operation.Parameters.FirstOrDefault();
        if (firstParam != null)
        {
            firstParam.Description += "New param";
            firstParam.Schema.Minimum = 100;
        }

        var body = operation.RequestBody?.Content?.FirstOrDefault();
        if (body != null && !string.IsNullOrWhiteSpace(body.Value.Key)) 
        {
            body.Value.Value.Schema.Maximum = 2222;
        }

        var conParam = context.SchemaRepository.Schemas.FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(conParam.Key))
        {
            conParam.Value.MaxLength = 1999;
        }

        List<(Type, int, OpenApiSchema, string)> models = new();
        var parameters = context.MethodInfo.GetParameters();
        foreach (var p in operation.Parameters.Where(p => p.Schema != null))
        {
            var foundParam = parameters.First(x => x.Name == p.Name);
            models.Add((foundParam.ParameterType, foundParam.Position, p.Schema, p.Name));
        }

        var missingParameters = parameters.Where(x => !operation.Parameters.Any(p => p.Name == x.Name));

        foreach (var c in operation.RequestBody?.Content.Values ?? [])
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
            }
        }

        var extensionObject = new OpenApiObject();
        operation.Extensions.Add(config.CustomValidationAttribute, extensionObject);
        

        foreach (var model in models)
        {
            var extensions = PopValidationSchemaFilter.InitExtension(config, model.Item3);
            bool isEnumerable = model.Item1.GetInterface(typeof(IEnumerable).Name) == null? false: true;
            var desc = ApiValidations.Execution.PopApiValidations.Configuation.DescribeValidatingParam.Invoke(
                context.MethodInfo, Math.Max(model.Item2, 0), null
            );
            var enumerableDesc = ApiValidations.Execution.PopApiValidations.Configuation.DescribeValidatingParam.Invoke(
                context.MethodInfo, Math.Max(model.Item2, 0), -1
            );

            var endPointExtensionRules = new OpenApiArray();
            if (!extensionObject.ContainsKey(model.Item4))
            {
                extensionObject.Add(model.Item4, endPointExtensionRules);
            }
            else
            {
                endPointExtensionRules = extensionObject[model.Item4] as OpenApiArray;
            }

            var allresults = results.Results.Where(x => x.Property.StartsWith(desc));
            var allnonenumerableresults = allresults.Where(x => !x.Property.StartsWith(enumerableDesc)).ToList();
            var allenumerableresults = allresults.Where(x => x.Property.StartsWith(enumerableDesc)).ToList();

            RunParameterRule(
                model.Item1,
                desc,
                allnonenumerableresults,
                model.Item3,
                model.Item4,
                model.Item2,
                isEnumerable,
                enumerableDesc,
                context.SchemaRepository,
                extensionObject
            );
        }
    }

    public void RunParameterRule(
        Type paramType,
        string currentObjectGraph,
        List<DescriptionItemResult> resultObjectGraph,
        OpenApiSchema paramSchema,
        string paramName,
        int paramPosition,
        bool isEnumerable,
        string currentObjectGraphEnumerable,
        SchemaRepository schemaRepository,
        OpenApiObject endPointExtensions
    )
    {

        //var baseParamResult = resultObjectGraph.Where(x => x.Item1 == currentObjectGraph);
        //var baseParamResultEnumerable = resultObjectGraph.Where(x => x.Item1 == currentObjectGraphEnumerable);

        //FlattenOutcomes(config, results.Results.star, desc),

        var array = InitParamExtension(config, paramSchema);

        ConvertValiatorsToOpenApiDescriptions(
            config,
            paramSchema,
            paramName,
            array,
            //resultObjectGraph
            FlattenOutcomes(config, resultObjectGraph, currentObjectGraph)
        );

        //var dir = new DescriptionItemResult(currentObjectGraph);
        //dir.Outcomes.AddRange(resultObjectGraph.Select(x => x.Item2));

        RunObjectRules(
            config,
            currentObjectGraph,
            paramSchema,
            resultObjectGraph,
            schemaRepository,
            paramType,
            null,
            paramType,
            endPointExtensions,
            ValidationLevel.FullDetails
        );
    }

    public static void RunObjectRules(
        OpenApiConfig config,
        string currentApiObjectHeirarchy,
        OpenApiSchema model,
        List<DescriptionItemResult> resultObjectGraph,
        SchemaRepository schemaRepository,
        Type owner,
        string? ownedby,
        Type? childType,
        OpenApiObject endPointObjectextention,
        ValidationLevel? validationLevelOverride
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

#pragma warning disable CS8604 // Possible null reference argument.
                var customRulesArray = InitExtensionsAndArray(config, model, openApiPropName/*, outcomeSet, ownedby*/);
#pragma warning restore CS8604 // Possible null reference argument.

                // Execute processes for Field and Array descriptors for the property.
                //foreach (var outcomeSet in fieldOutcomes)
                //{
                    ConvertValiatorsToOpenApiDescriptions(
                        config,
                        model,
                        openApiPropName,
                        customRulesArray,
                        FlattenOutcomes(config, fieldOutcomes.ToList(), fieldName)
                    );

                //}

                if (arrayOutcomes?.Any() == true)
                {
#pragma warning disable CS8604 // Possible null reference argument.
                    var customRulesArrayForArrays = InitExtensionsAndArray(config, model, properArrayName/*, outcomeSet, ownedby*/);
#pragma warning restore CS8604 // Possible null reference argument.

                    //foreach (var outcomeSet in new[] { arrayOutcomes })
                    //{
                    //    if (outcomeSet is null)
                    //        continue;

                        ConvertValiatorsToOpenApiDescriptions(
                            config,
                            model,
                            fieldName + config.OrdinalIndicator,
                            customRulesArrayForArrays,
                            //arrayOutcomes.ToList()
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
                            config,
                            fieldName,
                            nonArrayChildObject,
                            resultObjectGraph,
                            schemaRepository,
                            owner,
                            newOwner + config.ChildIndicator + (model.Properties[openApiPropName].Reference?.Id ?? openApiPropName),
                            propType,
                            endPointObjectextention,
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
                            config,
                            fullObjectHeirarchyProperArrayName,
                            arraychildObject,
                            resultObjectGraph,
                            schemaRepository,
                            owner,
                            newOwner + config.ChildIndicator + (model.Properties[openApiPropName].Items.Reference?.Id ?? openApiPropName),
                            propType.GetGenericArguments()[0],
                            endPointObjectextention,
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
    private static List<(string, DescriptionOutcome)> FlattenOutcomes(OpenApiConfig config, List<DescriptionItemResult> descriptionItems, string param)
    {
        var endOutcomes = new List<(string, DescriptionOutcome)>();

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

    private static List<(string, DescriptionOutcome)> FlattenRecurse(OpenApiConfig config, string existing, DescriptionGroupResult group)
    {
        var endOutcomes = new List<(string, DescriptionOutcome)>();
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
        OpenApiConfig config,
        OpenApiSchema model,
        string openApiPropName,
        OpenApiArray customRulesArray,
        List<(string, DescriptionOutcome)> outcomeSet)
    {
        Debug.Assert(outcomeSet != null);
        var validationArray = new PopValidationArray(customRulesArray);

        foreach (var (owner, outcome) in outcomeSet)
        {
            if (!string.IsNullOrWhiteSpace(owner))
            {
                validationArray.SetLineHeader(owner + config.GroupResultIndicator);
            }
            else
            {
                validationArray.SetLineHeader(string.Empty);
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
                        converter.UpdateSchema(null, model, openApiPropName, outcome);
                    }

                    converter.UpdateAttribute(null, model, openApiPropName, outcome, validationArray);
                }
            }
        }
    }
}
