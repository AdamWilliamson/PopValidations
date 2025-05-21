using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Interfaces;
using PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.MethodSimplification;

namespace PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.OpenApiSimplification;

public class OpenApiResponseMapping : IOpenApiParameterMapping
{
    public string StatusCode { get; set; }
    public string Content { get; set; }
    public OpenApiSchema? Schema { get; set; }
    public IDictionary<string, IOpenApiExtension> ParentPropertyExtensions { get; set; }
    public List<OpenApiPropertyMapping> PropertyMappings { get; set; } = new();



    public string Name => string.Empty;
    public OpenApiLocation Location => OpenApiLocation.Return;
    public List<OpenApiSchema> Schemas => (Schema != null)? new() { Schema }: new();
    public bool IsArray => Schema.Items != null;
}

