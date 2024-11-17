using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PopApiValidations.Swashbuckle.Internal.OperationFilter;

public class OpenApiOperationNavigator
{
    public OpenApiOperationNavigator(
        PopApiOpenApiConfig config,
        SchemaRepository schemaRepository,
        OpenApiOperation operation,
        MethodInfo methodInfo
        )
    {
        Config = config;
        SchemaRepository = schemaRepository;
        Operation = operation;
        MethodInfo = methodInfo;
    }

    public PopApiOpenApiConfig Config { get; }
    public SchemaRepository SchemaRepository { get; }
    public OpenApiOperation Operation { get; }
    public MethodInfo MethodInfo { get; }

    private OpenApiSchema[] GetSchemas(string? openApiPropertyName)
    {
        var param = Operation.Parameters.FirstOrDefault(x => x.Name == openApiPropertyName);
        if (param == null)
        {
            return Operation.RequestBody.Content.Values.Select(x =>
            {
                if (x.Schema.UnresolvedReference)
                {
                    return SchemaRepository.Schemas[x.Schema.Reference.Id];
                }
                return x.Schema;
            }).ToArray();
        }

        if (param.Schema.UnresolvedReference)
        {
            return new[] { SchemaRepository.Schemas[param.Schema.Reference.Id] };
        }

        return new[] { param.Schema };
    }

    public static bool IsComplexType(Type type)
    {
        return !type.IsPrimitive && type != typeof(string) && !type.IsValueType;
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

    public List<OpenApiParamNavigator> GetOpenApiParamNavigators()
    {
        List<OpenApiParamNavigator> navigators = new();
        var methodParams = MethodInfo.GetParameters().ToList();

        var groupedParams = Operation.Parameters.GroupBy(x => x.In);
        var groupedMethodParams = methodParams.GroupBy(x => GetParameterLocation(x));

        foreach (var openApiParamGroup in groupedParams)
        {
            foreach (var methodParamGroup in groupedMethodParams.Where(x => x.Key == openApiParamGroup.Key))
            {
                foreach (var openApiParam in openApiParamGroup)
                {
                    foreach (var methodParam in methodParamGroup)
                    {
                        var found = GetParamAsNavigator(openApiParam.Name, openApiParam.In, MethodInfo, methodParam);
                        if (found is not null)
                        {
                            navigators.Add(found!);
                            break;
                        }
                    }
                }
            }
        }

        foreach (var methodParamGroup in groupedMethodParams.Where(x => x.Key == null))
        {
            foreach (var methodParam in methodParamGroup)
            {
                navigators.Add(
                    new OpenApiParamNavigator(
                        operation: Operation,
                        parameterInfo: methodParam,
                        openApiParameterName: null,
                        GetSchemas(null),
                        null,
                        Operation.RequestBody,
                        parameterName: null
                    )
                );
            }
        }

        return navigators;
    }

    public OpenApiParamNavigator GetParamAsNavigator(string? openApiPropertyName, ParameterLocation? location, MethodInfo method, ParameterInfo param)
    {
        foreach(var methodParam in method.GetParameters())
        {
            if (QueryParameterConverter.GetAlternateName(param) == openApiPropertyName)
            {
                return new OpenApiParamNavigator(
                    operation: Operation,
                    parameterInfo: param,
                    openApiParameterName: openApiPropertyName,
                    GetSchemas(openApiPropertyName),
                    Operation.Parameters.First(x => x.Name == openApiPropertyName),
                    null,
                    parameterName: null
                );
            }
        }

        if (location.HasValue)
        {
            var paramName = QueryParameterConverter.GetAlternateName(param);

            if (paramName?.Equals(openApiPropertyName, StringComparison.OrdinalIgnoreCase) == true)
            {
                return new OpenApiParamNavigator(
                    operation: Operation,
                    parameterInfo: param,
                    openApiParameterName: openApiPropertyName,
                    GetSchemas(openApiPropertyName),
                    Operation.Parameters.First(x => x.Name == openApiPropertyName),
                    null,
                    parameterName: QueryParameterConverter.GetPropertyNameFromQuery(openApiPropertyName, param)
                );
            }
        }
        else
        {
            //  Is like an int.  Ignore.
            if (!IsComplexType(param.ParameterType))
            {
                return new OpenApiParamNavigator(
                        operation: Operation,
                        parameterInfo: param,
                        openApiParameterName: null,
                        GetSchemas(openApiPropertyName),
                        Operation.Parameters.First(x => x.Name == openApiPropertyName),
                        null,
                        parameterName: null
                    );
            }
            else
            {
                //  Is an object. we can assume openApiPropertyname is not null, because this is also a parameter.
                var objHeirarchy = openApiPropertyName.Split(".");
                var firstItem = objHeirarchy[0];

                foreach (var prop in param.ParameterType.GetProperties())
                {
                    var propName = QueryParameterConverter.GetAlternateName(prop);
                    if (firstItem.Equals(propName, StringComparison.OrdinalIgnoreCase))
                    {
                        var curProp = prop;

                        foreach (var heirarchyItem in objHeirarchy.Skip(1))
                        {
                            foreach (var childprop in prop.PropertyType.GetProperties())
                            {
                                var childName = QueryParameterConverter.GetAlternateName(childprop);
                                if (heirarchyItem.Equals(childName, StringComparison.OrdinalIgnoreCase))
                                {
                                    curProp = childprop;
                                    break;
                                }
                            }
                        }

                        return new OpenApiParamNavigator(
                            operation: Operation,
                            parameterInfo: param,
                            openApiParameterName: null, //param.Name,
                            GetSchemas(openApiPropertyName),
                            Operation.Parameters.First(x => x.Name == openApiPropertyName),
                            null,
                            parameterName: null
                        );
                    }
                }
            }
        }

        return null;
    }


    
}

public static class QueryParameterConverter
{
    public static string? GetAlternateName(PropertyInfo propInfo)
    {
        var renameAttr = propInfo.GetCustomAttribute<JsonPropertyAttribute>();
        return renameAttr?.PropertyName ?? propInfo.Name;
    }

    public static string? GetAlternateName(ParameterInfo paramInfo)
    {
        var urlAttr = paramInfo.GetCustomAttribute<FromRouteAttribute>();
        if (urlAttr is not null && !string.IsNullOrWhiteSpace(urlAttr.Name))
        {
            return urlAttr.Name;
        }
        else if (urlAttr is not null)
        {
            return paramInfo.Name;
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
            return paramInfo.Name;
        }

        var headerAttr = paramInfo.GetCustomAttribute<FromHeaderAttribute>();
        if (headerAttr is not null && !string.IsNullOrWhiteSpace(headerAttr.Name))
        {
            return headerAttr.Name;
        }
        else if (headerAttr is not null)
        {
            return paramInfo.Name;
        }

        var formAttr = paramInfo.GetCustomAttribute<FromFormAttribute>();
        if (formAttr is not null)
        {
            return string.Empty;
        }

        var jsonAttr = paramInfo.GetCustomAttribute<JsonPropertyAttribute>();
        if (jsonAttr is not null)
        {
            return jsonAttr.PropertyName;
        }

        return paramInfo.Name;
    }

    public static string? GetPropertyNameFromQuery(string queryParameter, ParameterInfo parameterInfo)
    {
        if (string.IsNullOrWhiteSpace(queryParameter)) return null;

        if (queryParameter == GetAlternateName(parameterInfo)) return null;

        // Split the query parameter by the '.' character
        var parts = queryParameter.Split('.');
        Type currentType = parameterInfo.ParameterType; // Start with the type of the parameter
        string builtUpName = "";

        if (IsBasicType(parameterInfo.ParameterType) && parts.Count() > 1)
        {
            throw new Exception("Contains a . showing its a child property, but there isn't any..");
        }
        else if (IsBasicType(parameterInfo.ParameterType) && GetAlternateName(parameterInfo) == queryParameter)
        {
            //return parameterInfo.Name;
            return null;
        }

        // Iterate through the parts of the query parameter
        for (int i = 0; i < parts.Length; i++)
        {
            var part = parts[i];
            // Get the property info for the current part
            PropertyInfo? propertyInfo = GetPropertyInfo(currentType, part);

            // If the property is not found, throw an exception
            if (propertyInfo == null)
            {
                throw new ArgumentException($"Property '{part}' not found on type '{currentType.Name}'.");
            }

            // Build the hierarchy name
            if (IsBasicType(propertyInfo.PropertyType))
            {
                builtUpName += $"{propertyInfo.Name}";
            }
            else if (IsEnumerableType(propertyInfo.PropertyType))
            {
                // Add the property name with [n] to denote an enumerable
                builtUpName += $"{propertyInfo.Name}[n]";
            }
            else
            {
                builtUpName += $"{propertyInfo.Name}";
            }

            // Move to the type of the current property
            currentType = propertyInfo.PropertyType.IsArray ? propertyInfo.PropertyType.GetElementType() : propertyInfo.PropertyType;

            // Add a dot after the property name unless it's the last part
            if (i < parts.Length - 1)
            {
                builtUpName += ".";
            }
        }

        return builtUpName; // Return the built-up name without a trailing dot
    }

    private static PropertyInfo? GetPropertyInfo(Type type, string propertyName)
    {
        // Get the property by name first
        PropertyInfo property = type.GetProperty(propertyName);
        if (property != null)
        {
            return property;
        }

        // If not found, check for JsonProperty attributes
        var properties = type.GetProperties();
        foreach (var prop in properties)
        {
            var jsonAttr = prop.GetCustomAttribute<JsonPropertyAttribute>();
            if (jsonAttr != null && string.Equals(jsonAttr.PropertyName, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return prop;
            }
        }

        return null; // Property not found
    }

    private static bool IsBasicType(Type type)
    {
        // Define what constitutes a basic type (e.g., int, string, bool, etc.)
        return type.IsPrimitive || type == typeof(string) || type.IsValueType;
    }

    private static bool IsEnumerableType(Type type)
    {
        // Check if the type is an array or implements IEnumerable<>
        return type.IsArray || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(System.Collections.Generic.IEnumerable<>));
    }
}