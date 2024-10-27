using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
                        var found = GetParamAsNavigator(openApiParam.Name, openApiParam.In, methodParam);
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
                navigators.Add(new OpenApiParamNavigator(
                        parameterInfo: methodParam,
                        parameterName: null,
                        GetSchemas(null),
                        null,
                        Operation.RequestBody
                    )
                );
            }
        }

        return navigators;
    }

    public OpenApiParamNavigator GetParamAsNavigator(string openApiPropertyName, ParameterLocation? location, ParameterInfo param)
    {
        ParameterLocation[] parameterNamedTypes = [ParameterLocation.Query, ParameterLocation.Header, ParameterLocation.Path];

        if (location.HasValue && parameterNamedTypes.Contains(location.Value) != true)
        {
            var paramName = GetAlternateName(param);

            if (paramName?.Equals(openApiPropertyName, StringComparison.OrdinalIgnoreCase) == true)
            {
                return new OpenApiParamNavigator(
                    parameterInfo: param,
                    parameterName: openApiPropertyName,
                    GetSchemas(openApiPropertyName),
                    Operation.Parameters.First(x => x.Name == openApiPropertyName),
                    null
                );
            }
        }
        else
        {
            var objHeirarchy = openApiPropertyName.Split(".");
            var firstItem = objHeirarchy[0];

            if (!IsComplexType(param.ParameterType))
            {
                return new OpenApiParamNavigator(
                        parameterInfo: param,
                        parameterName: openApiPropertyName,
                        GetSchemas(openApiPropertyName),
                        Operation.Parameters.First(x => x.Name == openApiPropertyName),
                        null
                    );
            }
            else
            {
                foreach (var prop in param.ParameterType.GetProperties())
                {
                    var propName = GetAlternateName(prop);
                    if (firstItem.Equals(propName, StringComparison.OrdinalIgnoreCase))
                    {
                        var curProp = prop;

                        foreach (var heirarchyItem in objHeirarchy.Skip(1))
                        {
                            foreach (var childprop in prop.PropertyType.GetProperties())
                            {
                                var childName = GetAlternateName(childprop);
                                if (heirarchyItem.Equals(childName, StringComparison.OrdinalIgnoreCase))
                                {
                                    curProp = childprop;
                                    break;
                                }
                            }
                        }

                        return new OpenApiParamNavigator(
                            parameterInfo: param,
                            parameterName: openApiPropertyName,
                            GetSchemas(openApiPropertyName),
                            Operation.Parameters.First(x => x.Name == openApiPropertyName),
                            null
                        );
                    }
                }
            }
        }

        throw new Exception("Param is not real?");
    }


    public string? GetAlternateName(PropertyInfo propInfo)
    {
        var renameAttr = propInfo.GetCustomAttribute<JsonPropertyAttribute>();
        return renameAttr?.PropertyName ?? propInfo.Name;
    }

    public string? GetAlternateName(ParameterInfo paramInfo)
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
}
