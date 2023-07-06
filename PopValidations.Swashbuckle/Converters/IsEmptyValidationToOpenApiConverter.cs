using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using PopValidations.Execution.Validations;
using PopValidations.Swashbuckle.Converters.Base;
using PopValidations.Swashbuckle.Helpers;
using PopValidations.Validations;

namespace PopValidations.Swashbuckle.Converters;

public class IsEmptyValidationToOpenApiConverter : IValidationToOpenApiConverter
{
    public bool Supports(DescriptionOutcome description)
    {
        return description.Validator == GenericNameHelper
            .GetNameWithoutGenericArity(typeof(IsEmptyValidation));
    }

    public void UpdateSchema(
        OpenApiSchema owningObjectSchema,
        OpenApiSchema propertySchema,
        string property,
        DescriptionOutcome description
    )
    {
        propertySchema.MaxLength = 0;
        propertySchema.MaxItems = 0;
    }

    public void UpdateAttribute(
        OpenApiSchema owningObjectSchema, 
        OpenApiSchema propertySchema, 
        string property, 
        DescriptionOutcome description, 
        OpenApiArray attributeDescription)
    {
        attributeDescription.Add(new OpenApiString(description.Message));
    }
}