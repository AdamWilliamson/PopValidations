using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Interfaces;

namespace PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.OpenApiSimplification;

public class OpenApiRequestBodyMapping
{
    public OpenApiRequestBody RequestBody { get; set; }
    public Dictionary<string, OpenApiSchema> ContentSchemas { get; set; } = new Dictionary<string, OpenApiSchema>();

    public List<OpenApiPropertyMapping> PropertyMappings { get; set; } = new List<OpenApiPropertyMapping>();
    public IDictionary<string, IOpenApiExtension> ParentPropertyExtensions { get; set; }
}

