using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace PopApiValidations.Swashbuckle.Internal.OperationFilter;

public class OpenApiPropertyBasis(
    string openApiPropertyName,
    string currentObjectHeirarchy,
    Type objectType,
    OpenApiSchema schema)
{
    public string OpenApiPropertyName => openApiPropertyName;
    public string CurrentObjectHeirarchy => currentObjectHeirarchy;
    public Type ObjectType => objectType;
    public OpenApiSchema Schema => schema;
}

public class OpenApiParamBasis(
    string openApiPropertyName,
    string currentObjectHeirarchy,
    Type objectType,
    OpenApiSchema[] schemas,
    OpenApiParameter? parameterSchema,
    OpenApiRequestBody? requestBody
    )
{
    public string OpenApiPropertyName => openApiPropertyName;
    public string CurrentObjectHeirarchy => currentObjectHeirarchy;
    public Type ObjectType => objectType;
    public OpenApiSchema[] Schemas => schemas;
    public OpenApiParameter? ParameterSchema  => parameterSchema;
    public OpenApiRequestBody? RequestBody => requestBody;
}

public class OpenApiParamNavigator
{
    public OpenApiParamNavigator(
        ParameterInfo parameterInfo,
        string? parameterName,
        OpenApiSchema[] schemas,
        OpenApiParameter? parameterSchema,
        OpenApiRequestBody? requestBody
        )
    {
        ParameterInfo = parameterInfo;
        ParameterName = parameterName;
        Schemas = schemas;
        ParameterSchema = parameterSchema;
        RequestBody = requestBody;
    }

    public string? ParameterName { get; }
    public Type ObjectType => ParameterInfo.ParameterType;
    public OpenApiSchema[] Schemas { get; }
    public OpenApiParameter? ParameterSchema { get; }
    public OpenApiRequestBody? RequestBody { get; }
    public ParameterInfo ParameterInfo { get; }
    public int ParamIndex => ParameterInfo.Position;
    public bool IsArray
    { 
        get
        {
            if (ParameterSchema is not null && ParameterSchema.Schema.Items != null)
            {
                return true;
            }

            if (RequestBody is not null && RequestBody.Content.Values.Any(x => x.Schema.Items != null))
            {
                return true;
            }

            return false;
        }
    }

    internal IEnumerable<OpenApiParamBasis> GetParamBases(string prefix, string oridinalIndicator)
    {
        var curObjHeirarchy = prefix + ParameterName;
        if (ParameterName is not null && ParameterSchema is not null)
        {
            yield return new OpenApiParamBasis(
                ParameterName, 
                curObjHeirarchy, 
                ObjectType, 
                Schemas, 
                ParameterSchema, 
                RequestBody);

            yield return new OpenApiParamBasis(
                    ParameterName,
                    curObjHeirarchy + oridinalIndicator,
                    ObjectType,
                    Schemas,
                    ParameterSchema,
                    RequestBody);
        }

        if (RequestBody is not null)
        {
            foreach (var key in RequestBody.Content.Keys)
            {
                yield return new OpenApiParamBasis(
                    key,
                    curObjHeirarchy,
                    ObjectType,
                    Schemas,
                    ParameterSchema,
                    RequestBody);

                yield return new OpenApiParamBasis(
                        key,
                        curObjHeirarchy + oridinalIndicator,
                        ObjectType,
                        Schemas,
                        ParameterSchema,
                        RequestBody);
            }
        }
    }

    internal IEnumerable<OpenApiPropertyBasis> GetPropertyBases(string prefix, string oridinalIndicator)
    {
        foreach(var p in GetParamBases(prefix, oridinalIndicator))
        {
            foreach (var schema in p.Schemas)
            {
                foreach (var property in schema.Properties)
                {
                    yield return new OpenApiPropertyBasis(
                        openApiPropertyName: property.Key,
                        currentObjectHeirarchy: p.CurrentObjectHeirarchy + "." + ,
                        objectType: ,
                        schema: schema
                    );
                }
            }
        }
    }
}

public static class PropertyHelper
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
}