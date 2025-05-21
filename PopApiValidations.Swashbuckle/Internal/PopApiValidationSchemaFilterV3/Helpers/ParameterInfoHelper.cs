using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.MethodSimplification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.Helpers;

public static class ParameterInfoHelper
{
    public static ParameterLocation? GetParameterLocation(ParameterInfo parameter)
    {
        // First check for attributes that specify parameter location
        if (parameter.GetCustomAttribute<FromBodyAttribute>() != null) return null;
        if (parameter.GetCustomAttribute<FromQueryAttribute>() != null) return ParameterLocation.Query;
        if (parameter.GetCustomAttribute<FromHeaderAttribute>() != null) return ParameterLocation.Header;
        if (parameter.GetCustomAttribute<FromRouteAttribute>() != null) return ParameterLocation.Path;
        if (parameter.GetCustomAttribute<FromFormAttribute>() != null) return null;
        if (TypeHelper.IsComplexOrEnumerable(parameter.ParameterType)) return null;

        // If it's a simple type and the route contains {parameterName}, classify it as Route
        if (TypeHelper.IsSimpleType(parameter.ParameterType) && IsRouteParameterInUrl(parameter))
        {
            return ParameterLocation.Path;
        }

        // Default to Unknown if no attribute and no matching conditions are found
        return null;
    }

    public static OpenApiLocation GetParameterLocation2(ParameterInfo parameter)
    {
        // First check for attributes that specify parameter location
        if (parameter.GetCustomAttribute<FromBodyAttribute>() != null) return OpenApiLocation.ResponseBody;
        if (parameter.GetCustomAttribute<FromQueryAttribute>() != null) return OpenApiLocation.Query;
        if (parameter.GetCustomAttribute<FromHeaderAttribute>() != null) return OpenApiLocation.Header;
        if (parameter.GetCustomAttribute<FromRouteAttribute>() != null) return OpenApiLocation.Path;
        if (parameter.GetCustomAttribute<FromFormAttribute>() != null) return OpenApiLocation.Form;
        //if (TypeHelper.IsComplexOrEnumerable(parameter.ParameterType)) return null;

        // If it's a simple type and the route contains {parameterName}, classify it as Route
        if (TypeHelper.IsSimpleType(parameter.ParameterType) && IsRouteParameterInUrl(parameter))
        {
            return OpenApiLocation.Path;
        }

        // Default to Unknown if no attribute and no matching conditions are found
        return OpenApiLocation.ResponseBody;
    }

    public static bool IsRouteParameterInUrl(ParameterInfo parameter)
    {
        // Check if the parameter is part of a route URL defined by either RouteAttribute or HTTP method attributes
        var methodInfo = parameter.Member as MethodInfo;
        if (methodInfo != null)
        {
            // Check for RouteAttribute
            var routeAttribute = methodInfo.GetCustomAttribute<RouteAttribute>();
            if (routeAttribute != null)
            {
                // Check if the route contains {parameterName}
                if (routeAttribute.Template.Contains($"{{{parameter.Name}}}", StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            // If RouteAttribute is not present, check HTTP method-specific attributes for a route
            var httpMethodAttributes = new List<Attribute>
            {
                methodInfo.GetCustomAttribute<HttpGetAttribute>(),
                methodInfo.GetCustomAttribute<HttpPostAttribute>(),
                methodInfo.GetCustomAttribute<HttpPutAttribute>(),
                methodInfo.GetCustomAttribute<HttpDeleteAttribute>(),
                methodInfo.GetCustomAttribute<HttpPatchAttribute>(),
                methodInfo.GetCustomAttribute<HttpOptionsAttribute>(),
                methodInfo.GetCustomAttribute<HttpHeadAttribute>()
            };

            foreach (var httpMethodAttribute in httpMethodAttributes.Where(attr => attr != null))
            {
                // Get the template/route defined by the HTTP method attribute
                var routeTemplate = httpMethodAttribute.GetType().GetProperty("Template")?.GetValue(httpMethodAttribute)?.ToString();
                if (!string.IsNullOrEmpty(routeTemplate) && routeTemplate.Contains($"{{{parameter.Name}}}", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
        }
        return false;
    }
}
