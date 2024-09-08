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
    public string MultiGroupIndicator { get; set; } = " & ";
    public string GroupResultIndicator { get; set; } = " : ";

    public List<IValidationToOpenApiConverter> Converters = new()
    {
        new IsNotNullValidationToOpenApiConverter(),
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
        new IsNotEmptyValidationToOpenApiConverter(),
        new IsCustomValidationToOpenApiConverter(),
        new IsEmailValidationToOpenApiConverter(),
    };

    public Func<Type, ValidationLevel> TypeValidationLevel { get; set; } 
        = (Type t) => ValidationLevel.FullDetails;

    public Func<string, string, bool> ObjectPropertyIsJsonProperty { get; set; }
        = (string obj, string json) => obj.Equals(json, StringComparison.OrdinalIgnoreCase);

    public Func<string, string, bool> ObjectPropertyIsDescriptorArray { get; set; }

    public Func<Type, bool> IsGenericList { get; set; }

    public Func<OpenApiConfig, Type?, string, Type?> GetPropertyType { get; set; }

    public OpenApiConfig()
    {
        ObjectPropertyIsDescriptorArray
            = (string obj, string json) => obj.Equals(json + OrdinalIndicator, StringComparison.OrdinalIgnoreCase);

        IsGenericList = (Type oType) =>
        {
            Console.WriteLine(oType.FullName);
            if (oType.IsGenericType)
            {
                var genDef = oType.GetGenericTypeDefinition();

                if (genDef.GetGenericTypeDefinition() == typeof(IEnumerable<>)) return true;

                if (genDef.GetInterface(typeof(IEnumerable<>).Name) != null) return true;

                return false;
            }
            return false;
        };

        GetPropertyType = (OpenApiConfig config, Type? src, string propName) =>
        {
            if (src == null) throw new ArgumentException("Value cannot be null.", "src");
            if (string.IsNullOrWhiteSpace(propName)) return src;

            var realProp = propName.Split(new char[] { '.' }).Last();

            if (realProp.Contains(config.OrdinalIndicator))
            {
                var result = config.GetPropertyFromType(src, realProp.Replace(config.OrdinalIndicator, ""));
                if (result is not null && (config.IsGenericList?.Invoke(result.PropertyType) ?? false))
                {
                    return result.PropertyType.GetGenericArguments().FirstOrDefault();
                }
                return null;
            }
            else
            {
                var result = config.GetPropertyFromType(src, realProp);

                return result?.PropertyType;
            }
        };
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
