//using Microsoft.AspNetCore.Mvc;
//using Microsoft.OpenApi.Models;
//using System.Reflection;
//using System.Text.Json.Serialization;

//public class PropertyMapping
//{
//    public Type PropertyType { get; set; }
//    public string PropertyName { get; set; }  // Retain the property name
//    public string OpenApiPropertyName { get; set; }  // Now it's just a string to represent the OpenAPI path
//    public string? Result { get; set; }  // New field for holding additional result, like the final OpenAPI property path
//    public List<PropertyMapping> Properties { get; set; } = new();
//    public bool IsArrayType { get; set; } = false;
//}

//public class ParameterMapping
//{
//    public ParameterInfo ParameterInfo { get; set; }
//    public bool IsOpenApiRequestBody { get; set; }
//    public string? OpenApiParameterName { get; set; }
//    public string? ResultParameterName { get; set; }
//    public bool IsArrayType { get; set; }
//    public ParameterLocation? Location { get; set; } = null; // Default value is Unknown
//    public List<PropertyMapping> Properties { get; set; } = new();

//    public List<string> GetOpenApiPropertyNames()
//    {
//        List<string> result = new();
//        var prefix = string.Empty;

//        if (!Properties.Any())
//        {
//            result.Add(OpenApiParameterName);
//            prefix = OpenApiParameterName;
//        }

//        foreach (var property in Properties)
//        {
//            var newPrefix = string.IsNullOrEmpty(prefix)
//                ? property.OpenApiPropertyName
//                : prefix + "." + property.OpenApiPropertyName;

//            result.Add(property.OpenApiPropertyName);
//            result.AddRange(GetRecursive(property, newPrefix));
//        }

//        return result;
//    }

//    private static List<string> GetRecursive(PropertyMapping mapping, string prefix)
//    {
//        List<string> result = new();

//        foreach (var property in mapping.Properties)
//        {
//            var newPrefix = prefix + "." + property.OpenApiPropertyName;
//            result.Add(newPrefix);
//            result.AddRange(GetRecursive(property, newPrefix));
//        }

//        return result;
//    }
//}

//public class FunctionMapping
//{
//    public MethodInfo MethodInfo { get; set; }
//    public string OpenApiPath { get; set; }
//    public string OpenApiOperation { get; set; }
//    public List<ParameterMapping> Parameters { get; set; } = new();
//}

//public class TypeToMapping
//{
//    public List<FunctionMapping> CreateMappings(Type controllerType)
//    {
//        var functionMappings = new List<FunctionMapping>();

//        // Get all methods of the controller
//        var methods = GetControllerMethods(controllerType);  // Get the public methods of the controller

//        foreach (var method in methods)
//        {
//            // Skip methods that are marked with NonAction
//            if (method.GetCustomAttribute<NonActionAttribute>() != null)
//                continue;

//            var functionMapping = GetMethodMap(method);

//            // Add the function mapping to the list of function mappings
//            functionMappings.Add(functionMapping);
//        }

//        return functionMappings;
//    }

//    public FunctionMapping GetMethodMap(MethodInfo method)
//    {
//        var functionMapping = new FunctionMapping
//        {
//            MethodInfo = method,
//            OpenApiOperation = GetOpenApiOperation(method)
//        };

//        // Process parameters for each method
//        var parameters = method.GetParameters();

//        foreach (var parameter in parameters)
//        {
//            var parameterMapping = new ParameterMapping
//            {
//                ParameterInfo = parameter,
//                OpenApiParameterName = GetOpenApiParameterName(parameter),
//                IsArrayType = IsArrayType(parameter.ParameterType),
//                Location = GetParameterLocation(parameter) // Determine the location of the parameter
//            };

//            // Determine if the parameter is a simple type or a complex type
//            if (IsSimpleType(parameter.ParameterType))
//            {
//                // Simple type: No need for body mapping, just add it directly
//                functionMapping.Parameters.Add(parameterMapping);
//            }
//            else
//            {
//                // Complex type: Consider it part of the body and map its properties recursively
//                if (parameterMapping.Location is null)
//                {
//                    parameterMapping.IsOpenApiRequestBody = true; // Mark as request body parameter

//                    // Recursively get property mappings for the complex type
//                    var propertyMappings = GetPropertyMappings(parameter.ParameterType, parameterMapping.OpenApiParameterName ?? parameter.Name);
//                    foreach (var propertyMapping in propertyMappings)
//                    {
//                        // Add the property mappings to the main parameter mapping
//                        parameterMapping.Properties.Add(propertyMapping);
//                    }
//                }
//                else
//                {
//                    // Recursively get property mappings for the complex type
//                    var propertyMappings = GetPropertyMappings(parameter.ParameterType, "");
//                    foreach (var propertyMapping in propertyMappings)
//                    {
//                        // Add the property mappings to the main parameter mapping
//                        parameterMapping.Properties.Add(propertyMapping);
//                    }
//                }

//                // Add the parameter mapping to the function (even if it’s complex)
//                functionMapping.Parameters.Add(parameterMapping);
//            }
//        }

//        return functionMapping;
//    }

//    public List<MethodInfo> GetControllerMethods(Type controllerType)
//    {
//        // A list to store methods that could become URLs
//        var validMethods = new List<MethodInfo>();

//        // Get all public instance methods from the controller
//        var methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance);

//        foreach (var method in methods)
//        {
//            // Skip methods that are non-action methods or are related to view rendering
//            if (IsNonActionMethod(method) || IsViewDataRelatedMethod(method))
//                continue;

//            // Add the method to the list, as it could potentially be mapped to a route
//            validMethods.Add(method);
//        }

//        return validMethods;
//    }

//    private bool IsNonActionMethod(MethodInfo method)
//    {
//        // Methods with [NonAction] attribute are not meant to be URL endpoints
//        if (method.GetCustomAttribute<NonActionAttribute>() != null)
//        {
//            return true;
//        }

//        string methodName = method.Name.ToLower();
//        return methodName == "index" || methodName == "Equals" || methodName == "redirecttoaction" || methodName == "view" || methodName.StartsWith("set_") || methodName.StartsWith("get") && !method.GetParameters().Any();
//    }

//    private bool IsViewDataRelatedMethod(MethodInfo method)
//    {
//        var viewDataMethods = new List<string> { "View", "ViewData", "RedirectToAction", "RedirectToRoute" };
//        return viewDataMethods.Contains(method.Name);
//    }

//    private string GetOpenApiOperation(MethodInfo method)
//    {
//        if (method.GetCustomAttribute<HttpGetAttribute>() != null) return "GET";
//        if (method.GetCustomAttribute<HttpPostAttribute>() != null) return "POST";
//        if (method.GetCustomAttribute<HttpPutAttribute>() != null) return "PUT";
//        if (method.GetCustomAttribute<HttpDeleteAttribute>() != null) return "DELETE";
//        if (method.GetCustomAttribute<HttpPatchAttribute>() != null) return "PATCH";
//        return "UNKNOWN"; // Default
//    }

//    private string? GetOpenApiParameterName(ParameterInfo parameter)
//    {
//        var parameterName = parameter.Name;

//        var fromBodyAttribute = parameter.GetCustomAttribute<FromBodyAttribute>();
//        if (fromBodyAttribute != null) return null;

//        var fromFormAttribute = parameter.GetCustomAttribute<FromFormAttribute>();
//        if (fromFormAttribute != null) return null;

//        var jsonPropertyName = parameter.GetCustomAttribute<JsonPropertyNameAttribute>();
//        var fromQueryAttribute = parameter.GetCustomAttribute<FromQueryAttribute>();
//        var fromHeaderAttribute = parameter.GetCustomAttribute<FromHeaderAttribute>();
//        var fromRouteAttribute = parameter.GetCustomAttribute<FromRouteAttribute>();

//        var attrName = jsonPropertyName?.Name
//            ?? fromQueryAttribute?.Name
//            ?? fromHeaderAttribute?.Name
//            ?? fromRouteAttribute?.Name;

//        if (IsComplexOrEnumerable(parameter.ParameterType) && string.IsNullOrEmpty(attrName))
//        {
//            return null;
//        }

//        return attrName ?? parameterName;
//    }

//    private ParameterLocation? GetParameterLocation(ParameterInfo parameter)
//    {
//        if (parameter.GetCustomAttribute<FromBodyAttribute>() != null) return null;
//        if (parameter.GetCustomAttribute<FromQueryAttribute>() != null) return ParameterLocation.Query;
//        if (parameter.GetCustomAttribute<FromHeaderAttribute>() != null) return ParameterLocation.Header;
//        if (parameter.GetCustomAttribute<FromRouteAttribute>() != null) return ParameterLocation.Path;
//        if (parameter.GetCustomAttribute<FromFormAttribute>() != null) return null;
//        if (IsComplexOrEnumerable(parameter.ParameterType)) return null;

//        if (IsSimpleType(parameter.ParameterType) && IsRouteParameterInUrl(parameter))
//        {
//            return ParameterLocation.Path;
//        }

//        return ParameterLocation.Query;
//    }

//    private bool IsArrayType(Type type)
//    {
//        return type.IsArray || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>));
//    }

//    private bool IsSimpleType(Type type)
//    {
//        Type underlyingType = Nullable.GetUnderlyingType(type) ?? type;
//        return underlyingType.IsPrimitive || underlyingType == typeof(string) || underlyingType == typeof(decimal)
//               || underlyingType == typeof(DateTime) || underlyingType == typeof(Guid);
//    }

//    private bool IsComplexOrEnumerable(Type type)
//    {
//        return !IsSimpleType(type) && !IsArrayType(type);
//    }

//    private List<PropertyMapping> GetPropertyMappings(Type type, string parentPropertyName)
//    {
//        var propertyMappings = new List<PropertyMapping>();

//        foreach (var property in type.GetProperties())
//        {
//            if (!property.CanRead || !property.CanWrite) continue;

//            var propertyMapping = new PropertyMapping
//            {
//                PropertyName = property.Name,
//                OpenApiPropertyName = string.IsNullOrEmpty(parentPropertyName)
//                    ? property.Name
//                    : $"{parentPropertyName}.{property.Name}",
//                PropertyType = property.PropertyType
//            };

//            // Recursively get property mappings for complex types
//            if (property.PropertyType.IsClass && property.PropertyType != typeof(string) && !IsArrayType(property.PropertyType) && !IsDictionaryType(property.PropertyType))
//            {
//                propertyMapping.Properties.AddRange(GetPropertyMappings(property.PropertyType, propertyMapping.OpenApiPropertyName));
//            }

//            // Handle arrays or lists
//            if (IsArrayType(property.PropertyType))
//            {
//                propertyMapping.Properties.AddRange(MapListOrArrayProperties(property.PropertyType, propertyMapping.OpenApiPropertyName));
//            }

//            // Handle dictionaries
//            if (IsDictionaryType(property.PropertyType))
//            {
//                propertyMapping.Properties.AddRange(MapDictionaryProperties(property.PropertyType, propertyMapping.OpenApiPropertyName));
//            }

//            propertyMappings.Add(propertyMapping);
//        }

//        return propertyMappings;
//    }

//    private List<PropertyMapping> MapListOrArrayProperties(Type listType, string parentPropertyName)
//    {
//        var propertyMappings = new List<PropertyMapping>();

//        Type elementType = GetElementType(listType);
//        if (elementType.IsClass && elementType != typeof(string))
//        {
//            propertyMappings.AddRange(GetPropertyMappings(elementType, $"{parentPropertyName}.Item"));
//        }

//        return propertyMappings;
//    }

//    private List<PropertyMapping> MapDictionaryProperties(Type dictionaryType, string parentPropertyName)
//    {
//        var propertyMappings = new List<PropertyMapping>();

//        var keyValueType = dictionaryType.GetGenericArguments();
//        Type keyType = keyValueType[0];
//        Type valueType = keyValueType[1];

//        if (keyType.IsClass && keyType != typeof(string))
//        {
//            propertyMappings.Add(new PropertyMapping
//            {
//                PropertyName = $"{parentPropertyName}.Key",
//                OpenApiPropertyName = $"{parentPropertyName}.Key",
//                PropertyType = keyType
//            });
//        }

//        if (valueType.IsClass && valueType != typeof(string))
//        {
//            propertyMappings.Add(new PropertyMapping
//            {
//                PropertyName = $"{parentPropertyName}.Value",
//                OpenApiPropertyName = $"{parentPropertyName}.Value",
//                PropertyType = valueType
//            });
//        }

//        return propertyMappings;
//    }

//    private Type GetElementType(Type type)
//    {
//        if (type.IsArray)
//            return type.GetElementType();

//        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
//            return type.GetGenericArguments()[0];

//        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
//            return typeof(KeyValuePair<,>).MakeGenericType(type.GetGenericArguments());

//        return type;
//    }

//    private bool IsDictionaryType(Type type)
//    {
//        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>);
//    }

//    private bool IsRouteParameterInUrl(ParameterInfo parameter)
//    {
//        // Check if the parameter is part of a route URL defined by either RouteAttribute or HTTP method attributes
//        var methodInfo = parameter.Member as MethodInfo;
//        if (methodInfo != null)
//        {
//            // Check for RouteAttribute
//            var routeAttribute = methodInfo.GetCustomAttribute<RouteAttribute>();
//            if (routeAttribute != null)
//            {
//                // Check if the route contains {parameterName}
//                if (routeAttribute.Template.Contains($"{{{parameter.Name}}}", StringComparison.OrdinalIgnoreCase))
//                    return true;
//            }

//            // If RouteAttribute is not present, check HTTP method-specific attributes for a route
//            var httpMethodAttributes = new List<Attribute>
//                    {
//                        methodInfo.GetCustomAttribute<HttpGetAttribute>(),
//                        methodInfo.GetCustomAttribute<HttpPostAttribute>(),
//                        methodInfo.GetCustomAttribute<HttpPutAttribute>(),
//                        methodInfo.GetCustomAttribute<HttpDeleteAttribute>(),
//                        methodInfo.GetCustomAttribute<HttpPatchAttribute>(),
//                        methodInfo.GetCustomAttribute<HttpOptionsAttribute>(),
//                        methodInfo.GetCustomAttribute<HttpHeadAttribute>()
//                    };

//            foreach (var httpMethodAttribute in httpMethodAttributes.Where(attr => attr != null))
//            {
//                // Get the template/route defined by the HTTP method attribute
//                var routeTemplate = httpMethodAttribute.GetType().GetProperty("Template")?.GetValue(httpMethodAttribute)?.ToString();
//                if (!string.IsNullOrEmpty(routeTemplate) && routeTemplate.Contains($"{{{parameter.Name}}}", StringComparison.OrdinalIgnoreCase))
//                {
//                    return true;
//                }
//            }
//        }
//        return false;
//    }
//}
