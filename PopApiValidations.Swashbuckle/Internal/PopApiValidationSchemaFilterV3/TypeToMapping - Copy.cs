using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Data.Common;
using System.Collections;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3
{
    public class PropertyMapping
    {
        public Type PropertyType { get; set; }
        public PropertyInfo PropertyInfo { get; set; }
        public string PropertyName { get; set; }
        public string OpenApiPropertyName { get; set; }
        public string? ResultPropertyName { get; set; }
        public List<PropertyMapping> Properties { get; set; } = new();
        public bool IsArrayType { get; set; } = false;
    }

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
                var newPrefix = (string.IsNullOrEmpty(prefix))
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
                var newPrefix = (string.IsNullOrEmpty(prefix))
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

    public class FunctionMapping
    {
        public MethodInfo MethodInfo { get; set; }
        public string OpenApiPath { get; set; }
        public string OpenApiOperation { get; set; }
        public List<ParameterMapping> Parameters { get; set; } = new();
        public List<ReturnMapping> Return { get; set; } = new();
    }

    public class TypeToMapping
    {
        public List<FunctionMapping> CreateMappings(Type controllerType)
        {
            var functionMappings = new List<FunctionMapping>();

            // Get the controller-level route (if defined)
            //var controllerRoute = GetControllerRoute(controllerType);

            // Get all methods of the controller
            var methods = GetControllerMethods(controllerType);  // Get the public methods of the controller

            foreach (var method in methods)
            {
                // Skip methods that are marked with NonAction
                if (method.GetCustomAttribute<NonActionAttribute>() != null)
                    continue;

                var functionMapping = GetMethodMap(method);

                // Add the function mapping to the list of function mappings
                functionMappings.Add(functionMapping);
            }

            return functionMappings;
        }

        public FunctionMapping GetMethodMap(MethodInfo method)
        {
            var functionMapping = new FunctionMapping
            {
                MethodInfo = method,
                //OpenApiPath = controllerRoute + GetOpenApiRoute(method),
                OpenApiOperation = GetOpenApiOperation(method)
            };

            // Process parameters for each method
            var parameters = method.GetParameters();

            foreach (var parameter in parameters)
            {
                var parameterMapping = new ParameterMapping
                {
                    ParameterInfo = parameter,
                    OpenApiParameterName = GetOpenApiParameterName(parameter),
                    IsArrayType = IsArrayType(parameter.ParameterType),
                    Location = GetParameterLocation(parameter), // Determine the location of the parameter
                    IsOpenApiRequestBody = (GetParameterLocation(parameter) == null)
                };

                // Determine if the parameter is a simple type or a complex type
                if (IsSimpleType(parameter.ParameterType))
                {
                    // Simple type: No need for body mapping, just add it directly
                    functionMapping.Parameters.Add(parameterMapping);
                    continue;
                }
                else
                {
                    // Complex type: Consider it part of the body and map its properties recursively
                    if (parameterMapping.Location is null)
                    {
                        parameterMapping.IsOpenApiRequestBody = true; // Mark as request body parameter

                        // Recursively get property mappings for the complex type
                        if (IsArrayType(parameter.ParameterType))
                        {
                            var propertyMappings = MapListOrArrayProperties(
                                parameterMapping.OpenApiParameterName ?? parameter.Name,
                                parameter.ParameterType
                            );

                            foreach (var propertyMapping in propertyMappings)
                            {
                                // Add the property mappings to the main parameter mapping
                                parameterMapping.Properties.Add(propertyMapping);
                            }
                        }
                        else if (IsDictionaryType(parameter.ParameterType))
                        {
                            var propertyMappings = MapDictionaryProperties(
                                parameterMapping.OpenApiParameterName ?? parameter.Name,
                                parameter.ParameterType
                            );

                            foreach (var propertyMapping in propertyMappings)
                            {
                                // Add the property mappings to the main parameter mapping
                                parameterMapping.Properties.Add(propertyMapping);
                            }
                        }
                        else if (IsUnTypedArrayType(method.ReturnType))
                        {
                            // skip
                        }
                        else
                        {
                            var propertyMappings = GetPropertyMappings(
                                string.Empty,
                                parameter.ParameterType
                            );

                            foreach (var propertyMapping in propertyMappings)
                            {
                                // Add the property mappings to the main parameter mapping
                                parameterMapping.Properties.Add(propertyMapping);
                            }
                        }
                    }
                    else
                    {
                        if (IsArrayType(parameter.ParameterType)) 
                        {
                            var propertyMappings = MapListOrArrayProperties(string.Empty, parameter.ParameterType);
                            foreach (var propertyMapping in propertyMappings)
                            {
                                parameterMapping.Properties.Add(propertyMapping);
                            }
                        }
                        else if (IsDictionaryType(parameter.ParameterType))
                        {
                            var propertyMappings = MapDictionaryProperties(string.Empty, parameter.ParameterType);
                            foreach (var propertyMapping in propertyMappings)
                            {
                                parameterMapping.Properties.Add(propertyMapping);
                            }
                        }
                        else
                        {
                            var propertyMappings = GetPropertyMappings(string.Empty, parameter.ParameterType);
                            foreach (var propertyMapping in propertyMappings)
                            {
                                parameterMapping.Properties.Add(propertyMapping);
                            }
                        }
                    }

                    // Add the parameter mapping to the function (even if it’s complex)
                    functionMapping.Parameters.Add(parameterMapping);
                }
            }

            if (method.ReturnType != typeof(void))
            {
                var returnMapping = new ReturnMapping
                {
                    ReturnType = method.ReturnType
                };

                // Determine if the parameter is a simple type or a complex type
                if (!IsSimpleType(method.ReturnType))
                {
                    if (IsDictionaryType(method.ReturnType))
                    {
                        var propertyMappings = MapDictionaryProperties(string.Empty, method.ReturnType);
                        foreach (var propertyMapping in propertyMappings)
                        {
                            returnMapping.Properties.Add(propertyMapping);
                        }
                    }
                    else if (IsArrayType(method.ReturnType))
                    {
                        var propertyMappings = MapListOrArrayProperties(string.Empty, method.ReturnType);
                        foreach (var propertyMapping in propertyMappings)
                        {
                            returnMapping.Properties.Add(propertyMapping);
                        }
                    }
                    else if (IsUnTypedArrayType(method.ReturnType))
                    {
                        // skip
                    }
                    else
                    {
                        var propertyMappings = GetPropertyMappings(string.Empty, method.ReturnType);
                        foreach (var propertyMapping in propertyMappings)
                        {
                            returnMapping.Properties.Add(propertyMapping);
                        }
                    }
                }

                functionMapping.Return.Add(returnMapping);
            }

            return functionMapping;
        }

        public List<MethodInfo> GetControllerMethods(Type controllerType)
        {
            // A list to store methods that could become URLs
            var validMethods = new List<MethodInfo>();

            // Get all public instance methods from the controller
            var methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance);

            foreach (var method in methods)
            {
                // Skip methods that are non-action methods or are related to view rendering
                if (IsNonActionMethod(method) || IsViewDataRelatedMethod(method))
                    continue;

                // Add the method to the list, as it could potentially be mapped to a route
                validMethods.Add(method);
            }

            return validMethods;
        }

        // Check if the method is marked with [NonAction] or if it's a common method related to view rendering
        private bool IsNonActionMethod(MethodInfo method)
        {
            // Methods with [NonAction] attribute are not meant to be URL endpoints
            if (method.GetCustomAttribute<NonActionAttribute>() != null)
            {
                return true;
            }

            // Optionally: Exclude common methods like `Index`, `RedirectToAction`, `View`, etc.
            // These are typical methods in controllers that return views and are not intended to be API actions
            string methodName = method.Name.ToLower();
            return methodName == "index" || methodName == "Equals" || methodName == "redirecttoaction" || methodName == "view" || methodName.StartsWith("set_") || methodName.StartsWith("get") && !method.GetParameters().Any();
        }

        // Check if the method is related to ViewData or view rendering (such as `View`, `RedirectToAction`, etc.)
        private bool IsViewDataRelatedMethod(MethodInfo method)
        {
            // Exclude methods related to view rendering and view data
            var viewDataMethods = new List<string> { "View", "ViewData", "RedirectToAction", "RedirectToRoute" };
            return viewDataMethods.Contains(method.Name);
        }

        // Get the controller-level route path
        //private string GetControllerRoute(Type controllerType)
        //{
        //    var routeAttribute = controllerType.GetCustomAttribute<RouteAttribute>();
        //    if (routeAttribute != null)
        //    {
        //        // Replace [controller] with the actual controller name without the 'Controller' suffix
        //        var controllerName = controllerType.Name.Replace("Controller", "").ToLower();
        //        return routeAttribute.Template.Replace("[controller]", controllerName);
        //    }
        //    return ""; // No route defined for controller
        //}

        // Get the OpenAPI route path for a method (can be customized based on routing attributes)
        //private string GetOpenApiRoute(MethodInfo method)
        //{
        //    var routeAttribute = method.GetCustomAttribute<RouteAttribute>();
        //    string routeTemplate = routeAttribute?.Template ?? "";

        //    // Append additional information if needed based on the HTTP method attributes
        //    if (method.GetCustomAttribute<HttpGetAttribute>() != null || method.GetCustomAttribute<HttpPostAttribute>() != null ||
        //        method.GetCustomAttribute<HttpPutAttribute>() != null || method.GetCustomAttribute<HttpDeleteAttribute>() != null)
        //    {
        //        var httpMethodAttributes = new List<Attribute>
        //        {
        //            method.GetCustomAttribute<HttpGetAttribute>(),
        //            method.GetCustomAttribute<HttpPostAttribute>(),
        //            method.GetCustomAttribute<HttpPutAttribute>(),
        //            method.GetCustomAttribute<HttpDeleteAttribute>()
        //        };

        //        foreach (var httpMethodAttribute in httpMethodAttributes.Where(x => x != null))
        //        {
        //            var httpMethodRoute = httpMethodAttribute.GetType().GetProperty("Template")?.GetValue(httpMethodAttribute)?.ToString();
        //            if (!string.IsNullOrEmpty(httpMethodRoute))
        //            {
        //                routeTemplate = $"{routeTemplate}/{httpMethodRoute}";
        //            }
        //        }
        //    }

        //    return routeTemplate;
        //}

        // Get the OpenAPI operation (HTTP method) based on the attributes (e.g., GET, POST, etc.)
        private string GetOpenApiOperation(MethodInfo method)
        {
            if (method.GetCustomAttribute<HttpGetAttribute>() != null) return "GET";
            if (method.GetCustomAttribute<HttpPostAttribute>() != null) return "POST";
            if (method.GetCustomAttribute<HttpPutAttribute>() != null) return "PUT";
            if (method.GetCustomAttribute<HttpDeleteAttribute>() != null) return "DELETE";
            if (method.GetCustomAttribute<HttpPatchAttribute>() != null) return "PATCH";
            return "UNKNOWN"; // Default
        }

        // Get the OpenAPI parameter name, using attributes like FromBody, FromQuery, etc.
        private string? GetOpenApiParameterName(ParameterInfo parameter)
        {
            var parameterName = parameter.Name;

            var fromBodyAttribute = parameter.GetCustomAttribute<FromBodyAttribute>();
            if (fromBodyAttribute != null) return null;

            var fromFormAttribute = parameter.GetCustomAttribute<FromFormAttribute>();
            if (fromFormAttribute != null) return null;

            var jsonPropertyName = parameter.GetCustomAttribute<JsonPropertyNameAttribute>();
            var fromQueryAttribute = parameter.GetCustomAttribute<FromQueryAttribute>();
            var fromHeaderAttribute = parameter.GetCustomAttribute<FromHeaderAttribute>();
            var fromRouteAttribute = parameter.GetCustomAttribute<FromRouteAttribute>();

            var attrName = jsonPropertyName?.Name
                ?? fromQueryAttribute?.Name
                ?? fromHeaderAttribute?.Name
                ?? fromRouteAttribute?.Name;

            //if (
            //    IsComplexOrEnumerable(parameter.ParameterType) && string.IsNullOrEmpty(attrName)
            //    && !IsSimpleTypeOrEnumerableOfSimpleType(parameter.ParameterType)
            //)
            //{
            //    return null;
            //}

            return attrName ?? parameterName; // Default
        }

        private ParameterLocation? GetParameterLocation(ParameterInfo parameter)
        {
            // First check for attributes that specify parameter location
            if (parameter.GetCustomAttribute<FromBodyAttribute>() != null) return null;
            if (parameter.GetCustomAttribute<FromQueryAttribute>() != null) return ParameterLocation.Query;
            if (parameter.GetCustomAttribute<FromHeaderAttribute>() != null) return ParameterLocation.Header;
            if (parameter.GetCustomAttribute<FromRouteAttribute>() != null) return ParameterLocation.Path;
            if (parameter.GetCustomAttribute<FromFormAttribute>() != null) return null;
            if (IsComplexOrEnumerable(parameter.ParameterType)) return null;

            // If it's a simple type and the route contains {parameterName}, classify it as Route
            if (IsSimpleType(parameter.ParameterType) && IsRouteParameterInUrl(parameter))
            {
                return ParameterLocation.Path;
            }

            // Default to Unknown if no attribute and no matching conditions are found
            return null;
        }

        private bool IsComplexOrEnumerable(Type type)
        {
            Type underlyingType = Nullable.GetUnderlyingType(type) ?? type;
            // Check if it's a complex type (class) or a collection type (array, List<T>, Dictionary<TKey, TValue>)
            return type.IsClass && type != typeof(string) || IsArrayType(type) || IsDictionaryType(type);
        }

        private bool IsSimpleType(Type type)
        {
            // If it's a nullable type, get the underlying type (e.g., int? -> int)
            Type underlyingType = Nullable.GetUnderlyingType(type) ?? type;

            // Simple types are typically primitive types or basic types like int, string, DateTime, etc.
            return underlyingType.IsPrimitive || underlyingType == typeof(string) || underlyingType == typeof(decimal)
                   || underlyingType == typeof(DateTime) || underlyingType == typeof(Guid);
        }

        //private bool IsSimpleTypeOrEnumerableOfSimpleType(Type type)
        //{
        //    if (IsSimpleType(type)) return true;
        //    if (IsArrayType(type)) return IsSimpleType(GetElementType(type));
        //    return false;
        //}

        private bool IsRouteParameterInUrl(ParameterInfo parameter)
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

        private string Prefix(string prefix, string addition)
        {
            if (string.IsNullOrWhiteSpace(prefix)) return addition;
            else if (addition.StartsWith("[n")) return prefix + addition;

            return prefix + "." + addition;
        }

        // Recursively map properties of complex types
        private List<PropertyMapping> GetPropertyMappings(string resultPrefix, Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;

            var propertyMappings = new List<PropertyMapping>();
            if (IsSimpleType(type))
            {
                return [];
            }
            else if (IsDictionaryType(type))
            {
                return MapDictionaryProperties(resultPrefix, type);
            }
            else if (IsArrayType(type))
            {
                return MapListOrArrayProperties(
                    resultPrefix,
                    type
                );
            }
            else if (IsUnTypedArrayType(type))
            {
                return [];
            }


            foreach (var property in type.GetProperties())
            {
                if (!property.CanRead || !property.CanWrite) continue;

                var propertyMapping = new PropertyMapping
                {
                    PropertyName = property.Name,
                    OpenApiPropertyName = GetOpenApiPropertyName(property),
                    IsArrayType = IsArrayType(property.PropertyType) || IsDictionaryType(property.PropertyType),
                    PropertyType = GetElementType(property.PropertyType), // Ensure we use the generic element type
                    ResultPropertyName = Prefix(resultPrefix, property.Name),
                };

                // If the property is a complex type (object), recurse into its properties
                if (property.PropertyType.IsClass
                    && property.PropertyType != typeof(string)
                    && !IsArrayType(property.PropertyType)
                    && !IsDictionaryType(property.PropertyType))
                {
                    propertyMapping.Properties.AddRange(
                        GetPropertyMappings(
                            propertyMapping.ResultPropertyName, 
                            property.PropertyType
                        )
                    );
                }

                // If the property is a dictionary, map both key and value recursively
                if (IsDictionaryType(property.PropertyType))
                {
                    propertyMappings.Add(
                        new PropertyMapping
                        {
                            PropertyName = property.Name,
                            OpenApiPropertyName = GetOpenApiPropertyName(property),
                            IsArrayType = true,
                            PropertyType = GetDictionaryKeyType(property.PropertyType), // Ensure we use the generic element type
                            ResultPropertyName = propertyMapping.ResultPropertyName +"[n.Key]",
                        }
                    );

                    propertyMappings.Add(
                        new PropertyMapping
                        {
                            PropertyName = property.Name,
                            OpenApiPropertyName = GetOpenApiPropertyName(property),
                            IsArrayType = true,
                            PropertyType = GetDictionaryValueType(property.PropertyType), // Ensure we use the generic element type
                            ResultPropertyName = propertyMapping.ResultPropertyName + "[n.Value]",
                        }
                    );

                    propertyMapping.Properties.AddRange(MapDictionaryProperties(propertyMapping.ResultPropertyName, property.PropertyType));
                }

                // If the property is a list or array, directly handle the value type without "Item"
                else if (IsArrayType(property.PropertyType))
                {
                    propertyMappings.Add(
                        new PropertyMapping
                        {
                            PropertyName = property.Name,
                            OpenApiPropertyName = GetOpenApiPropertyName(property),
                            IsArrayType = IsArrayType(property.PropertyType),
                            PropertyType = GetElementType(property.PropertyType), // Ensure we use the generic element type
                            ResultPropertyName = propertyMapping.ResultPropertyName + "[n]",
                        }
                    );

                    propertyMapping.Properties.AddRange(
                        MapListOrArrayProperties(
                            propertyMapping.ResultPropertyName, 
                            property.PropertyType
                        )
                    );
                }

                // Add this property mapping to the list
                propertyMappings.Add(propertyMapping);
            }

            return propertyMappings;
        }

        // Get the OpenAPI name for the property (can be customized based on attributes like JsonPropertyName, etc.)
        private string GetOpenApiPropertyName(PropertyInfo property)
        {
            var jsonPropertyName = property.GetCustomAttribute<JsonPropertyNameAttribute>();
            if (jsonPropertyName != null)
            {
                return jsonPropertyName.Name;
            }
            return property.Name; // Default to the property name
        }

        // Handle list or array properties directly (without adding an "Items" property)
        private List<PropertyMapping> MapListOrArrayProperties(string resultPrefix, Type listType)
        {
            listType = Nullable.GetUnderlyingType(listType) ?? listType;
            var propertyMappings = new List<PropertyMapping>();

            // Check if the type is a list or array and get the element type
            Type elementType = GetElementType(listType);

            //GetPropertyMappings(Prefix(resultPrefix, "[n]"), elementType, "Property")
            

            // If the element type is a complex type, recursively map its properties
            if (elementType.IsClass && elementType != typeof(string))
            {
                propertyMappings.AddRange(GetPropertyMappings(resultPrefix + "[n]", elementType));
            }

            return propertyMappings;
        }

        // Handle dictionary key and value properties separately
        private List<PropertyMapping> MapDictionaryProperties(string resultPrefix, Type dictionaryType)
        {
            dictionaryType = Nullable.GetUnderlyingType(dictionaryType) ?? dictionaryType;
            var propertyMappings = new List<PropertyMapping>();

            var keyValueType = dictionaryType.GetGenericArguments();
            Type keyType = keyValueType[0];
            Type valueType = keyValueType[1];

            // Map Key
            if (keyType.IsClass && keyType != typeof(string))
            {
                var name = Prefix(resultPrefix, "[n.Key]");
                propertyMappings.Add(new PropertyMapping
                {
                    PropertyName = "Key",
                    OpenApiPropertyName = "Key",
                    PropertyType = keyType,
                    Properties = GetPropertyMappings(name, keyType),
                    ResultPropertyName = Prefix(resultPrefix, name),
                });
            }
            else
            {
                propertyMappings.Add(new PropertyMapping
                {
                    PropertyName = "Key",
                    OpenApiPropertyName = "Key",
                    PropertyType = keyType,
                    ResultPropertyName = Prefix(resultPrefix, "[n.Key]"),
                });
            }

            // Map Value
            if (valueType.IsClass && valueType != typeof(string))
            {
                var name = Prefix(resultPrefix, "[n.Value]");
                propertyMappings.Add(new PropertyMapping
                {
                    PropertyName = "Value",
                    OpenApiPropertyName = "Value",
                    PropertyType = valueType,
                    Properties = GetPropertyMappings(name, valueType), // Recursively map the value properties
                    ResultPropertyName = Prefix(resultPrefix, "[n.Value]")
                });
            }
            else
            {
                propertyMappings.Add(new PropertyMapping
                {
                    PropertyName = "Value",
                    OpenApiPropertyName = "Value",
                    PropertyType = valueType,
                    ResultPropertyName = Prefix(resultPrefix, "[n.Value]")
                });
            }

            return propertyMappings;
        }

        // Check if the type is an array, list, or dictionary
        private bool IsArrayType(Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;

            if (type == typeof(string))
            {
                return false;
            }

            // Check if the type is an array
            if (type.IsArray && type.IsGenericType)
            {
                return true;
            }

            var isEnumerable = type.GetInterfaces()
                   .Append(type) // ensure this type is also checked
                   .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            // Check if the type is a generic collection like List<T>, LinkedList<T>, etc.
            if (isEnumerable)
            {
                return true;
            }

            return false;
        }

        private bool IsUnTypedArrayType(Type type)
        {
            if (!IsArrayType(type))
            {
                var isEnumerable = type.GetInterfaces()
                   .Append(type) // ensure this type is also checked
                   .Any(x => !x.IsGenericType && x == typeof(IEnumerable));

                return isEnumerable;
            }

            return false;
        }

        // Check if the type is a Dictionary<TKey, TValue>
        private bool IsDictionaryType(Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;
            //return type.IsGenericType && typeof(IDictionary<,>).IsAssignableFrom(type.GetGenericTypeDefinition());
            return type.GetInterfaces()
                   .Append(type) // ensure this type is also checked
                   .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IDictionary<,>));
        }

        // Get the element type for arrays, lists, or dictionaries
        private Type GetElementType(Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;

            if (type.IsArray && type.IsGenericType)
            {
                return type.GetElementType();  // Get the element type for arrays
            }

            if (type.IsGenericType)
            {
                //var genericType = type.GetGenericTypeDefinition();

                //var isIEnumerableT = type.GetInterfaces()
                //   .Append(type) // ensure this type is also checked
                //   .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>));

                if (IsDictionaryType(type))
                {
                    // Return KeyValuePair<TKey, TValue> for Dictionary<TKey, TValue>
                    return typeof(KeyValuePair<,>).MakeGenericType(type.GetGenericArguments());
                }

                if (IsArrayType(type))
                {
                    return type.GetGenericArguments()[0];  // Get the generic argument type for List<T>
                }

                //var isIDictionaryT = type.GetInterfaces()
                //   .Append(type) // ensure this type is also checked
                //   .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IDictionary<,>));

            }

            return type;
        }

        private Type GetDictionaryKeyType(Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;

            if (type.IsGenericType)
            {
            
                if (IsDictionaryType(type))
                {
                    // Return KeyValuePair<TKey, TValue> for Dictionary<TKey, TValue>
                    return type.GetGenericArguments()[0];
                }
            }

            return type;
        }

        private Type GetDictionaryValueType(Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;

            if (type.IsGenericType)
            {

                if (IsDictionaryType(type))
                {
                    // Return KeyValuePair<TKey, TValue> for Dictionary<TKey, TValue>
                    return type.GetGenericArguments()[1];
                }
            }

            return type;
        }
    }
}
