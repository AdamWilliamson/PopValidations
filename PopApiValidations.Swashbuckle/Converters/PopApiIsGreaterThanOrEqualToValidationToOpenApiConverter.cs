using Microsoft.OpenApi.Models;
using PopApiValidations.Swashbuckle.Converters;
using PopValidations.Execution.Validations;
using PopValidations.Swashbuckle.Converters;
using PopValidations.Swashbuckle.Internal;

namespace PopApiPopValidations.Swashbuckle.Converters;

public class PopApiIsGreaterThanOrEqualToValidationToOpenApiConverter : IsGreaterThanOrEqualToValidationConverter, IPopApiValidationToOpenApiConverter
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
        //var value = description.Values.FirstOrDefault(x => x.Key == "value").Value;
        //if (decimal.TryParse(value, out var decimalValue))
        //{
        //    schema.Minimum = decimalValue;
        //    schema.ExclusiveMinimum = false;
        //}
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
            itemSchema.ExclusiveMinimum = false;
        }
    }

    public void UpdateRequestBodySchema(OpenApiRequestBody owningObjectSchema, OpenApiSchema paramSchema, string paramName, DescriptionOutcome description)
    {
        var value = description.Values.FirstOrDefault(x => x.Key == "value").Value;
        if (decimal.TryParse(value, out var decimalValue))
        {
            paramSchema.Minimum = decimalValue;
            paramSchema.ExclusiveMinimum = false;
        }
    }
}