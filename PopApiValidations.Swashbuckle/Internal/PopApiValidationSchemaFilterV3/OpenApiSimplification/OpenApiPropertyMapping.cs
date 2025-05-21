using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Interfaces;

namespace PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.OpenApiSimplification;

//public interface IOpenApiParameterMapping
//{
//    string Name { get; }
//    List<OpenApiPropertyMapping> PropertyMappings { get; }
//    IDictionary<string, IOpenApiExtension> ParentPropertyExtensions { get; }
//    OpenApiLocation Location { get; }
//    List<OpenApiSchema> Schemas { get; }
//}

public interface IOpenApiPropertyMapping 
{
    string Name { get; }
    List<OpenApiPropertyMapping> PropertyMappings { get; }
    IDictionary<string, IOpenApiExtension> ParentPropertyExtensions { get; }
    bool IsArray { get; }
    public List<OpenApiSchema> Schemas { get; }
}

public class OpenApiPropertyMapping : IOpenApiPropertyMapping
{
    public string PropertyName { get; set; }
    public string Name => PropertyName;
    public OpenApiSchema PropertySchema { get; set; }
    public List<OpenApiSchema> Schemas => new() { PropertySchema };
    public OpenApiSchema DirectParentSchema { get; set; }
    public bool IsArray { get; set; } // Indicates if the property is an array
    public List<OpenApiPropertyMapping> NestedProperties { get; set; } = new List<OpenApiPropertyMapping>(); // Nested properties (if any)
    public List<OpenApiPropertyMapping> PropertyMappings => NestedProperties;
    public IDictionary<string, IOpenApiExtension> ParentPropertyExtensions { get; set; }

    public OpenApiSchema[] SecondarySchemas { get; set; }
}

