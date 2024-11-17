using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using PopApiValidations.Swashbuckle.Converters;
using PopValidations.Execution.Validations;
using PopValidations.Swashbuckle.Converters;
using PopValidations.Swashbuckle.Internal;

namespace PopApiPopValidations.Swashbuckle.Converters;

public class PopApiIsEmptyValidationToOpenApiConverter : IsEmptyValidationToOpenApiConverter, IPopApiValidationToOpenApiConverter
{
    public void UpdateAttribute(OpenApiOperation owningObjectSchema, OpenApiSchema paramSchema, string paramName, DescriptionOutcome description, PopValidationArray attributeDescription)
    {
        attributeDescription.Add(description.Message);
    }

    public void UpdateParamSchema(OpenApiOperation owningObjectSchema, OpenApiParameter parameterSchema, string paramName, DescriptionOutcome description)
    {
        parameterSchema.Schema.MaxLength = 0;
        parameterSchema.Schema.MaxItems = 0;
    }

    public void UpdateParamArraySchema(
        OpenApiOperation owningObjectSchema,
        OpenApiSchema itemSchema,
        string paramName,
        DescriptionOutcome description
    )
    {
        itemSchema.MaxLength = 0;
        itemSchema.MaxItems = 0;
    }

    public void UpdateRequestBodySchema(OpenApiRequestBody owningObjectSchema, OpenApiSchema paramSchema, string paramName, DescriptionOutcome description)
    {
        paramSchema.MaxLength = 0;
        paramSchema.MaxItems = 0;
    }
}