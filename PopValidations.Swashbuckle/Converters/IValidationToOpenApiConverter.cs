using Microsoft.OpenApi.Models;
using PopValidations.Execution.Validations;

namespace PopValidations.Swashbuckle.Converters;

public interface IValidationToOpenApiConverter
{
    bool Supports(DescriptionOutcome outcome);
    void UpdateSchema(
        OpenApiSchema owningObjectSchema,
        OpenApiSchema propertySchema,
        string property,
        DescriptionOutcome description
    );
}
