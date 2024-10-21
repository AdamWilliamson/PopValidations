using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using PopApiValidations.Swashbuckle.Converters;
using PopValidations.Execution.Validations;
using PopValidations.Swashbuckle.Converters;
using PopValidations.Swashbuckle.Converters.Base;
using PopValidations.Swashbuckle.Helpers;
using PopValidations.Swashbuckle.Internal;
using PopValidations.Validations;

namespace PopApiPopValidations.Swashbuckle.Converters;

public class PopApiIsLengthExclusivelyBetweenValidationToOpenApiConverter : IsLengthExclusivelyBetweenValidationToOpenApiConverter, IPopApiValidationToOpenApiConverter
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
            parameterSchema.Schema.MinLength = start + 1;
            parameterSchema.Schema.MaxLength = end - 1;
        }   
    }

    public void UpdateRequestBodySchema(OpenApiRequestBody owningObjectSchema, OpenApiSchema paramSchema, string paramName, DescriptionOutcome description)
    {
        var startValue = description.Values.FirstOrDefault(x => x.Key == "startValue").Value;
        var endValue = description.Values.FirstOrDefault(x => x.Key == "endValue").Value;

        if (int.TryParse(startValue, out var start) && int.TryParse(endValue, out var end))
        {
            paramSchema.MinLength = start + 1;
            paramSchema.MaxLength = end - 1;
        }
    }
}
