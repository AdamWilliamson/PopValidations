using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using PopValidations.Execution.Validations;
using PopValidations.Swashbuckle.Converters.Base;
using PopValidations.Swashbuckle.Helpers;
using PopValidations.Validations;

namespace PopValidations.Swashbuckle.Converters;

public class IsLengthInclusivelyBetweenValidationToOpenApiConverter : IValidationToOpenApiConverter
{
    public bool Supports(DescriptionOutcome description)
    {
        return description.Validator == GenericNameHelper
            .GetNameWithoutGenericArity(typeof(IsLengthInclusivelyBetweenValidation<>));
    }

    public void UpdateSchema(
        OpenApiSchema owningObjectSchema,
        OpenApiSchema propertySchema,
        string property,
        DescriptionOutcome description
    )
    {
        var startValue = description.Values.FirstOrDefault(x => x.Key == "startValue").Value;
        var endValue = description.Values.FirstOrDefault(x => x.Key == "endValue").Value;

        if (int.TryParse(startValue, out var start) && int.TryParse(endValue, out var end))
        {
            propertySchema.MinLength = start;
            propertySchema.MaxLength = end;
        }
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
