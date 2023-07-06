using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using PopValidations.Execution.Validations;

namespace PopValidations.Swashbuckle.Converters.Base;

public interface IValidationToOpenApiConverter
{
    bool Supports(DescriptionOutcome outcome);
    void UpdateSchema(
        OpenApiSchema owningObjectSchema,
        OpenApiSchema propertySchema,
        string property,
        DescriptionOutcome description
    );

    void UpdateAttribute(
        OpenApiSchema owningObjectSchema,
        OpenApiSchema propertySchema,
        string property,
        DescriptionOutcome description,
        OpenApiArray attributeDescription
    );
}
