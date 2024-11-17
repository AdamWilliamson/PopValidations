using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using PopValidations.Execution.Description;
using PopValidations.Swashbuckle;
using PopValidations.Swashbuckle.Internal;
using Swashbuckle.AspNetCore.SwaggerGen;
using PopApiValidations.Swashbuckle.Internal.OperationFilter;
using PopApiValidations.Swashbuckle.Converters;

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

        
        var openApiMapper = new OpenApiToMapping();
        var typeMapper = new TypeToMapping();
        var openApiToTypeMapper = new OpenApiToTypeMapper();

        var baseData = new BaseData(config, operation, context.SchemaRepository, context.MethodInfo, results.Results);

        var typeMapping = typeMapper.GetMethodMap(context.MethodInfo);//CreateMappings(context.MethodInfo.DeclaringType);
        var openApiMapping = openApiMapper.MapOpenApiOperation(operation, context.SchemaRepository, context.MethodInfo);
        var flatMap = openApiToTypeMapper.MapOpenApiOperationToFunction(
            openApiMapping,
            typeMapping//.Single(x => x.MethodInfo == openApiMapping.MethodInfo)
        );

        ProcessTheFlatMap(baseData, flatMap, results, context);
    }

    private void ProcessTheFlatMap(
        BaseData baseData,
        List<TypeToOpenApiMappingResult> mappingResults,
        DescriptionResult results,
        OperationFilterContext context)
    {
        //foreach (var mappingResult in mappingResults)
        //{
        //    ////desc = (string.IsNullOrWhiteSpace(paramNavigator.OpenApiParameterName)) ? desc : desc + "." + paramNavigator.OpenApiParameterName;
        //    //var prefix = PopApi.Configuation.DescribeValidatingParam?.Invoke(context.MethodInfo, 0, null);
        //    //var validationDescriptions = ValidationProcessor.GetFlattenedValidationsFor(config, results.Results, desc); 
        //}

        var groupings = mappingResults.GroupBy(x => x.ParameterMapping?.ParameterInfo?.Position ?? -1);
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
        string functionDesc = //(position != -1)
            //? 
            ApiValidations.Execution.PopApi.Configuation.DescribeValidatingParam.Invoke(
                    context.MethodInfo, position, null
                );
            //: ApiValidations.Execution.PopApi.Configuation.DescribeValidatingReturn.Invoke(
            //        context.MethodInfo,null
            //    );

        foreach (var mappingResult in mappingResults)
        {
            string desc = functionDesc;

            if (position == -1)
            {
                desc += ApiValidations.Execution.PopApi.Configuation.ReturnDescription.Invoke(mappingResult.ReturnMapping.ReturnType);
            }

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
            //if (mappingResult.PropertyMapping is not null)
            //{
            //    desc = string.Join('.', desc, mappingResult.PropertyMapping.ResultPropertyName);
            //}
            //else if (mappingResult.ParameterMapping is not null && mappingResult.IsArray)
            //{
            //    desc += "[n]";
            //}
            
            //if (mappingResult.PropertyMapping is null && mappingResult.IsArray)
            //{
            //    desc += "[n]";
            //}

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
                if (mapping.PropertyMapping is null && mapping.Parameter is not null)
                {
                    AddValidationToOpenApiParameter(
                        baseData,
                        mapping,
                        outcome,
                        converter
                    );
                }
                else if (mapping.PropertyMapping is null && mapping.RequestBody is not null)
                {
                    AddValidationToOpenApiRequestBody(
                        baseData,
                        mapping,
                        outcome,
                        converter
                    );
                }
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
        var ove = baseData.Config.TypeValidationLevel?.Invoke(mapping.PropertyMapping?.PropertyType ?? mapping.ParameterMapping.ParameterInfo.ParameterType);

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
                        paramName: mapping.Parameter.Name,
                        description: outcome.Outcome);
                }
                else
                {
                    converter.UpdateParamSchema(
                        owningObjectSchema: baseData.Operation,
                        paramSchema: mapping.Parameter,
                        paramName: mapping.Parameter.Name,
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
                paramSchema: mapping.PropertySchema,
                paramName: mapping.Parameter.Name,
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
        var ove = baseData.Config.TypeValidationLevel?.Invoke(mapping.ParameterMapping.ParameterInfo.ParameterType);

        var validationLevel = CalculateOverride(ove, ValidationLevel.FullDetails);

        if (string.IsNullOrWhiteSpace(outcome.GroupTitle) && validationLevel.HasFlag(ValidationLevel.OpenApi))
        {
            foreach (var schema in mapping.RequestBodyContentSchemas)
            {
                converter.UpdateRequestBodySchema(
                    owningObjectSchema: mapping.RequestBody,
                    paramSchema: schema,
                    paramName: "RequestBody",
                    description: outcome.Outcome);
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
                paramName: "RequestBody" + (mapping.IsArray?"[n]": string.Empty),
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
                    description: outcome.Outcome);
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
        var ove = baseData.Config.TypeValidationLevel?.Invoke(mapping.ReturnMapping.ReturnType);

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
                paramName: "Response",
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
            converter.UpdateSchema( 
                owningObjectSchema: null,
                propertySchema: mapping.PropertySchema,
                property: mapping.PropertyMapping.PropertyName,
                description: outcome.Outcome
            );
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
                paramName: mapping.PropertyMapping.OpenApiPropertyName,
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
                    property: mapping.PropertyMapping.PropertyName,
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
                mapping.OpenApiObjHeirarchy
            );
            array.SetLineHeader(groupHeader);

            return array;
        }

        if (validationLevel.HasFlag(ValidationLevel.ValidationAttribute))
        {
            if (mapping.PropertyMapping is not null)
            {
                var array = PopValidationArray.From(extension, mapping.ParentPropertyExtensions, mapping.OpenApiPropertyName);
                array.SetLineHeader(groupHeader);

                return array;
            }
            else if (mapping.Parameter is not null)
            {
                var array = PopValidationArray.From(extension, mapping.ParentPropertyExtensions, mapping.OpenApiPropertyName);
                array.SetLineHeader(groupHeader);

                return array;
            }
            else if (mapping.RequestBody is not null)
            {
                var array = PopValidationArray.From(extension, mapping.ParentPropertyExtensions, "RequestBody" + (mapping.IsArray? "[n]": string.Empty));
                array.SetLineHeader(groupHeader);

                return array;
            }
            else if (mapping.ReturnMapping is not null)
            {
                var array = PopValidationArray.From(extension, mapping.ParentPropertyExtensions, "Response" + (mapping.IsArray? "[n]": string.Empty));
                array.SetLineHeader(groupHeader);

                return array;
            }
        }

        return null;
    }
}
