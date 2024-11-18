using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Interfaces;

namespace PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.OpenApiSimplification;

public class OpenApiParameterMapping
{
    public string Name { get; set; }
    public ParameterLocation In { get; set; }
    public OpenApiParameter Parameter { get; set; }
    public OpenApiSchema Schema { get; set; }

    public bool IsArray { get; set; } // Indicates if the parameter is an array
    public List<OpenApiPropertyMapping> PropertyMappings { get; set; } = new List<OpenApiPropertyMapping>();
    public IDictionary<string, IOpenApiExtension> ParentPropertyExtensions { get; set; }
}

