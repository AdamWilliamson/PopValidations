using System.Reflection;
using Microsoft.OpenApi.Interfaces;

namespace PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.OpenApiSimplification;

public class OpenApiOperationMapping
{
    public string Path { get; set; }
    public string HttpMethod { get; set; } // GET, POST, PUT, DELETE, etc.
    public MethodInfo MethodInfo { get; set; } // The MethodInfo for the operation
    public List<OpenApiParameterMapping> Parameters { get; set; } = new List<OpenApiParameterMapping>();
    public OpenApiRequestBodyMapping RequestBody { get; set; } // Request body, if applicable
    public List<OpenApiResponseMapping> Responses { get; set; } = new List<OpenApiResponseMapping>(); // Responses for the operation
    public IDictionary<string, IOpenApiExtension> Extensions { get; set; }
}

