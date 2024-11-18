using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.Helpers;

public static class PropertyInfoHelper
{
    public static string GetOpenApiPropertyName(PropertyInfo property)
    {
        var jsonPropertyName = property.GetCustomAttribute<JsonPropertyNameAttribute>();
        if (jsonPropertyName != null)
        {
            return jsonPropertyName.Name;
        }
        return property.Name; // Default to the property name
    }
}
