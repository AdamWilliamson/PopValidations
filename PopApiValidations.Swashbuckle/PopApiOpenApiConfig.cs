using Microsoft.OpenApi.Models;
using PopValidations.Execution.Validations;
using PopValidations.Swashbuckle;
using PopValidations.Swashbuckle.Internal;
using PopValidations.Validations;
using System.Reflection;

namespace PopApiValidations.Swashbuckle;

public interface IPopApiValidationToOpenApiConverter 
{
    bool Supports(DescriptionOutcome outcome);

    void UpdateParamSchema(
        OpenApiOperation? owningObjectSchema,
        OpenApiParameter parameterSchema,
        string paramName,
        DescriptionOutcome description
    );

    void UpdateRequestBodySchema(
        OpenApiRequestBody? owningObjectSchema,
        OpenApiSchema paramSchema,
        string paramName,
        DescriptionOutcome description
    );

    void UpdateAttribute(
        OpenApiOperation? owningObjectSchema,
        OpenApiSchema paramSchema,
        string paramName,
        DescriptionOutcome description,
        PopValidationArray attributeDescription
    );
}

public class PopApiIsNotNullValidationToOpenApiConverter : IPopApiValidationToOpenApiConverter
{
    public bool Supports(DescriptionOutcome outcome)
    {
        return outcome.Validator == nameof(IsNotNullValidation);
    }

    public void UpdateParamSchema(
        OpenApiOperation? owningObjectSchema, 
        OpenApiParameter paramSchema, 
        string paramName, 
        DescriptionOutcome description)
    {
        if (paramSchema is not null)
        {
            paramSchema.Required = true;
        }
    }

    public void UpdateRequestBodySchema(
        OpenApiRequestBody? owningObjectSchema, 
        OpenApiSchema paramSchema, 
        string paramName, 
        DescriptionOutcome description)
    {
        if (owningObjectSchema is not null)
        {
            owningObjectSchema.Required = true;
        }
    }

    public void UpdateAttribute(
        OpenApiOperation? owningObjectSchema,
        OpenApiSchema propertySchema,
        string property,
        DescriptionOutcome description,
        PopValidationArray attributeDescription
    )
    {
        attributeDescription.Add(description.Message);
    }
}

public class PopApiOpenApiConfig : OpenApiConfig
{
    public List<IPopApiValidationToOpenApiConverter> PopApiConverters = new()
    {
        new PopApiIsNotNullValidationToOpenApiConverter()
    };

    public Func<MethodInfo, bool>? ValidateEndpoint { get; set; } = null;
}
