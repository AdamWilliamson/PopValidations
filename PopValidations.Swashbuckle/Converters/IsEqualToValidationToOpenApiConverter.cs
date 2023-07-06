using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using PopValidations.Execution.Validations;
using PopValidations.Swashbuckle.Converters.Base;
using PopValidations.Validations;

namespace PopValidations.Swashbuckle.Converters;

public class IsEqualToValidationToOpenApiConverter : IValidationToOpenApiConverter
{
    public bool Supports(DescriptionOutcome description)
    {
        return description.Validator == nameof(IsEqualToValidation);
    }

    public void UpdateSchema(
        OpenApiSchema owningObjectSchema,
        OpenApiSchema propertySchema,
        string property,
        DescriptionOutcome description
    )
    {
        propertySchema.Enum = new List<IOpenApiAny>()
        {
            new OpenApiString(description.Values.First(c => c.Key == "value").Value)
        };
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
