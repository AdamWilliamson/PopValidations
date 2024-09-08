using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using PopValidations.Execution.Validations;
using PopValidations.Swashbuckle.Converters.Base;
using PopValidations.Swashbuckle.Helpers;
using PopValidations.Swashbuckle.Internal;
using PopValidations.Validations;

namespace PopValidations.Swashbuckle.Converters;

public class IsNotEmptyValidationToOpenApiConverter : IValidationToOpenApiConverter
{
    public bool Supports(DescriptionOutcome description)
    {
        return description.Validator == GenericNameHelper
            .GetNameWithoutGenericArity(typeof(IsNotEmptyValidation));
    }

    public void UpdateSchema(
        OpenApiSchema? owningObjectSchema,
        OpenApiSchema propertySchema,
        string property,
        DescriptionOutcome description
    )
    {
        if (owningObjectSchema != null && owningObjectSchema.Required == null)
        {
            owningObjectSchema.Required = new HashSet<string>();
        }
        if (owningObjectSchema != null && !owningObjectSchema.Required.Contains(property))
        {
            owningObjectSchema.Required.Add(property);
        }
        propertySchema.MinLength = 1;
        propertySchema.MinItems = 1;
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