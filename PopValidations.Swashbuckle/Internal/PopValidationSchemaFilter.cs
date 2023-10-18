using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using PopValidations.Execution.Description;
using PopValidations.Execution.Validations;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PopValidations.Swashbuckle.Internal;

public class PopValidationSchemaFilter : ISchemaFilter
{
    private readonly IValidationRunnerFactory factory;
    private readonly OpenApiConfig config;
    private readonly ILogger<PopValidationSchemaFilter> logger;

    public PopValidationSchemaFilter(
        IValidationRunnerFactory factory,
        OpenApiConfig config,
        ILogger<PopValidationSchemaFilter> logger
    )
    {
        this.factory = factory;
        this.config = config;
        this.logger = logger;
    }

    public void Apply(OpenApiSchema model, SchemaFilterContext context)
    {
        if (model.Properties?.Any() != true)
        {
            return;
        }

        var runner = factory.GetRunner(context.Type);

        if (runner == null)
            return;

        if (model.Required == null)
        {
            logger.LogInformation("TEST: Model.Required is null");
            model.Required = new HashSet<string>();
        }

        var results = runner.Describe();

        if (!results.Results.Any())
            return;

#pragma warning disable CS8604 // Possible null reference argument.
        var extensions = InitExtension(model);
#pragma warning restore CS8604 // Possible null reference argument.

        RunRules(String.Empty, model, results.Results, context, null, context.Type, extensions, null);
    }

    private ValidationLevel CalculateOverride(ValidationLevel? validationLevelOverride, ValidationLevel objLevel)
    {
        if (validationLevelOverride == null) return objLevel;
        if (validationLevelOverride > objLevel) return objLevel;

        return validationLevelOverride.Value;
    }

    private void RunRules(
        string parent,
        OpenApiSchema model,
        List<DescriptionItemResult> fieldDescriptions,
        SchemaFilterContext context,
        string? ownedby,
        Type? childType,
        OpenApiObject endPointObjectextention,
        ValidationLevel? validationLevelOverride
    )
    {
        //Need to decide WHEN to turn on or off logging schema details.
        ValidationLevel ObjectLevel = ValidationLevel.None;
        var objType = GetPropertyType(config, childType ?? context.Type, parent);
        if (objType != null)
        {
            ObjectLevel = (config.TypeValidationLevel?.Invoke(objType!) ?? 0);
        }

        ObjectLevel = CalculateOverride(validationLevelOverride, ObjectLevel);

        if (ObjectLevel == ValidationLevel.None) { 
            return; 
        }

        // Loop through the OpenApi model's proeprties
        foreach (var openApiPropName in model.Properties.Keys)
        {
            //TODO:  If property is ignored, don't do these things.
            ValidationLevel PropertyValidationLevel = ValidationLevel.None;
            var propType = GetPropertyType(config, childType ?? context.Type, parent);
            if (propType != null)
            {
                PropertyValidationLevel = (config.TypeValidationLevel?.Invoke(propType!) ?? ValidationLevel.FullDetails);
            }

            PropertyValidationLevel = CalculateOverride(validationLevelOverride, PropertyValidationLevel);

            if (PropertyValidationLevel == ValidationLevel.None) { return; }

            validationLevelOverride = PropertyValidationLevel;

            // Generate a field Name Approximation, from the previous objects's accessor (parent), and the child property key.
            var fieldName = !string.IsNullOrWhiteSpace(parent) ? parent + config.ChildIndicator + openApiPropName : openApiPropName;

            // If the field descriptor has something that matches the current field
            if (
                fieldDescriptions.Any(
                    x => x.Property.StartsWith(fieldName, StringComparison.OrdinalIgnoreCase)
                )
            )
            {
                // Find the field descriptor exactly.
                var fieldOutcomes = fieldDescriptions.FirstOrDefault(
                    x => config.ObjectPropertyIsJsonProperty(x.Property, fieldName)
                );
                // Find the field descriptor if its an array
                var arrayOutcomes = fieldDescriptions.FirstOrDefault(
                    x => config.ObjectPropertyIsDescriptorArray(x.Property, fieldName)
                );

#pragma warning disable CS8604 // Possible null reference argument.
                var customRulesArray = InitExtensionsAndArray(model, openApiPropName/*, outcomeSet, ownedby*/);
#pragma warning restore CS8604 // Possible null reference argument.

                // Execute processes for Field and Array descriptors for the property.
                foreach (var outcomeSet in new[] { fieldOutcomes, arrayOutcomes })
                {
                    if (outcomeSet is null)
                        continue;

                    ConvertValiatorsToOpenApiDescriptions(
                        model, 
                        endPointObjectextention, 
                        openApiPropName, 
                        PropertyValidationLevel, 
                        fieldName, 
                        customRulesArray, 
                        outcomeSet
                    );

                }

                //if (PropertyValidationLevel.HasFlag(ValidationLevel.ValidationAttributeInBase))
                {
                    var newOwner = ownedby ?? "Owned By " + context.Type.Name;
                    if (
                        model.Properties[openApiPropName].Reference != null
                        && context.SchemaRepository.Schemas.ContainsKey(
                            model.Properties[openApiPropName].Reference.Id
                        )
                    )
                    {
                        var childObject = context.SchemaRepository.Schemas[
                            model.Properties[openApiPropName].Reference.Id
                        ];

                        RunRules(
                            fieldName,
                            childObject,
                            fieldDescriptions,
                            context,
                            newOwner + config.ChildIndicator + model.Properties[openApiPropName].Reference.Id,
                            propType,
                            endPointObjectextention,
                            validationLevelOverride
                        );
                    }

                    if (
                        model.Properties[openApiPropName].Items?.Reference != null
                        && context.SchemaRepository.Schemas.ContainsKey(
                            model.Properties[openApiPropName].Items.Reference.Id
                        )
                    )
                    {
                        var childObject = context.SchemaRepository.Schemas[
                            model.Properties[openApiPropName].Items.Reference.Id
                        ];

                        RunRules(
                            fieldName + config.OrdinalIndicator,
                            childObject,
                            fieldDescriptions,
                            context,
                            newOwner + config.ChildIndicator + model.Properties[openApiPropName].Items.Reference.Id,
                            propType,
                            endPointObjectextention,
                            validationLevelOverride
                        );
                    }
                }
            }
        }
    }

    private List<(string, DescriptionOutcome)> FlattenOutcomes(DescriptionItemResult descriptionItem)
    {
        //string Property = descriptionItem.Property;
        var endOutcomes = new List<(string, DescriptionOutcome)>();

        if (descriptionItem == null) return endOutcomes;
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
                endOutcomes.AddRange(FlattenRecurse(string.Empty, group));
            }
        }

        return endOutcomes;
    }

    private List<(string, DescriptionOutcome)> FlattenRecurse(string existing, DescriptionGroupResult group)
    {
        var endOutcomes = new List<(string, DescriptionOutcome)>();
        var additive = string.IsNullOrWhiteSpace(existing) ? group.Description : config.MultiGroupIndicator + group.Description;

        if (group.Outcomes?.Any() == true)
        {
            foreach(var outcome in group.Outcomes)
            {
                if (outcome == null) continue;

                endOutcomes.Add(new (existing + additive, outcome!));
            }
        }

        if (group.Children?.Any() == true)
        {
            foreach (var child in group.Children)
            {
                endOutcomes.AddRange(FlattenRecurse(existing + additive, child));
            }
        }

        return endOutcomes;
    }

    private void ConvertValiatorsToOpenApiDescriptions(
        OpenApiSchema model, 
        OpenApiObject endPointObjectextention, 
        string openApiPropName, 
        ValidationLevel PropertyValidationLevel, 
        string fieldName, 
        OpenApiArray customRulesArray, 
        DescriptionItemResult? outcomeSet)
    {

        if (outcomeSet == null) return;

        var validationArray = new PopValidationArray(customRulesArray);

        foreach (
            var (owner, outcome) in FlattenOutcomes(outcomeSet)
        //var outcome in outcomeSet?.Outcomes
        //   ?? Enumerable.Empty<DescriptionOutcome>()
        )
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
                    if (string.IsNullOrWhiteSpace(owner) && PropertyValidationLevel.HasFlag(ValidationLevel.OpenApi))
                    {
                        converter.UpdateSchema(model, model.Properties[openApiPropName], openApiPropName, outcome);
                    }

                    if (PropertyValidationLevel.HasFlag(ValidationLevel.ValidationAttribute))
                    {
                        converter.UpdateAttribute(model, model.Properties[openApiPropName], openApiPropName, outcome, validationArray);
                    }

                    if (PropertyValidationLevel.HasFlag(ValidationLevel.ValidationAttributeInBase))
                    {
                        var array = new PopValidationArray(InitArray(endPointObjectextention, fieldName));
                        array.SetLineHeader(owner);
                        converter.UpdateAttribute(model, model.Properties[openApiPropName], openApiPropName, outcome, array);
                    }
                }
            }
        }

        //foreach (
        //    var (owner, outcome) in FlattenOutcomes(outcomeSet)
        //)
        //{
        //    foreach (var converter in config.Converters)
        //    {
        //        // Check if it supports the descriptor outcome
        //        if (converter.Supports(outcome))
        //        {
        //            if (PropertyValidationLevel.HasFlag(ValidationLevel.ValidationAttribute))
        //            {
        //                converter.UpdateAttribute(model, model.Properties[openApiPropName], openApiPropName, outcome, customRulesArray);
        //            }

        //            if (PropertyValidationLevel.HasFlag(ValidationLevel.ValidationAttributeInBase))
        //            {
        //                var array = InitArray(endPointObjectextention, fieldName);
        //                converter.UpdateAttribute(model, model.Properties[openApiPropName], openApiPropName, outcome, array);
        //            }
        //        }
        //    }
        //}
    }

    public static bool IsGenericList(Type oType)
    {
        Console.WriteLine(oType.FullName);
        if (oType.IsGenericType)
        {
            var genDef = oType.GetGenericTypeDefinition();

            if (genDef.GetGenericTypeDefinition() == typeof(IEnumerable<>)) return true;

            if (genDef.GetInterface(typeof(IEnumerable<>).Name) != null) return true;

            return false;
        }
        return false;
    }

    private static Type? GetPropertyType(OpenApiConfig config, Type? src, string propName)
    {
        if (src == null) throw new ArgumentException("Value cannot be null.", "src");
        if (string.IsNullOrWhiteSpace(propName)) return src;

        var realProp = propName.Split(new char[] { '.' }).Last();

        //if (propName.Contains("."))//complex type nested
        //{
        //    var temp = propName.Split(new char[] { '.' }, 2);
        //    return GetPropertyType(config, GetPropertyType(config, src, temp[0]), temp[1]);
        //}
        //else
        {
            if (realProp.Contains(config.OrdinalIndicator))
            {
                var result = config.GetPropertyFromType(src, realProp.Replace(config.OrdinalIndicator, ""));
                if (IsGenericList(result.PropertyType))
                {
                    return result.PropertyType.GetGenericArguments().FirstOrDefault();
                }
                return null;
            }
            else
            {
                var result = config.GetPropertyFromType(src, realProp);

                return result?.PropertyType;
            }
            //return prop != null ? prop.GetValue(src, null) : null;
        }
    }

    private OpenApiObject InitExtension(OpenApiSchema modelSchema)
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

    private OpenApiArray InitArray(
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

    private OpenApiArray InitExtensionsAndArray(
        OpenApiSchema modelSchema,
        string key//,
        //DescriptionItemResult outcome,
        //string? ownedBy
    )
    {
        var modelValidations = InitExtension(modelSchema);

        return InitArray(modelValidations, key);

        //foreach (var group in outcome.ValidationGroups)
        //{
        //    array.Add(
        //        new OpenApiString(
        //            (ownedBy
        //                + RecursiveGroupDescriber(group, string.Empty, string.Empty)).Trim()
        //        )
        //    );
        //}

        //foreach (var description in outcome.Outcomes)
        //{
        //    array.Add(new OpenApiString(description.Message?.Trim()));
        //}
    }

    private string RecursiveGroupDescriber(
        DescriptionGroupResult group,
        string depth,
        string existing
    )
    {
        if (!string.IsNullOrWhiteSpace(group.Description))
        {
            existing += config.NewLine + depth + group.Description;
            depth += config.IndentCharacter;
        }

        foreach (var parentGroup in group.Children)
        {
            existing = RecursiveGroupDescriber(parentGroup, depth, existing);
        }

        foreach (var description in group.Outcomes)
        {
            existing += config.NewLine + depth + description.Message;
        }

        return existing;
    }
}
