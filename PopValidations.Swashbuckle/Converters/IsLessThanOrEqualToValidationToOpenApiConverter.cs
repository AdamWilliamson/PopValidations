using Microsoft.OpenApi.Models;
using PopValidations.Execution.Validations;
using PopValidations.Swashbuckle.Converters.Base;
using PopValidations.Swashbuckle.Helpers;
using PopValidations.Validations;

namespace PopValidations.Swashbuckle.Converters;

public class IsLessThanOrEqualToValidationToOpenApiConverter : IValidationToOpenApiConverter
{
    public bool Supports(DescriptionOutcome description)
    {
        return description.Validator == GenericNameHelper
            .GetNameWithoutGenericArity(typeof(IsLessThanOrEqualToValidation));
    }

    public void UpdateSchema(
        OpenApiSchema owningObjectSchema,
        OpenApiSchema propertySchema,
        string property,
        DescriptionOutcome description
    )
    {
        var value = description.Values.FirstOrDefault(x => x.Key == "value").Value;
        if (decimal.TryParse(value, out var decimalValue))
        {
            propertySchema.Maximum = decimalValue;
            propertySchema.ExclusiveMaximum = false;
        }
    }
}