using System.Reflection;
using Microsoft.OpenApi.Models;

namespace PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.MethodSimplification;

public class ParameterMapping
{
    public ParameterInfo ParameterInfo { get; set; }
    public bool IsOpenApiRequestBody { get; set; }
    public string? OpenApiParameterName { get; set; }
    public bool IsArrayType { get; set; }
    public ParameterLocation? Location { get; set; } = null; // Default value is Unknown
    public List<PropertyMapping> Properties { get; set; } = new();
    public List<(string, PropertyMapping?)> GetOpenApiPropertyNames()
    {
        List<(string, PropertyMapping?)> result = new();
        var prefix = string.Empty;

        //var paramLocation = new[] { ParameterLocation.Query, ParameterLocation.Path, ParameterLocation.Header  };

        if (IsOpenApiRequestBody)
        {
            prefix = "RequestBody";
        }
        //else if (Location != null)
        //{
        //    prefix = ParameterInfo.Name;
        //}

        if (!Properties.Any())
        {
            result.Add((OpenApiParameterName, null));
            //prefix = OpenApiParameterName;
            prefix = ParameterInfo.Name;
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
