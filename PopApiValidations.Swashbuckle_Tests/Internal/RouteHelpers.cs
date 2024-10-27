using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Reflection;

namespace PopApiValidations.Swashbuckle_Tests.Internal;

public static class RouteHelpers {
    public static bool IsComplexType(Type type)
    {
        return !type.IsPrimitive && type != typeof(string) && !type.IsValueType;
    }

    public static string GetAlternateName(ParameterInfo paramInfo)
    {
        var urlAttr = paramInfo.GetCustomAttribute<FromRouteAttribute>();
        if (urlAttr is not null && !string.IsNullOrWhiteSpace(urlAttr.Name))
        {
            return urlAttr.Name;
        }
        else if (urlAttr is not null)
        {
            return paramInfo.Name ?? string.Empty;
        }

        var bodyAttr = paramInfo.GetCustomAttribute<FromBodyAttribute>();
        if (bodyAttr is not null)
        {
            return string.Empty;
        }

        var queryAttr = paramInfo.GetCustomAttribute<FromQueryAttribute>();
        if (queryAttr is not null && !string.IsNullOrWhiteSpace(queryAttr.Name))
        {
            return queryAttr.Name;
        }
        else if (queryAttr is not null)
        {
            return paramInfo.Name ?? string.Empty;
        }

        var headerAttr = paramInfo.GetCustomAttribute<FromHeaderAttribute>();
        if (headerAttr is not null && !string.IsNullOrWhiteSpace(headerAttr.Name))
        {
            return headerAttr.Name;
        }
        else if (headerAttr is not null)
        {
            return paramInfo.Name ?? string.Empty;
        }

        var formAttr = paramInfo.GetCustomAttribute<FromFormAttribute>();
        if (formAttr is not null)
        {
            return string.Empty;
        }

        if (IsComplexType(paramInfo.ParameterType))
        {
            return string.Empty;
        }

        return paramInfo.Name ?? string.Empty;
    }

    public static string GetAlternateName(PropertyInfo property)
    {
        var json = property.GetCustomAttribute<JsonPropertyAttribute>();
        if (json is not null && !string.IsNullOrWhiteSpace(json.PropertyName))
        {
            return json.PropertyName;
        }
        
        return property.Name;
    }

    public static ParameterLocation? GetParameterLocation(ParameterInfo paramInfo)
    {
        var urlAttr = paramInfo.GetCustomAttribute<FromRouteAttribute>();
        if (urlAttr is not null)
        {
            return ParameterLocation.Path;
        }

        var bodyAttr = paramInfo.GetCustomAttribute<FromBodyAttribute>();
        if (bodyAttr is not null)
        {
            return null;
        }

        var queryAttr = paramInfo.GetCustomAttribute<FromQueryAttribute>();
        if (queryAttr is not null)
        {
            return ParameterLocation.Query;
        }

        var headerAttr = paramInfo.GetCustomAttribute<FromHeaderAttribute>();
        if (headerAttr is not null)
        {
            return ParameterLocation.Header;
        }
        
        var formAttr = paramInfo.GetCustomAttribute<FromFormAttribute>();
        if (formAttr is not null)
        {
            return null;
        }

        if (IsComplexType(paramInfo.ParameterType))
        {
            return null;
        }

        return ParameterLocation.Path;
    }
}
