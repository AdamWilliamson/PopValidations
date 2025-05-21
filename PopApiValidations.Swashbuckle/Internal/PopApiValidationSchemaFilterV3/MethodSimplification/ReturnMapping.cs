using System.Reflection;

namespace PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.MethodSimplification;

public class ReturnMapping : IGeneralMapping
{
    public Type Type => ReturnType;
    public string Name => string.Empty;
    public string OpenApiName => string.Empty;
    public string? ResultName => null;
    public OpenApiLocation MappingType => OpenApiLocation.Return;


    public required Type ReturnType { get; set; }
    public bool IsArrayType { get; set; }
    public List<PropertyMapping> Properties { get; set; } = new();


    private List<(string, PropertyMapping?)> result = new();
    public List<(string, PropertyMapping?)> GetOpenApiPropertyNames()
    {
        if (result.Any()) return result;

        var prefix = "Response";

        if (!Properties.Any())
        {
            result.Add((prefix, null));
        }

        result.Add((prefix, new PropertyMapping
        {
            IsArrayType = IsArrayType,
            PropertyType = ReturnType,
            PropertyName = string.Empty,
            OpenApiPropertyName = prefix,
            ResultPropertyName = null,
            Properties = Properties,
        }));

        foreach (var property in Properties)
        {
            var newPrefix = string.IsNullOrEmpty(prefix)
                ? property.OpenApiPropertyName
                : prefix + "." + property.OpenApiPropertyName;

            result.Add((newPrefix, property));
            result.AddRange(GetRecursive(property, newPrefix));
        }

        return result;
    }

    private static List<(string, PropertyMapping?)> GetRecursive(PropertyMapping mapping, string prefix)
    {
        List<(string, PropertyMapping?)> result = new();

        foreach (var property in mapping.Properties)
        {
            var newPrefix = prefix + "." + property.OpenApiPropertyName;
            result.Add((newPrefix, property));
            result.AddRange(GetRecursive(property, newPrefix));
        }

        return result;
    }
}
