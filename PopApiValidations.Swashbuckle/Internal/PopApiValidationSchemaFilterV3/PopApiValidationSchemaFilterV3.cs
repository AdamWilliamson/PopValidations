using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using PopValidations.Execution.Description;
using PopValidations.Swashbuckle;
using PopValidations.Swashbuckle.Internal;
using Swashbuckle.AspNetCore.SwaggerGen;
using PopApiValidations.Swashbuckle.Converters;
using PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.Helpers;
using PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.MethodSimplification;
using PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.OpenApiSimplification;
using PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.OpenApiToMethodMapping;
using Microsoft.AspNetCore.Components.Forms;

namespace PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3;

public class PopApiValidationSchemaFilter : IOperationFilter
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

        var results = runner.Describe(context.MethodInfo);

        if (!results.Results.Any()) return;

        var openApiMapper = new OpenApiToSimplifier();
        var typeMapper = new MethodSimplifier();
        var openApiToTypeMapper = new OpenApiToTypeMapper();

        var baseData = new BaseData(config, operation, context.SchemaRepository, context.MethodInfo, results.Results);

        var typeMapping = typeMapper.GetMethodMap(context.MethodInfo);
        var openApiMapping = openApiMapper.MapOpenApiOperation(operation, context.SchemaRepository, context.MethodInfo);
        var flatMap = openApiToTypeMapper.MapOpenApiOperationToFunction(
            openApiMapping,
            typeMapping
        );

        ProcessTheFlatMap(baseData, flatMap, results, context);
    }

    private void ProcessTheFlatMap(
        BaseData baseData,
        List<TypeToOpenApiMappingResult> mappingResults,
        DescriptionResult results,
        OperationFilterContext context)
    {
        var groupings = mappingResults.GroupBy(x => (x.FunctionItemMapping as ParameterMapping)?.ParameterInfo?.Position ?? -1);
        foreach (var grouping in groupings)
        {
            ProcessTheParameterGroup(
                baseData,
                grouping.Key,
                grouping.ToList(),
                results,
                context
            );
        }
    }

    private void ProcessTheParameterGroup(
        BaseData baseData,
        int position,
        List<TypeToOpenApiMappingResult> mappingResults,
        DescriptionResult results,
        OperationFilterContext context)
    {
        string functionDesc = 
            ApiValidations.Execution.PopApi.Configuation.DescribeValidatingParam.Invoke(
                context.MethodInfo, position, null
            );

        foreach (var mappingResult in mappingResults)
        {
            string desc = functionDesc;

            //if (position == -1)
            //{
            //    desc += ApiValidations.Execution.PopApi.Configuation.ReturnDescription.Invoke(mappingResult.FunctionReturnMapping.ReturnType);
            //}

            if (!string.IsNullOrWhiteSpace(mappingResult.ResultPropertyHeirarchy))
            {
                if (mappingResult.ResultPropertyHeirarchy?.StartsWith("[n]") == true)
                {
                    desc = desc + mappingResult.ResultPropertyHeirarchy;
                }
                else
                {
                    desc = desc + "." +mappingResult.ResultPropertyHeirarchy;
                }
            }
            
            var validationDescriptions = ValidationProcessor.GetFlattenedValidationsFor(baseData.Config, results.Results, desc);

            ProcessProperty(baseData, validationDescriptions, mappingResult);
        }
    }

    private void ProcessProperty(BaseData baseData, ValidationForSchema validationDescriptions, TypeToOpenApiMappingResult mapping)
    {
        foreach (var outcome in validationDescriptions.Outcomes)
        {
            foreach (var converter in baseData.Config.PopApiConverters.Where(c => c.Supports(outcome.Outcome)))
            {
                // Property, or not, Matches a Specific OpenApi Parameter
                if (mapping.Parameter is not null && mapping.DirectParentSchema is null) //mapping.PropertyMapping is null && 
                {
                    AddValidationToOpenApiParameter(
                        baseData,
                        mapping,
                        outcome,
                        converter
                    );
                }
                // Not a Property, but is the Request Body.
                else if (mapping.PropertyMapping is null && mapping.RequestBody is not null)
                {
                    AddValidationToOpenApiRequestBody(
                        baseData,
                        mapping,
                        outcome,
                        converter
                    );
                }
                // Is a Response
                else if (mapping.PropertyMapping is null && mapping.ResponseMapping is not null)
                {
                    AddValidationToOpenApiResponse(
                        baseData,
                        mapping,
                        outcome,
                        converter
                    );
                }
                else
                {
                    AddValidationToProperties(
                        baseData,
                        mapping,
                        outcome,
                        converter
                    );
                }
            }
        }
    }

    public static void AddValidationToOpenApiParameter(
        BaseData baseData,
        TypeToOpenApiMappingResult mapping,
        GroupedDescriptions outcome,
        IPopApiValidationToOpenApiConverter converter
    )
    {
        var ove = baseData.Config.TypeValidationLevel
            ?.Invoke(mapping.PropertyMapping?.PropertyType 
            ?? (mapping.ParameterMapping as ParameterMapping)?.ParameterInfo.ParameterType);

        var validationLevel = CalculateOverride(ove, ValidationLevel.FullDetails);

        if (string.IsNullOrWhiteSpace(outcome.GroupTitle) && validationLevel.HasFlag(ValidationLevel.OpenApi))
        {
            if (mapping.Parameter is not null)
            {
                if (mapping.IsArray)
                {
                    converter.UpdateParamArraySchema(
                        owningObjectSchema: baseData.Operation,
                        itemSchema: mapping.Parameter.Schema.Items,
                        paramName: ToLowerFirstChar(mapping.Parameter.Name, mapping),
                        description: outcome.Outcome);
                }
                else
                {
                    converter.UpdateParamSchema(
                        owningObjectSchema: baseData.Operation,
                        paramSchema: mapping.Parameter,
                        paramName: ToLowerFirstChar(mapping.Parameter.Name, mapping),
                        description: outcome.Outcome);
                }
            }
        }

        var validationArray = GetValidationArray(
                validationLevel,
                baseData,
                baseData.Config.CustomValidationAttribute,
                outcome.GroupTitle,
                mapping
            );

        if (validationArray != null)
        {
            converter.UpdateAttribute(
                owningObjectSchema: baseData.Operation,
                paramSchema: mapping.Parameter.Schema,
                paramName: ToLowerFirstChar(mapping.Parameter.Name, mapping),
                description: outcome.Outcome,
                attributeDescription: validationArray
            );
        }
    }

    public static void AddValidationToOpenApiRequestBody(
        BaseData baseData,
        TypeToOpenApiMappingResult mapping,
        GroupedDescriptions outcome,
        IPopApiValidationToOpenApiConverter converter
    )
    {
        var ove = baseData.Config.TypeValidationLevel?.Invoke((mapping.ParameterMapping as ParameterMapping)?.ParameterInfo.ParameterType);

        var validationLevel = CalculateOverride(ove, ValidationLevel.FullDetails);

        if (string.IsNullOrWhiteSpace(outcome.GroupTitle) && validationLevel.HasFlag(ValidationLevel.OpenApi))
        {
            foreach (var schema in mapping.RequestBodyContentSchemas)
            {
                converter.UpdateRequestBodySchema(
                    owningObjectSchema: mapping.RequestBody,
                    paramSchema: schema,
                    paramName: "requestBody",
                    description: outcome.Outcome
                );
            }
        }

        var validationArray = GetValidationArray(
            validationLevel,
            baseData,
            baseData.Config.CustomValidationAttribute,
            outcome.GroupTitle,
            mapping
        );

        if (validationArray != null)
        {
            converter.UpdateAttribute(
                owningObjectSchema: baseData.Operation,
                paramSchema: null,
                paramName: "requestBody" + (mapping.IsArray?"[n]": string.Empty),
                description: outcome.Outcome,
                attributeDescription: validationArray
            );
        }

        foreach (var schema in mapping.RequestBodyContentSchemas)
        {
            if (mapping.RequestBody is not null)
            {
                converter.UpdateRequestBodySchema(
                    owningObjectSchema: mapping.RequestBody,
                    paramSchema: schema,
                    paramName: string.Empty, //  This can be removed.. Request bodies dont have paramnames.
                    description: outcome.Outcome
                );
            }
        }
    }

    public static void AddValidationToOpenApiResponse(
        BaseData baseData,
        TypeToOpenApiMappingResult mapping,
        GroupedDescriptions outcome,
        IPopApiValidationToOpenApiConverter converter
    )
    {
        var ove = baseData.Config.TypeValidationLevel?.Invoke(mapping.FunctionReturnMapping.ReturnType);

        var validationLevel = CalculateOverride(ove, ValidationLevel.FullDetails);

        var validationArray = GetValidationArray(
                validationLevel,
                baseData,
                baseData.Config.CustomValidationAttribute,
                outcome.GroupTitle,
                mapping
            );

        if (validationArray != null)
        {
            converter.UpdateAttribute(
                owningObjectSchema: baseData.Operation,
                paramSchema: null,
                paramName: "response",
                description: outcome.Outcome,
                attributeDescription: validationArray
            );
        }
    }

    public static void AddValidationToProperties(
        BaseData baseData,
        TypeToOpenApiMappingResult mapping,
        GroupedDescriptions outcome,
        IPopApiValidationToOpenApiConverter converter
    )
    {
        var ove = baseData.Config.TypeValidationLevel?.Invoke(mapping.PropertyMapping.PropertyType);

        var validationLevel = CalculateOverride(ove, ValidationLevel.FullDetails);

        if (string.IsNullOrWhiteSpace(outcome.GroupTitle) && validationLevel.HasFlag(ValidationLevel.OpenApi))
        {
            if (mapping.IsArray)
            {
                converter.UpdateSchema(
                    owningObjectSchema: mapping.PropertySchema,
                    propertySchema: mapping.PropertySchema.Items,
                    property: "items",
                    description: outcome.Outcome
                );
            }
            else
            {
                converter.UpdateSchema(
                    owningObjectSchema: mapping.DirectParentSchema,
                    propertySchema: mapping.PropertySchema,
                    property: ToLowerFirstChar(mapping.PropertyMapping.PropertyName, mapping),
                    description: outcome.Outcome
                );
            }
        }

        var validationArray = GetValidationArray(
            validationLevel,
            baseData,
            baseData.Config.CustomValidationAttribute,
            outcome.GroupTitle,
            mapping
        );

        if (validationArray != null)
        {
            converter.UpdateAttribute(
                owningObjectSchema: baseData.Operation,
                paramSchema: mapping.PropertySchema,
                paramName: ToLowerFirstChar(mapping.PropertyMapping.OpenApiPropertyName, mapping),
                description: outcome.Outcome,
                attributeDescription: validationArray
            );
        }

        if (string.IsNullOrWhiteSpace(outcome.GroupTitle)  && mapping.PropertySecondarySchemas is not null)
        {
            foreach (var schema in mapping.PropertySecondarySchemas)
            {
                converter.UpdateSchema(
                    owningObjectSchema: null,
                    propertySchema: schema,
                    property: ToLowerFirstChar(mapping.PropertyMapping.PropertyName, mapping),
                    description: outcome.Outcome
                );
            }
        }
    }

    private static ValidationLevel CalculateOverride(ValidationLevel? validationLevelOverride, ValidationLevel objLevel)
    {
        if (validationLevelOverride == null) return objLevel;
        if (validationLevelOverride > objLevel) return objLevel;

        return validationLevelOverride.Value;
    }

    private static PopValidationArray? GetValidationArray(
        ValidationLevel validationLevel,
        BaseData baseData,
        string extension, 
        string? groupHeader,
        TypeToOpenApiMappingResult mapping
        )
    {
        if (validationLevel.HasFlag(ValidationLevel.ValidationAttributeInBase))
        {
            var array = PopValidationArray.From(
                extension,
                baseData.Operation.Extensions,
                ToLowerFirstChar(mapping.OpenApiObjHeirarchy, mapping)
            );
            array.SetLineHeader(groupHeader);

            return array;
        }

        if (validationLevel.HasFlag(ValidationLevel.ValidationAttribute))
        {
            if (mapping.PropertyMapping is not null)
            {
                var array = PopValidationArray.From(extension, mapping.ParentPropertyExtensions, ToLowerFirstChar(mapping.OpenApiPropertyName, mapping));
                array.SetLineHeader(groupHeader);

                return array;
            }
            else if (mapping.Parameter is not null)
            {
                var array = PopValidationArray.From(extension, mapping.ParentPropertyExtensions, ToLowerFirstChar(mapping.OpenApiPropertyName, mapping));
                array.SetLineHeader(groupHeader);

                return array;
            }
            else if (mapping.RequestBody is not null)
            {
                var array = PopValidationArray.From(extension, mapping.ParentPropertyExtensions, "requestBody" + (mapping.IsArray? "[n]": string.Empty));
                array.SetLineHeader(groupHeader);

                return array;
            }
            else if (mapping.FunctionReturnMapping is not null)
            {
                var array = PopValidationArray.From(extension, mapping.ParentPropertyExtensions, "response" + (mapping.IsArray? "[n]": string.Empty));
                array.SetLineHeader(groupHeader);

                return array;
            }
        }

        return null;
    }

    private static string ToLowerFirstChar(string input, TypeToOpenApiMappingResult mapping)
    {
        var name = mapping?.Parameter?.Name ?? string.Empty;
        if (name.EndsWith("[n]"))
        {
            name = name.Substring(0, name.LastIndexOf("[n]"));
        }

        if (mapping?.Parameter?.In != null && string.Equals(mapping?.Parameter?.Name, name, StringComparison.InvariantCultureIgnoreCase))
        {
            return input;
        }

        if (string.IsNullOrEmpty(input))
            return input;

        return char.ToLower(input[0]) + input.Substring(1);
    }
}
