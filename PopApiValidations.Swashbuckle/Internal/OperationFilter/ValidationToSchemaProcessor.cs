using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using PopValidations.Configurations.Languages;
using PopValidations.Swashbuckle.Internal;

namespace PopApiValidations.Swashbuckle.Internal.OperationFilter;

public static class ValidationToSchemaProcessor
{
    public static void ProcessOpenApiParameter(
        OpenApiParamBasis parameter, 
        ValidationForSchema validationDescriptions,
        BaseData baseData
    )
    {
        foreach(var outcome in validationDescriptions.Outcomes)
        {
            PopValidationArray? validationArray = null;
            PopValidationArray? requestBodyValidationArray = null;

            foreach (var converter in baseData.Config.PopApiConverters)
            {
                // Check if it supports the descriptor outcome
                if (converter.Supports(outcome.Outcome))
                {
                    if (validationArray is null && parameter.RequestBody is null)
                    {
                        validationArray = InitExtensionsAndArray(
                            baseData.Config.CustomValidationAttribute, 
                            baseData.Operation, 
                            parameter.OpenApiPropertyName);
                    }

                    if (parameter.RequestBody is not null && requestBodyValidationArray is null)
                    {
                        requestBodyValidationArray = InitExtensionsAndArray(
                            baseData.Config.CustomValidationAttribute, 
                            baseData.Operation,
                            "RequestBody");
                    }

                    foreach (var schema in parameter.Schemas) 
                    {
                        // Do not modify OpenApiSchema validations, for any group, incase that group would cause the validation to not apply. 
                        if (string.IsNullOrWhiteSpace(outcome.GroupTitle))
                        {
                            if (parameter.ParameterSchema is not null)
                            {
                                converter.UpdateParamSchema(
                                    owningObjectSchema: baseData.Operation,
                                    paramSchema: parameter.ParameterSchema,
                                    //schema: schema,
                                    paramName: parameter.OpenApiPropertyName,
                                    description: outcome.Outcome);
                            }

                            if (parameter.RequestBody != null)
                            {
                                converter.UpdateRequestBodySchema(
                                    owningObjectSchema: parameter.RequestBody,
                                    paramSchema: schema,
                                    paramName: parameter.OpenApiPropertyName,
                                    description: outcome.Outcome);
                            }

                            converter.UpdateSchema(
                                null,
                                propertySchema: schema,
                                property: parameter.OpenApiPropertyName,
                                description: outcome.Outcome);
                        }

                        if (parameter.RequestBody is not null && requestBodyValidationArray is not null)
                        {
                            converter.UpdateAttribute(
                                owningObjectSchema: baseData.Operation,
                                paramSchema: schema,
                                paramName: parameter.OpenApiPropertyName,
                                description: outcome.Outcome,
                                attributeDescription: requestBodyValidationArray);
                        }
                        else if (parameter.ParameterSchema is not null)
                        {
                            converter.UpdateAttribute(
                                owningObjectSchema: baseData.Operation,
                                paramSchema: schema,
                                paramName: parameter.OpenApiPropertyName,
                                description: outcome.Outcome,
                                attributeDescription: validationArray);
                        }
                    }
                }
            }
        }

        var properties = parameter.GetPropertyBases();
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
                            && schemaRepository.Schemas.ContainsKey(model.Properties[openApiPropName].Items.Reference.Id)
                        )
                        ? schemaRepository.Schemas[model.Properties[openApiPropName].Items.Reference.Id]
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
                            newOwner + config.OrdinalIndicator + config.ChildIndicator + (model.Properties[openApiPropName].Items.Reference?.Id ?? openApiPropName),
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





    public static PopValidationArray InitExtensionsAndArray(
        string extensionKey,
        OpenApiOperation oeprationSchema,
        string propertyKey
    )
    {
        return new PopValidationArray(InitArray(InitExtension(extensionKey, oeprationSchema), propertyKey));
    }

    public static OpenApiObject InitExtension(string extensionKey, OpenApiOperation oeprationSchema)
    {
        if (oeprationSchema.Extensions.ContainsKey(extensionKey))
        {
            if (oeprationSchema.Extensions[extensionKey] is OpenApiObject converted)
            {
                return converted;
            }
            else
            {
                var modelValidations = new OpenApiObject();
                oeprationSchema.Extensions[extensionKey] = modelValidations;
                return modelValidations;
            }
        }
        else
        {
            var modelValidations = new OpenApiObject();
            oeprationSchema.Extensions.Add(extensionKey, modelValidations);
            return modelValidations;
        }
    }


    private static OpenApiArray InitArray(
        OpenApiObject owningObject,
        string propertyKey
    )
    {
        OpenApiArray array;
        if (owningObject.ContainsKey(propertyKey))
        {
            array = owningObject[propertyKey] as OpenApiArray ?? new OpenApiArray();
        }
        else
        {
            array = new OpenApiArray();
        }

        owningObject[propertyKey] = array;

        return array;
    }

    //public void RunParameterRule(
    //    OpenApiValidationParam paramInfo,
    //    string currentObjectGraph,
    //    List<DescriptionItemResult> resultObjectGraph,
    //    SchemaRepository schemaRepository,
    //    //OpenApiObject endPointExtensions,
    //    ValidationLevel validationLevel
    //)
    //{
    //    var flattenedOutcomes = FlattenOutcomes(config, resultObjectGraph, currentObjectGraph);

    //    if (flattenedOutcomes.Any(x => x.GroupTitle == string.Empty))
    //    {
    //        //var functionExtensionsArray = InitExtensionsAndArray(config, paramInfo.Operation, paramInfo.ParamName);
    //        PopValidationArray? validationArray = null;// = new PopValidationArray(functionExtensionsArray);
    //        PopValidationArray? requestBodyValidationArray = null;

    //        foreach (var (owner, outcome) in flattenedOutcomes.Where(x => x.Item1 == string.Empty))
    //        {
    //            string propertyName = paramInfo.OpenApiParamName + ((paramInfo.IsArray) ? config.OrdinalIndicator : string.Empty);

    //            foreach (var converter in config.PopApiConverters)
    //            {
    //                // Check if it supports the descriptor outcome
    //                if (converter.Supports(outcome))
    //                {
    //                    if (validationArray is null && paramInfo.ParamRequestBody is null)
    //                    {
    //                        var functionExtensionsArray = InitExtensionsAndArray(config, paramInfo.Operation, propertyName);
    //                        validationArray = new PopValidationArray(functionExtensionsArray);
    //                    }

    //                    if (paramInfo.ParamRequestBody is not null && requestBodyValidationArray is null)
    //                    {
    //                        var requestBodyExtensionsArray = InitExtensionsAndArray(config, paramInfo.Operation, propertyName);
    //                        requestBodyValidationArray = new PopValidationArray(requestBodyExtensionsArray);
    //                    }

    //                    //Incomplete
    //                    if (string.IsNullOrWhiteSpace(owner))
    //                    {
    //                        if (paramInfo.OpenApiParam != null)
    //                            converter.UpdateParamSchema(paramInfo.Operation, paramInfo.OpenApiParam, propertyName, outcome);

    //                        if (paramInfo.ParamRequestBody != null)
    //                            converter.UpdateRequestBodySchema(paramInfo.ParamRequestBody, paramInfo.ParameterSchema, propertyName, outcome);
    //                    }

    //                    if (paramInfo.ParamRequestBody != null && requestBodyValidationArray is not null)
    //                        converter.UpdateAttribute(paramInfo.Operation, paramInfo.ParameterSchema, propertyName, outcome, requestBodyValidationArray);
    //                    else if (!string.IsNullOrEmpty(paramInfo.OpenApiParam?.Name))
    //                        converter.UpdateAttribute(paramInfo.Operation, paramInfo.ParameterSchema, propertyName, outcome, validationArray);
    //                }
    //            }
    //        }

    //        //ConvertValiatorsToOpenApiDescriptionsForParam(
    //        //    config,
    //        //    paramInfo,
    //        //    //paramInfo.Operation,
    //        //    //paramInfo.ParameterSchema,
    //        //    //paramInfo.ParamName,
    //        //    validationLevel,
    //        //    flattenedOutcomes
    //        //);

    //    }
    //    flattenedOutcomes = flattenedOutcomes.Where(x => x.GroupTitle != string.Empty).ToList();

    //    ConvertValiatorsToOpenApiDescriptions(
    //        config,
    //        null,
    //        paramInfo.ParameterSchema,
    //        paramInfo.ParamName,
    //        validationLevel,
    //        flattenedOutcomes
    //    );

    //    RunObjectRules(
    //        paramInfo,
    //        config,
    //        currentObjectGraph,
    //        paramInfo.ParameterSchema,
    //        resultObjectGraph,
    //        schemaRepository,
    //        paramInfo.ParamType,
    //        null,
    //        paramInfo.ParamType,
    //        //endPointExtensions,
    //        ValidationLevel.FullDetails
    //    );
    //}

}
