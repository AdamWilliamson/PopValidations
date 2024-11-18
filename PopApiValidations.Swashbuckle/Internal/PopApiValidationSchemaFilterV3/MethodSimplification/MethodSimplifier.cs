using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Text.Json.Serialization;
using PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.Helpers;

namespace PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.MethodSimplification;

public class MethodSimplifier
{
    public FunctionMapping GetMethodMap(MethodInfo method)
    {
        var functionMapping = new FunctionMapping
        {
            MethodInfo = method,
            OpenApiOperation = MethodHelper.GetOpenApiOperation(method)
        };

        // Process parameters for each method
        var parameters = method.GetParameters();

        foreach (var parameter in parameters)
        {
            var parameterMapping = new ParameterMapping
            {
                ParameterInfo = parameter,
                OpenApiParameterName = GetOpenApiParameterName(parameter),
                IsArrayType = TypeHelper.IsArrayType(parameter.ParameterType),
                Location = ParameterInfoHelper.GetParameterLocation(parameter), // Determine the location of the parameter
                IsOpenApiRequestBody = ParameterInfoHelper.GetParameterLocation(parameter) == null
            };

            // Determine if the parameter is a simple type or a complex type
            if (TypeHelper.IsSimpleType(parameter.ParameterType))
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
                    if (TypeHelper.IsArrayType(parameter.ParameterType))
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
                    else if (TypeHelper.IsDictionaryType(parameter.ParameterType))
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
                    else if (TypeHelper.IsUnTypedArrayType(method.ReturnType))
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
                    if (TypeHelper.IsArrayType(parameter.ParameterType))
                    {
                        var propertyMappings = MapListOrArrayProperties(string.Empty, parameter.ParameterType);
                        foreach (var propertyMapping in propertyMappings)
                        {
                            parameterMapping.Properties.Add(propertyMapping);
                        }
                    }
                    else if (TypeHelper.IsDictionaryType(parameter.ParameterType))
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
            if (!TypeHelper.IsSimpleType(method.ReturnType))
            {
                if (TypeHelper.IsDictionaryType(method.ReturnType))
                {
                    var propertyMappings = MapDictionaryProperties(string.Empty, method.ReturnType);
                    foreach (var propertyMapping in propertyMappings)
                    {
                        returnMapping.Properties.Add(propertyMapping);
                    }
                }
                else if (TypeHelper.IsArrayType(method.ReturnType))
                {
                    var propertyMappings = MapListOrArrayProperties(string.Empty, method.ReturnType);
                    foreach (var propertyMapping in propertyMappings)
                    {
                        returnMapping.Properties.Add(propertyMapping);
                    }
                }
                else if (TypeHelper.IsUnTypedArrayType(method.ReturnType))
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

        return attrName ?? parameterName; // Default
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
        type = TypeHelper.GetUnderlyingType(type) ?? type;

        var propertyMappings = new List<PropertyMapping>();
        if (TypeHelper.IsSimpleType(type))
        {
            return [];
        }
        else if (TypeHelper.IsDictionaryType(type))
        {
            return MapDictionaryProperties(resultPrefix, type);
        }
        else if (TypeHelper.IsArrayType(type))
        {
            return MapListOrArrayProperties(
                resultPrefix,
                type
            );
        }
        else if (TypeHelper.IsUnTypedArrayType(type))
        {
            return [];
        }


        foreach (var property in type.GetProperties())
        {
            if (!property.CanRead || !property.CanWrite) continue;

            var propertyMapping = new PropertyMapping
            {
                PropertyName = property.Name,
                OpenApiPropertyName = PropertyInfoHelper.GetOpenApiPropertyName(property),
                IsArrayType = TypeHelper.IsArrayType(property.PropertyType) || TypeHelper.IsDictionaryType(property.PropertyType),
                PropertyType = TypeHelper.GetElementType(property.PropertyType), // Ensure we use the generic element type
                ResultPropertyName = Prefix(resultPrefix, property.Name),
            };

            // If the property is a complex type (object), recurse into its properties
            if (property.PropertyType.IsClass
                && property.PropertyType != typeof(string)
                && !TypeHelper.IsArrayType(property.PropertyType)
                && !TypeHelper.IsDictionaryType(property.PropertyType))
            {
                propertyMapping.Properties.AddRange(
                    GetPropertyMappings(
                        propertyMapping.ResultPropertyName,
                        property.PropertyType
                    )
                );
            }

            // If the property is a dictionary, map both key and value recursively
            if (TypeHelper.IsDictionaryType(property.PropertyType))
            {
                propertyMappings.Add(
                    new PropertyMapping
                    {
                        PropertyName = property.Name,
                        OpenApiPropertyName = PropertyInfoHelper.GetOpenApiPropertyName(property),
                        IsArrayType = true,
                        PropertyType = TypeHelper.GetDictionaryKeyType(property.PropertyType), // Ensure we use the generic element type
                        ResultPropertyName = propertyMapping.ResultPropertyName + "[n.Key]",
                    }
                );

                propertyMappings.Add(
                    new PropertyMapping
                    {
                        PropertyName = property.Name,
                        OpenApiPropertyName = PropertyInfoHelper.GetOpenApiPropertyName(property),
                        IsArrayType = true,
                        PropertyType = TypeHelper.GetDictionaryValueType(property.PropertyType), // Ensure we use the generic element type
                        ResultPropertyName = propertyMapping.ResultPropertyName + "[n.Value]",
                    }
                );

                propertyMapping.Properties.AddRange(MapDictionaryProperties(propertyMapping.ResultPropertyName, property.PropertyType));
            }

            // If the property is a list or array, directly handle the value type without "Item"
            else if (TypeHelper.IsArrayType(property.PropertyType))
            {
                propertyMappings.Add(
                    new PropertyMapping
                    {
                        PropertyName = property.Name,
                        OpenApiPropertyName = PropertyInfoHelper.GetOpenApiPropertyName(property),
                        IsArrayType = TypeHelper.IsArrayType(property.PropertyType),
                        PropertyType = TypeHelper.GetElementType(property.PropertyType), // Ensure we use the generic element type
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

    // Handle list or array properties directly (without adding an "Items" property)
    private List<PropertyMapping> MapListOrArrayProperties(string resultPrefix, Type listType)
    {
        listType = TypeHelper.GetUnderlyingType(listType) ?? listType;
        var propertyMappings = new List<PropertyMapping>();

        // Check if the type is a list or array and get the element type
        Type elementType = TypeHelper.GetElementType(listType);

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
        dictionaryType = TypeHelper.GetUnderlyingType(dictionaryType) ?? dictionaryType;
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
}
