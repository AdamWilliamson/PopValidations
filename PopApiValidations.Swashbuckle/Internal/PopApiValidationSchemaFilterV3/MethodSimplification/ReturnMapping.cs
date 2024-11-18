namespace PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.MethodSimplification;

public class ReturnMapping
{
    public Type ReturnType { get; set; }
    public bool IsArrayType { get; set; }
    public List<PropertyMapping> Properties { get; set; } = new();

    public List<(string, PropertyMapping?)> GetOpenApiPropertyNames()
    {
        List<(string, PropertyMapping?)> result = new();
        var prefix = "Response";

        if (!Properties.Any())
        {
            result.Add((prefix, null));
        }

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
