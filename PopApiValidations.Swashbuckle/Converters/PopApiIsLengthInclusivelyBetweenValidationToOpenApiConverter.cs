using Microsoft.OpenApi.Models;
using PopApiValidations.Swashbuckle.Converters;
using PopValidations.Execution.Validations;
using PopValidations.Swashbuckle.Converters;
using PopValidations.Swashbuckle.Internal;

namespace PopApiPopValidations.Swashbuckle.Converters;

public class PopApiIsLengthInclusivelyBetweenValidationToOpenApiConverter : IsLengthInclusivelyBetweenValidationToOpenApiConverter, IPopApiValidationToOpenApiConverter
{
    public void UpdateAttribute(OpenApiOperation owningObjectSchema, OpenApiSchema paramSchema, string paramName, DescriptionOutcome description, PopValidationArray attributeDescription)
    {
        attributeDescription.Add(description.Message);
    }

    public void UpdateParamSchema(OpenApiOperation owningObjectSchema, OpenApiParameter parameterSchema, string paramName, DescriptionOutcome description)
    {
        var startValue = description.Values.FirstOrDefault(x => x.Key == "startValue").Value;
        var endValue = description.Values.FirstOrDefault(x => x.Key == "endValue").Value;

        if (int.TryParse(startValue, out var start) && int.TryParse(endValue, out var end))
        {
            parameterSchema.Schema.MinLength = start;
            parameterSchema.Schema.MaxLength = end;
        }
    }

    public void UpdateRequestBodySchema(OpenApiRequestBody owningObjectSchema, OpenApiSchema paramSchema, string paramName, DescriptionOutcome description)
    {
        var startValue = description.Values.FirstOrDefault(x => x.Key == "startValue").Value;
        var endValue = description.Values.FirstOrDefault(x => x.Key == "endValue").Value;

        if (int.TryParse(startValue, out var start) && int.TryParse(endValue, out var end))
        {
            paramSchema.MinLength = start;
            paramSchema.MaxLength = end;
        }
    }
}
