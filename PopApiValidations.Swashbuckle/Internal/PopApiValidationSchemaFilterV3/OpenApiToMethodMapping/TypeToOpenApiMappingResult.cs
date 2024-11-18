using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.MethodSimplification;
using PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.OpenApiSimplification;

namespace PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.OpenApiToMethodMapping
{
    public class TypeToOpenApiMappingResult
    {
        public string Route { get; set; }
        public string ResultPropertyHeirarchy => PropertyMapping?.ResultPropertyName ?? string.Empty;
        public string OpenApiObjHeirarchy { get; set; }
        public string OpenApiPropertyName { get; set; }
        public PropertyMapping? PropertyMapping { get; set; }
        public OpenApiSchema? PropertySchema { get; set; }
        public OpenApiSchema[]? PropertySecondarySchemas { get; set; }
        public IDictionary<string, IOpenApiExtension> ParentPropertyExtensions { get; set; }
        public OpenApiParameter? Parameter { get; set; }
        public OpenApiRequestBody? RequestBody { get; set; }
        public OpenApiSchema[] RequestBodyContentSchemas => RequestBody?.Content.Select(c => c.Value.Schema)?.ToArray() ?? new OpenApiSchema[0];
        public bool IsArray { get; set; }
        public ParameterMapping ParameterMapping { get; set; } // Added to store the method parameter index
        public OpenApiResponseMapping? ResponseMapping { get; set; }
        public ReturnMapping? ReturnMapping { get; set; }
    }
}
