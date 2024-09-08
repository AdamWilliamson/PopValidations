using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using PopValidations.Execution.Validations;
using PopValidations.Swashbuckle.Converters.Base;
using PopValidations.Swashbuckle.Helpers;
using PopValidations.Swashbuckle.Internal;
using PopValidations.Validations;

namespace PopValidations.Swashbuckle.Converters;

public class IsEnumValidationToOpenApiConverter : IValidationToOpenApiConverter
{
    public bool Supports(DescriptionOutcome description)
    {
        return description.Validator == GenericNameHelper
            .GetNameWithoutGenericArity(typeof(IsEnumValidation<>));
    }

    public void UpdateSchema(
        OpenApiSchema? owningObjectSchema,
        OpenApiSchema propertySchema,
        string property,
        DescriptionOutcome description
    )
    {
        var fieldType = description.Values.First(c => c.Key == "fieldType").Value;
        var stringValues = description.Values.First(c => c.Key == "enumNames").Value.Split(",");
        var numericValues = description.Values.First(c => c.Key == "enumValues").Value.Split(",");

        propertySchema.Enum = fieldType switch
        {
            "enum"
                => new List<IOpenApiAny>(
                    stringValues.Concat(numericValues).Select(v => new OpenApiString(v))
                ),
            "string" => new List<IOpenApiAny>(stringValues.Select(v => new OpenApiString(v))),
            "numeric" => new List<IOpenApiAny>(numericValues.Select(v => new OpenApiString(v))),
            _ => new List<IOpenApiAny>()
        };
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
