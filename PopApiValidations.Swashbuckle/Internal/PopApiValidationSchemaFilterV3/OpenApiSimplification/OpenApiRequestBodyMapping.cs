using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Interfaces;
using PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.MethodSimplification;

namespace PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.OpenApiSimplification;

public class OpenApiRequestBodyMapping : IOpenApiParameterMapping
{
    public OpenApiRequestBody RequestBody { get; set; }
    public Dictionary<string, OpenApiSchema> ContentSchemas { get; set; } = new Dictionary<string, OpenApiSchema>();

    public List<OpenApiPropertyMapping> PropertyMappings { get; set; } = new List<OpenApiPropertyMapping>();
    public IDictionary<string, IOpenApiExtension> ParentPropertyExtensions { get; set; }


    public string Name => string.Empty;
    public OpenApiLocation Location => OpenApiLocation.ResponseBody;
    public List<OpenApiSchema> Schemas => ContentSchemas.Values.Take(1).ToList();
    public bool IsArray => ContentSchemas.Values.First().Items != null;
}

