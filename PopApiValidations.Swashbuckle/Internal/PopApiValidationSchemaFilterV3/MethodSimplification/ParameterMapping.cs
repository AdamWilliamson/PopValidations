using System.Reflection;
using Microsoft.OpenApi.Models;

namespace PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.MethodSimplification;

/// </summary>
public enum OpenApiLocation
{
    Query,
    Header,
    Path,
    Cookie,
    Return,
    ResponseBody,
    Form
}

public interface IGeneralMapping
{
    public bool IsArrayType { get; }
    public Type Type { get; }
    public List<PropertyMapping> Properties { get; }

    public string Name { get; }
    public string OpenApiName { get; }
    public string? ResultName { get; }
    public OpenApiLocation MappingType { get; }

    public List<(string, PropertyMapping?)> GetOpenApiPropertyNames();
}

public class ParameterMapping : IGeneralMapping
{
    public required ParameterInfo ParameterInfo { get; set; }
    public Type Type => ParameterInfo.ParameterType;
    public bool IsOpenApiRequestBody { get; set; }
    public string? OpenApiParameterName { get; set; }
    public bool IsArrayType { get; set; }
    public ParameterLocation? Location { get; set; } = null; // Default value is Unknown

    public string Name => ParameterInfo.Name ?? string.Empty;
    public string OpenApiName => OpenApiParameterName;
    public string? ResultName => (string.Equals(this.ParameterInfo.Name,OpenApiName,StringComparison.InvariantCultureIgnoreCase))? string.Empty : OpenApiParameterName;
    public required OpenApiLocation MappingType { get; set; }

    public List<PropertyMapping> Properties { get; set; } = new();

    private bool useName()
    {
        if (IsOpenApiRequestBody) return false;

        if (!Properties.Any()) return true;

        if (Location != ParameterLocation.Query) return true;

        if (Location == ParameterLocation.Query && IsArrayType) return true;

        return false;
    }

    List<(string, PropertyMapping?)> result = new();
    public List<(string, PropertyMapping?)> GetOpenApiPropertyNames()
    {
        if (result.Any()) return result;

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

        result.Add(
            (
                string.IsNullOrWhiteSpace(prefix)? OpenApiParameterName : prefix + "." + OpenApiParameterName, 
                new PropertyMapping
                {
                    IsArrayType = IsArrayType,
                    PropertyType = ParameterInfo.ParameterType,
                    PropertyName = ParameterInfo.Name,
                    OpenApiPropertyName = OpenApiParameterName,
                    ResultPropertyName = ResultName,
                    Properties = Properties,
                }
            ));

        if (useName())
        {
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
