using Microsoft.OpenApi.Models;
using PopValidations.Execution.Validations;
using PopValidations.Swashbuckle.Converters.Base;
using PopValidations.Swashbuckle.Internal;

namespace PopApiValidations.Swashbuckle.Converters;

public interface IPopApiValidationToOpenApiConverter : IValidationToOpenApiConverter
{
    void UpdateParamSchema(
        OpenApiOperation owningObjectSchema,
        OpenApiParameter parameterSchema,
        string paramName,
        DescriptionOutcome description
    );

    void UpdateRequestBodySchema(
        OpenApiRequestBody owningObjectSchema,
        OpenApiSchema paramSchema,
        string paramName,
        DescriptionOutcome description
    );

    void UpdateAttribute(
        OpenApiOperation owningObjectSchema,
        OpenApiSchema paramSchema,
        string paramName,
        DescriptionOutcome description,
        PopValidationArray attributeDescription
    );
}
