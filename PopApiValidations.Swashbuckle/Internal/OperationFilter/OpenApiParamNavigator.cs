using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using PopValidations.Swashbuckle;
using PopValidations.Swashbuckle.Internal;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace PopApiValidations.Swashbuckle.Internal.OperationFilter;

public class OpenApiPropertyBasis(
    OpenApiOperation operation,
    string openApiPropertyName,
    string parentOpenApiHeirarchy,
    string currentObjectHeirarchy,
    Type objectType,
    OpenApiSchema schema,
    ValidationLevel propertyValidationLevel,
    OpenApiSchema? parentSchema)
{
    private readonly OpenApiOperation operation = operation;
    public string OpenApiPropertyName => openApiPropertyName;
    public string CurrentObjectHeirarchy => currentObjectHeirarchy;
    public Type ObjectType => objectType;
    public OpenApiSchema Schema => schema;
    public string OpenApiHeirarchy => parentOpenApiHeirarchy + "." + openApiPropertyName;
    public ValidationLevel PropertyValidationLevel => propertyValidationLevel;
    public OpenApiSchema? ParentSchema => parentSchema;

    private PopValidationArray? SchemaValidationArray = null;
    private PopValidationArray? OperationValidationArray = null;
    
    public OpenApiPropertyBasis[] GetPropertyBases() 
    {
        return From(operation, Schema, OpenApiHeirarchy, ObjectType, CurrentObjectHeirarchy, PropertyValidationLevel);
    }

    public PopValidationArray? GetValidationArray(string extension, string? groupHeader)
    {
        if (PropertyValidationLevel.HasFlag(ValidationLevel.ValidationAttributeInBase))
        {
            OperationValidationArray ??= PopValidationArray.From(
                extension, 
                operation.Extensions, 
                CurrentObjectHeirarchy + "." + OpenApiPropertyName
            );
            OperationValidationArray.SetLineHeader(groupHeader);

            return OperationValidationArray;
        }

        if (PropertyValidationLevel.HasFlag(ValidationLevel.ValidationAttribute))
        {
            SchemaValidationArray ??= PopValidationArray.From(extension, schema.Extensions, OpenApiPropertyName);
            SchemaValidationArray.SetLineHeader(groupHeader);

            return SchemaValidationArray;
        }

        return null;
    }

    public static OpenApiPropertyBasis[] From(
        OpenApiOperation operation, 
        OpenApiSchema schema,
        string parentOpenApiHeirarchy,
        Type objectType,
        string objHeirarchy,       
        ValidationLevel PropertyValidationLevel)
    {
        return schema.Properties.Select(kvp =>
            new OpenApiPropertyBasis(
                operation: operation,
                openApiPropertyName: kvp.Key,
                parentOpenApiHeirarchy: parentOpenApiHeirarchy,
                currentObjectHeirarchy: objHeirarchy + "." + kvp.Key,
                PropertyHelper.GetProperties(objectType).First(x => string.Equals(PropertyHelper.GetAlternateName(x), kvp.Key)).PropertyType,
                kvp.Value,
                PropertyValidationLevel,
                schema
            )
        )
        .ToArray();
    }
}

public class OpenApiParamBasis(
    string openApiPropertyName,
    string openApiHeirarchy,
    string? propertyName,
    string currentObjectHeirarchy,
    Type objectType,
    OpenApiOperation operation,
    OpenApiSchema[] schemas,
    OpenApiParameter? parameterSchema,
    OpenApiRequestBody? requestBody,
    bool disableArray
    )
{
    public string OpenApiPropertyName => openApiPropertyName;
    public string OpenApiHeirarchy => string.IsNullOrWhiteSpace(openApiHeirarchy)
        ? OpenApiPropertyName
        : openApiHeirarchy + "." + OpenApiPropertyName;
    public string CurrentObjectHeirarchy => string.IsNullOrWhiteSpace(propertyName)
        ? currentObjectHeirarchy 
        : currentObjectHeirarchy + "." + propertyName; 
    public Type ObjectType => objectType;
    public OpenApiSchema[] Schemas => schemas;
    public OpenApiParameter? ParameterSchema  => parameterSchema;
    public OpenApiRequestBody? RequestBody => requestBody;

    public string PropertyName { get; } = propertyName;

    private PopValidationArray? SchemaValidationArray = null;
    private PopValidationArray? OperationValidationArray = null;
    
    private readonly OpenApiOperation operation = operation;
    private readonly bool disableArray = disableArray;

    public PopValidationArray? GetValidationArray(ValidationLevel level, string extension, string? groupHeader)
    {
        if (disableArray) return null;

        if (level.HasFlag(ValidationLevel.ValidationAttributeInBase))
        {
            OperationValidationArray ??= PopValidationArray.From(extension, operation.Extensions, OpenApiHeirarchy);

            OperationValidationArray.SetLineHeader(groupHeader);

            return OperationValidationArray;
        }

        if (level.HasFlag(ValidationLevel.ValidationAttribute))
        {
            SchemaValidationArray ??= PopValidationArray.From(extension, operation.Extensions, OpenApiPropertyName);
            SchemaValidationArray.SetLineHeader(groupHeader);
            //foreach (var schema in Schemas)
            //{
            //    SchemaValidationArray ??= PopValidationArray.From(extension, schema.Extensions, OpenApiPropertyName);
            //    SchemaValidationArray.SetLineHeader(groupHeader);
            //}
            return SchemaValidationArray;
        }

        return null;
    }

    public OpenApiPropertyBasis[] GetPropertyBases(PopApiOpenApiConfig config)
    {
        ValidationLevel PropertyValidationLevel = ValidationLevel.None;
        //var propType = property.ObjectType baseData.Config.GetPropertyType.Invoke(config, childType ?? owner, fieldName);
        if (ObjectType != null)
        {
            PropertyValidationLevel = (config.TypeValidationLevel?.Invoke(ObjectType) ?? ValidationLevel.FullDetails);
        }

        PropertyValidationLevel = CalculateOverride(PropertyValidationLevel, PropertyValidationLevel);

        return schemas.SelectMany(s => OpenApiPropertyBasis.From(
            operation, 
            s, 
            OpenApiPropertyName, 
            ObjectType, 
            CurrentObjectHeirarchy, 
            PropertyValidationLevel
        )).ToArray();
    }

    private static ValidationLevel CalculateOverride(ValidationLevel? validationLevelOverride, ValidationLevel objLevel)
    {
        if (validationLevelOverride == null) return objLevel;
        if (validationLevelOverride > objLevel) return objLevel;

        return validationLevelOverride.Value;
    }
}

public class OpenApiParamNavigator
{
    private readonly OpenApiOperation operation;

    public OpenApiParamNavigator(
        OpenApiOperation operation,
        ParameterInfo parameterInfo,
        string? openApiParameterName,
        OpenApiSchema[] schemas,
        OpenApiParameter? parameterSchema,
        OpenApiRequestBody? requestBody,
        string? parameterName
    )
    {
        this.operation = operation;
        ParameterInfo = parameterInfo;
        OpenApiParameterName = openApiParameterName;
        Schemas = schemas;
        ParameterSchema = parameterSchema;
        RequestBody = requestBody;
        ParameterName = parameterName;
    }

    public string? OpenApiParameterName { get; }
    public Type ObjectType => TypeHelper.GetIndividualType(ParameterInfo.ParameterType);
    public OpenApiSchema[] Schemas { get; }
    public OpenApiParameter? ParameterSchema { get; }
    public OpenApiRequestBody? RequestBody { get; }
    public string? ParameterName { get; }
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

    public IEnumerable<OpenApiParamBasis> GetParamBases(string prefix, string oridinalIndicator)
    {
        //var name = (ParameterSchema.In == null) ?null : ;
        //var curObjHeirarchy = prefix +"."+ ParameterName;
        if (OpenApiParameterName is not null && ParameterSchema is not null)
        {
            yield return new OpenApiParamBasis(
                openApiPropertyName: OpenApiParameterName,
                openApiHeirarchy: string.Empty,
                propertyName: ParameterInfo.Name,
                currentObjectHeirarchy: prefix,
                objectType: ObjectType, 
                operation: operation,
                schemas: Schemas,
                parameterSchema: ParameterSchema, 
                requestBody: RequestBody,
                disableArray: false
            );

            if (ParameterSchema.Schema.Items is not null)
            {
                yield return new OpenApiParamBasis(
                    openApiPropertyName: OpenApiParameterName + oridinalIndicator,
                    openApiHeirarchy: string.Empty,
                    propertyName: ParameterInfo.Name,
                    currentObjectHeirarchy: prefix,
                    objectType: ObjectType,
                    operation: operation,
                    schemas: Schemas,
                    parameterSchema: ParameterSchema,
                    requestBody: RequestBody,
                    disableArray: false
                );
            }
        }

        if (RequestBody is not null)
        {
            foreach (var key in RequestBody.Content.Keys)
            {
                yield return new OpenApiParamBasis(
                    openApiPropertyName: "RequestBody",
                    openApiHeirarchy: null,
                    propertyName: null, //ParameterInfo.Name,
                    currentObjectHeirarchy: prefix,
                    objectType: ObjectType,
                    operation: operation,
                    schemas: [RequestBody.Content[key].Schema],
                    parameterSchema: ParameterSchema,
                    requestBody: RequestBody,
                    disableArray: RequestBody.Content.Keys.First() != key
                );

                if (RequestBody.Content[key].Schema.Items is not null)
                {
                    yield return new OpenApiParamBasis(
                        openApiPropertyName: "RequestBody" + oridinalIndicator,
                        openApiHeirarchy: null,
                        propertyName: null, //ParameterInfo.Name,
                        currentObjectHeirarchy: prefix,
                        objectType: ObjectType,
                        operation: operation,
                        schemas: [RequestBody.Content[key].Schema],
                        parameterSchema: ParameterSchema,
                        requestBody: RequestBody,
                        disableArray: RequestBody.Content.Keys.First() != key
                    );
                }
            }
        }
    }

    //internal IEnumerable<OpenApiPropertyBasis> GetPropertyBases(string prefix, string oridinalIndicator)
    //{
    //    foreach(var p in GetParamBases(prefix, oridinalIndicator))
    //    {
    //        foreach (var schema in p.Schemas)
    //        {
    //            foreach (var property in schema.Properties)
    //            {
    //                yield return null;
    //                //    new OpenApiPropertyBasis(
    //                //    openApiPropertyName: property.Key,
    //                //    currentObjectHeirarchy: p.CurrentObjectHeirarchy + "." + ,
    //                //    objectType: ,
    //                //    schema: schema
    //                //);
    //            }
    //        }
    //    }
    //}
}

public static class TypeHelper
{
    public static bool IsComplexType(Type type)
    {
        return !type.IsPrimitive && type != typeof(string) && !type.IsValueType;
    }

    public static Type GetIndividualType(Type type)
    {
        if (IsEnumerableType(type))
        {
            //if (IsDictionaryType(type))
            //{
            //    if (type.IsGenericType)
            //    {
            //        return ty
            //    }
            //}
            //else 
            if (type.IsGenericType)
            {
                return type.GetGenericArguments()[0];
            }
        }
        return type;
    }

    private static bool IsEnumerableType(Type type)
    {
        return typeof(IEnumerable).IsAssignableFrom(type);
    }

    private static bool IsDictionaryType(Type type)
    {
        return typeof(IDictionary).IsAssignableFrom(type);
    }
}

public static class PropertyHelper
{
    public static PropertyInfo[] GetProperties(Type type)
    {
        return type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
    }

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