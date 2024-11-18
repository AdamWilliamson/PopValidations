using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Interfaces;

namespace PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.OpenApiSimplification;

public class OpenApiResponseMapping
{
    public string StatusCode { get; set; }
    public string Content { get; set; }
    public OpenApiSchema? Schema { get; set; }
    public IDictionary<string, IOpenApiExtension> ParentPropertyExtensions { get; set; }
    public List<OpenApiPropertyMapping> PropertyMappings { get; set; } = new();
}

