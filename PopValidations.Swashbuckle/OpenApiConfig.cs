using PopValidations.Swashbuckle.Converters;
using PopValidations.Swashbuckle.Converters.Base;
using System.Reflection;

namespace PopValidations.Swashbuckle;

[Flags]
public enum ValidationLevel
{
    None = 0,
    OpenApi = 1,
    ValidationAttribute = 2,
    ValidationAttributeInBase = 4,
    FullDetails = OpenApi | ValidationAttribute,
}

public class OpenApiConfig
{
    public string CustomValidationAttribute { get; set; } = "x-validation";
    public string IndentCharacter { get; set; } = "\t";
    public string NewLine { get; set; } = Environment.NewLine;
    public string OrdinalIndicator { get; set; } = "[n]";
    public string ChildIndicator { get; set; } = ".";

    public List<IValidationToOpenApiConverter> Converters = new()
    {
        new NotNullValidationToOpenApiConverter(),
        new IsEnumValidationToOpenApiConverter(),
        new IsEqualToValidationToOpenApiConverter(),
        new IsNullValidationToOpenApiConverter(),
        new IsLengthInclusivelyBetweenValidationToOpenApiConverter(),
        new IsLengthExclusivelyBetweenValidationToOpenApiConverter(),
        new IsGreaterThanValidationToOpenApiConverter(),
        new IsGreaterThanOrEqualToValidationToOpenApiConverter(),
        new IsLessThanValidationToOpenApiConverter(),
        new IsLessThanOrEqualToValidationToOpenApiConverter(),
        new IsEmptyValidationToOpenApiConverter(),
        new IsNotEmptyValidationToOpenApiConverter()
    };

    public Func<Type, ValidationLevel> TypeValidationLevel { get; set; } 
        = (Type t) => ValidationLevel.FullDetails;

    public Func<string, string, bool> ObjectPropertyIsJsonProperty { get; set; }
        = (string obj, string json) => obj.Equals(json, StringComparison.OrdinalIgnoreCase);

    public Func<string, string, bool> ObjectPropertyIsDescriptorArray { get; set; }

    public OpenApiConfig()
    {
        ObjectPropertyIsDescriptorArray
            = (string obj, string json) => obj.Equals(json + OrdinalIndicator, StringComparison.OrdinalIgnoreCase);
    }

    public void AllClassesAreReused()
    {
        TypeValidationLevel = (Type t) => ValidationLevel.ValidationAttributeInBase;
    }

    public PropertyInfo? GetPropertyFromType(Type src, string propName)
    {
        return src.GetProperty(
                propName,
                System.Reflection.BindingFlags.IgnoreCase
                    | System.Reflection.BindingFlags.Public
                    | System.Reflection.BindingFlags.Instance
                    | System.Reflection.BindingFlags.FlattenHierarchy
            );
    }
}
