using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.Helpers;

public static class MethodHelper
{
    public static string GetOpenApiOperation(MethodInfo method)
    {
        if (method.GetCustomAttribute<HttpGetAttribute>() != null) return "GET";
        if (method.GetCustomAttribute<HttpPostAttribute>() != null) return "POST";
        if (method.GetCustomAttribute<HttpPutAttribute>() != null) return "PUT";
        if (method.GetCustomAttribute<HttpDeleteAttribute>() != null) return "DELETE";
        if (method.GetCustomAttribute<HttpPatchAttribute>() != null) return "PATCH";
        return "UNKNOWN"; // Default
    }
}
