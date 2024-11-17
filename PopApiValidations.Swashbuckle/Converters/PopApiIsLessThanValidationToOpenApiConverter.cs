using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Linq;
using PopApiValidations.Swashbuckle.Converters;
using PopValidations.Execution.Validations;
using PopValidations.Swashbuckle.Converters;
using PopValidations.Swashbuckle.Converters.Base;
using PopValidations.Swashbuckle.Helpers;
using PopValidations.Swashbuckle.Internal;
using PopValidations.Validations;

namespace PopApiPopValidations.Swashbuckle.Converters;

public class PopApiIsLessThanValidationToOpenApiConverter : IsLessThanValidationToOpenApiConverter, IPopApiValidationToOpenApiConverter
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
            paramSchema.Schema.Maximum = decimalValue;
            paramSchema.Schema.ExclusiveMaximum = true;
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
            itemSchema.Maximum = decimalValue;
            itemSchema.ExclusiveMaximum = true;
        }
    }

    public void UpdateRequestBodySchema(OpenApiRequestBody owningObjectSchema, OpenApiSchema paramSchema, string paramName, DescriptionOutcome description)
    {
        var value = description.Values.FirstOrDefault(x => x.Key == "value").Value;
        if (decimal.TryParse(value, out var decimalValue))
        {
            paramSchema.Maximum = decimalValue;
            paramSchema.ExclusiveMaximum = true;
        }
    }
}