using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using PopValidations.Execution.Description;
using PopValidations.Execution.Validations;
using PopValidations.Swashbuckle;
using PopValidations.Swashbuckle.Internal;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections;
using System.Diagnostics;
using PopValidations.FieldDescriptors.Base;
using PopValidations.FieldDescriptors;
using Newtonsoft.Json.Schema;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Http;
using System.Reflection;

namespace PopApiValidations.Swashbuckle.Internal.OperationFilter;

public record BaseData(
    PopApiOpenApiConfig Config,
    OpenApiOperation Operation, 
    SchemaRepository SchemaRepository, 
    MethodInfo MethodInfo, 
    List<DescriptionItemResult> ValidationResults
);

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

        var results = runner.Describe();

        if (!results.Results.Any()) return;

        var operationNavigator = new OpenApiOperationNavigator(
            config: config, 
            schemaRepository: context.SchemaRepository, 
            operation: operation, 
            methodInfo: context.MethodInfo);

        var baseData = new BaseData(config, operation, context.SchemaRepository, context.MethodInfo, results.Results);

        foreach (var paramNavigator in operationNavigator.GetOpenApiParamNavigators())
        {
            var desc = ApiValidations.Execution.PopApiValidations.Configuation.DescribeValidatingParam.Invoke(
                context.MethodInfo, Math.Max(paramNavigator.ParamIndex, 0), null
            );

            var validationDescriptions = ValidationProcessor.GetFlattenedValidationsFor(config, results.Results, paramNavigator.ParameterName);

            foreach (var paramImpl in paramNavigator.GetParamBases(desc, config.OrdinalIndicator))
            {
                ValidationToSchemaProcessor.ProcessOpenApiParameter(paramImpl, validationDescriptions, baseData);
            }

            foreach(var paramProperty in paramNavigator.GetPropertyBases(desc, config.OrdinalIndicator))
            {

            }
        }
    }
}
