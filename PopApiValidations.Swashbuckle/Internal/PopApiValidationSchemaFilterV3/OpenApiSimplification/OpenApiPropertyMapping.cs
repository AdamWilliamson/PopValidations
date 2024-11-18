using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Interfaces;

namespace PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.OpenApiSimplification;

public class OpenApiPropertyMapping
{
    public string PropertyName { get; set; }
    public OpenApiSchema PropertySchema { get; set; }
    public bool IsArray { get; set; } // Indicates if the property is an array
    public List<OpenApiPropertyMapping> NestedProperties { get; set; } = new List<OpenApiPropertyMapping>(); // Nested properties (if any)
    public IDictionary<string, IOpenApiExtension> ParentPropertyExtensions { get; set; }

    public OpenApiSchema[] SecondarySchemas { get; set; }
}

