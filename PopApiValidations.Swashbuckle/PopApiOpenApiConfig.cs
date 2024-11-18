using PopApiPopValidations.Swashbuckle.Converters;
using PopApiValidations.Swashbuckle.Converters;
using PopValidations.Swashbuckle;
using System.Reflection;

namespace PopApiValidations.Swashbuckle;

public class PopApiOpenApiConfig : OpenApiConfig
{
    public List<IPopApiValidationToOpenApiConverter> PopApiConverters = new()
    {
        new PopApiIsNotNullValidationToOpenApiConverter(),
        new PopApiIsEnumValidationToOpenApiConverter(),
        new PopApiIsEqualToValidationToOpenApiConverter(),
        new PopApiIsNullValidationToOpenApiConverter(),
        new PopApiIsLengthInclusivelyBetweenValidationToOpenApiConverter(),
        new PopApiIsLengthExclusivelyBetweenValidationToOpenApiConverter(),
        new PopApiIsGreaterThanValidationToOpenApiConverter(),
        new PopApiIsGreaterThanOrEqualToValidationToOpenApiConverter(),
        new PopApiIsLessThanValidationToOpenApiConverter(),
        new PopApiIsLessThanOrEqualToValidationToOpenApiConverter(),
        new PopApiIsEmptyValidationToOpenApiConverter(),
        new PopApiIsNotEmptyValidationToOpenApiConverter(),
        new PopApiIsCustomValidationToOpenApiConverter(),
        new PopApiIsEmailValidationToOpenApiConverter(),
    };

    public Func<MethodInfo, bool>? ValidateEndpoint { get; set; } = null;
}
