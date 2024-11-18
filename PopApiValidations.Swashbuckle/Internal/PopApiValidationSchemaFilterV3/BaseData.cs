using Microsoft.OpenApi.Models;
using PopValidations.Execution.Description;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3;

public record BaseData(
    PopApiOpenApiConfig Config,
    OpenApiOperation Operation,
    SchemaRepository SchemaRepository,
    MethodInfo MethodInfo,
    List<DescriptionItemResult> ValidationResults
);
