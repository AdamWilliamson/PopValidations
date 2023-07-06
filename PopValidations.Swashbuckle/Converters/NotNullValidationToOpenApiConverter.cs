using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using PopValidations.Execution.Validations;
using PopValidations.Swashbuckle.Converters.Base;
using PopValidations.Validations;

namespace PopValidations.Swashbuckle.Converters;

public class NotNullValidationToOpenApiConverter : IValidationToOpenApiConverter
{
    public bool Supports(DescriptionOutcome description)
    {
        return description.Validator == nameof(NotNullValidation);
    }

    public void UpdateSchema(
        OpenApiSchema owningObjectSchema,
        OpenApiSchema propertySchema,
        string property,
        DescriptionOutcome description
    )
    {
        propertySchema.Nullable = false;
        owningObjectSchema.Required.Add(property);
    }

    public void UpdateAttribute(
        OpenApiSchema owningObjectSchema,
        OpenApiSchema propertySchema,
        string property,
        DescriptionOutcome description,
        OpenApiArray attributeDescription
    )
    {
        attributeDescription.Add(new OpenApiString(description.Message));
    }
}
