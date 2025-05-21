using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Interfaces;
using PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.MethodSimplification;

namespace PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.OpenApiSimplification;

public interface IOpenApiParameterMapping : IOpenApiPropertyMapping
{
    string Name { get; }
    List<OpenApiPropertyMapping> PropertyMappings { get; }
    IDictionary<string, IOpenApiExtension> ParentPropertyExtensions { get; }
    OpenApiLocation Location { get; }
    List<OpenApiSchema> Schemas { get; }
}

public class OpenApiParameterMapping : IOpenApiParameterMapping
{
    public string Name { get; set; }
    public ParameterLocation In { get; set; }
    public OpenApiParameter Parameter { get; set; }
    public OpenApiSchema Schema { get; set; }
    public List<OpenApiSchema> Schemas => new() { Schema };
    public OpenApiLocation Location => In switch
    {
        ParameterLocation.Query => OpenApiLocation.Query,
        ParameterLocation.Header => OpenApiLocation.Header,
        ParameterLocation.Path => OpenApiLocation.Path,
        ParameterLocation.Cookie => OpenApiLocation.Cookie,
        _ => OpenApiLocation.ResponseBody
    };

    public bool IsArray { get; set; } // Indicates if the parameter is an array
    public List<OpenApiPropertyMapping> PropertyMappings { get; set; } = new List<OpenApiPropertyMapping>();
    public IDictionary<string, IOpenApiExtension> ParentPropertyExtensions { get; set; }
}

