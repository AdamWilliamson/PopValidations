using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using PopValidations.Execution.Validations;
using PopValidations.Swashbuckle.Converters.Base;
using PopValidations.Swashbuckle.Helpers;
using PopValidations.Swashbuckle.Internal;
using PopValidations.Validations;

namespace PopValidations.Swashbuckle.Converters;

public class IsGreaterThanOrEqualToValidationToOpenApiConverter : IValidationToOpenApiConverter
{
    public bool Supports(DescriptionOutcome description)
    {
        return description.Validator == GenericNameHelper
            .GetNameWithoutGenericArity(typeof(IsGreaterThanOrEqualToValidation));
    }

    public void UpdateSchema(
        OpenApiSchema? owningObjectSchema,
        OpenApiSchema propertySchema,
        string property,
        DescriptionOutcome description
    )
    {
        var value = description.Values.FirstOrDefault(x => x.Key == "value").Value;
        if (decimal.TryParse(value, out var decimalValue))
        {
            propertySchema.Minimum = decimalValue;
            propertySchema.ExclusiveMinimum = false;
        }
    }

    public void UpdateAttribute(
        OpenApiSchema? owningObjectSchema,
        OpenApiSchema propertySchema,
        string property,
        DescriptionOutcome description,
        PopValidationArray attributeDescription
    )
    {
        attributeDescription.Add(description.Message);
    }
}