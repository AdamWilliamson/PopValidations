using Microsoft.OpenApi.Models;
using PopApiValidations.Swashbuckle.Converters;
using PopValidations.Execution.Validations;
using PopValidations.Swashbuckle.Converters;
using PopValidations.Swashbuckle.Internal;

namespace PopApiPopValidations.Swashbuckle.Converters;

public class PopApiIsGreaterThanValidationToOpenApiConverter : IsGreaterThanValidationToOpenApiConverter, IPopApiValidationToOpenApiConverter
{
    public void UpdateAttribute(OpenApiOperation owningObjectSchema, OpenApiSchema paramSchema, string paramName, DescriptionOutcome description, PopValidationArray attributeDescription)
    {
        attributeDescription.Add(description.Message);
    }

    public void UpdateParamSchema(
        OpenApiOperation owningObjectSchema,
        OpenApiParameter paramSchema,
        string paramName,
        DescriptionOutcome description)
    {
        var value = description.Values.FirstOrDefault(x => x.Key == "value").Value;
        if (decimal.TryParse(value, out var decimalValue))
        {
            paramSchema.Schema.Minimum = decimalValue;
            paramSchema.Schema.ExclusiveMinimum = true;
        }
    }

    public void UpdateParamArraySchema(
        OpenApiOperation owningObjectSchema,
        OpenApiSchema itemSchema,
        string paramName,
        DescriptionOutcome description
    )
    {
        var value = description.Values.FirstOrDefault(x => x.Key == "value").Value;
        if (decimal.TryParse(value, out var decimalValue))
        {
            itemSchema.Minimum = decimalValue;
            itemSchema.ExclusiveMinimum = true;
        }
    }

    public void UpdateRequestBodySchema(OpenApiRequestBody owningObjectSchema, OpenApiSchema paramSchema, string paramName, DescriptionOutcome description)
    {
        var value = description.Values.FirstOrDefault(x => x.Key == "value").Value;
        if (decimal.TryParse(value, out var decimalValue))
        {
            paramSchema.Minimum = decimalValue;
            paramSchema.ExclusiveMinimum = true;
        }
    }
}
