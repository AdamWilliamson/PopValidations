using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using PopValidations.Execution.Validations;
using PopValidations.Swashbuckle.Converters;
using PopValidations.Swashbuckle.Internal;

namespace PopApiValidations.Swashbuckle.Converters;

public class PopApiIsNotNullValidationToOpenApiConverter : IsNotNullValidationToOpenApiConverter, IPopApiValidationToOpenApiConverter
{
    public void UpdateParamSchema(
        OpenApiOperation owningObjectSchema,
        OpenApiParameter paramSchema,
        string paramName,
        DescriptionOutcome description)
    {
        paramSchema.Required = true;
    }

    public void UpdateRequestBodySchema(
        OpenApiRequestBody owningObjectSchema,
        OpenApiSchema paramSchema,
        string paramName,
        DescriptionOutcome description)
    {
        owningObjectSchema.Required = true;
    }

    public void UpdateParamArraySchema(
    OpenApiOperation owningObjectSchema,
    OpenApiSchema itemSchema,
    string paramName,
    DescriptionOutcome description
)
    {
        //itemSchema.Required.Add(paramName);
        itemSchema.Nullable = false;
    }

    public void UpdateAttribute(
        OpenApiOperation owningObjectSchema,
        OpenApiSchema propertySchema,
        string property,
        DescriptionOutcome description,
        PopValidationArray attributeDescription
    )
    {
        attributeDescription.Add(description.Message);
    }
}
