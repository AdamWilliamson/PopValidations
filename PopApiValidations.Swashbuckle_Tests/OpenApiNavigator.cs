using FluentAssertions;
using Newtonsoft.Json.Linq;
using PopApiValidations.Swashbuckle;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;

namespace PopApiValidations.Swashbuckle_Tests;

//public class Request_CreationValidation : ApiSubValidator<Request>
//{
//    public Request_CreationValidation()
//    {
//        Describe(x => x.IntegerField).Vitally().IsEqualTo(0);
//    }
//}

public class OpenApiNavigator(PopApiOpenApiConfig config, JObject openApi)
{
    private readonly PopApiOpenApiConfig config = config;

    //public JObject OpenApi { get; } = openApi;

    //public JToken? Position { get; set; } = null;


    //public JObject GetPaths()
    //{
    //    return (JObject)OpenApi["paths"];
    //}

    //public JObject GetDefinitions()
    //{
    //    return (JObject)OpenApi["components"]?["schemas"];
    //}

    //public JObject GetOperation(string path, string method)
    //{
    //    var paths = GetPaths();
    //    if (paths != null && paths[path] != null)
    //    {
    //        return (JObject)paths[path][method.ToLower()];
    //    }

    //    return null;
    //}

    //public OpenApiNavigator Path(string url)
    //{
    //    Position = openApi?["paths"]?[url];
    //    return this;
    //}

    //public OpenApiNavigator Type(string type)
    //{
    //    Position = Position[type];
    //    return this;
    //}

    public Dictionary<string, JObject> Parameters(string url, string type)
    {
        return openApi?["paths"]?[url]?[type]["parameters"]
                ?.Where(x => x is not null && x as JObject is not null && x.Value<string>("name") is not null)
                ?.ToDictionary(x => x.Value<string>("name"), x => x as JObject)
            ?? new();
    }

    public JObject? Parameter(string url, string type, string paramName)
    {
        var parameters = Parameters(url, type);

        if (parameters is not null && parameters.ContainsKey(paramName))
            return Parameters(url, type)[paramName];

        return null;
    }

    public JObject GetParamValidationOwner(string url, string type, string paramName)
    {
        var parameter = Parameter(url, type, paramName);
        if (parameter is JObject convertedParam)
        {
            return convertedParam;
        }

        return openApi["paths"]?[url]?[type]?["requestBody"] as JObject;
    }

    public List<JObject> GetParamValidationObjectSchemas(string url, string type, string paramName)
    {
        var parameter = Parameter(url, type, paramName);
        if (parameter is JObject convertedParam)
        {
            return new List<JObject>() { convertedParam["schema"] as JObject };
        }

        return openApi["paths"]?[url]?[type]?["requestBody"]?["content"]
            ?.Values()
            .Select(x => x["schema"]).Cast<JObject>().ToList() ?? new();
    }

    public List<JObject> GetParamValidationObjectSchemas(string url, string type, string[] objHeirarchy)
    {
        var schemas = GetParamValidationObjectSchemas(url, type, objHeirarchy[0]);

        foreach(var nextProperty in objHeirarchy.Skip(1))
        {
            schemas = GoToChild(schemas, nextProperty);
        }

        return schemas;
    }

    public List<JObject> GetParamPropertyValidationObjects(string url, string type, string paramName)
    {
        var schemas = GetParamValidationObjectSchemas(url, type, paramName);

        foreach(var schema in schemas)
        {
            if (schema[config.CustomValidationAttribute] == null)
            {
                schema[config.CustomValidationAttribute] = new JObject();
            }
        }

        return schemas.Select(x => x[config.CustomValidationAttribute]).Cast<JObject>().ToList();
    }


    public List<JObject> GoToChild(List<JObject> items, string chidProperty)
    {
        return items.Select(x => x["properties"][chidProperty]).Cast<JObject>().ToList();
    }

    public List<JObject> GoToChild(List<JObject> items, string[] chidProperties)
    {
        var nextItems = items.ToList();

        foreach(var prop in chidProperties)
        {
            nextItems = nextItems.Select(x => x["properties"][prop]).Cast<JObject>().ToList();
        }

        return nextItems;
    }

    public List<JObject> GoToSchema(string url, string type, string[] childProperties)
    {
        var restOftheProps = childProperties.Skip(1).ToList();
        var baseObject = childProperties[0];

        var schemas = GetParamValidationObjectSchemas(url, type, baseObject);

        return GoToChild(schemas, restOftheProps.ToArray());
    }

    public JArray GetPathValidations(string url, string type, string paramName)
    {
        var path = openApi?["paths"]?[url]?[type];

        if (path[config.CustomValidationAttribute] is null)
        {
            path[config.CustomValidationAttribute] = new JObject();
        }

        var vobj = path[config.CustomValidationAttribute] as JObject;

        if (vobj[paramName] is null)
        {
            (vobj[paramName] as JObject).Add(paramName, new JArray());
        }

        return vobj[paramName][paramName] as JArray;
    }

    public JObject GetPathValidationObject(string url, string type)
    {
        var path = openApi?["paths"]?[url]?[type];

        if (path[config.CustomValidationAttribute] is null)
        {
            path[config.CustomValidationAttribute] = new JObject();
        }

        return path[config.CustomValidationAttribute] as JObject;
    }

    //public List<JObject> GetObjectParamValidations(string url, string type, string paramName)
    //{
    //    var schemas = GetParamSchemas(url, type, paramName);

    //    foreach(var schema in schemas)
    //    {
    //        if (schema[config.CustomValidationAttribute] is null)
    //        {
    //            schema[config.CustomValidationAttribute] = new JObject();
    //        }

    //        //return new() { schema[config.CustomValidationAttribute] as JObject };
    //    }

    //    return schemas.Select(x => x[config.CustomValidationAttribute] as JObject).ToList();
    //}

    //public List<JArray> GetArrayParamValidations(string url, string type, string paramName)
    //{
    //    var schemas = GetParamSchemas(url, type, paramName);

    //    foreach (var schema in schemas)
    //    {
    //        if (schema[config.CustomValidationAttribute] is null)
    //        {
    //            schema[config.CustomValidationAttribute] = new JArray();
    //        }

    //        //return new() { schema[config.CustomValidationAttribute] as JArray };
    //    }

    //    return schemas.Select(x => x[config.CustomValidationAttribute] as JArray).Where(x => x != null).ToList();
    //}

    public List<JArray> GetParamValidationsArray(string url, string type, string paramName)
    {
        //var schemas = GetParamSchemas(url, type, paramName);
        var validationAttributes = new List<JArray>();

        if (Parameter(url, type, paramName) is not null) 
        {
            var pathObj = openApi?["paths"]?[url]?[type];

            if (pathObj[config.CustomValidationAttribute] is null)
            {
                (pathObj as JObject).Add(config.CustomValidationAttribute, new JObject());
            }

            if (pathObj[config.CustomValidationAttribute][paramName] is null)
            {
                (pathObj[config.CustomValidationAttribute] as JObject).Add(paramName, new JArray());
            }

            validationAttributes.Add(pathObj[config.CustomValidationAttribute][paramName] as JArray);
        }


        var requestBody = openApi["paths"]?[url]?[type]?["requestBody"];

        if (!validationAttributes.Any() && requestBody is not null)
        {
            if (openApi["paths"]?[url]?[type]?["requestBody"][config.CustomValidationAttribute] is null)
            {
                (openApi["paths"]?[url]?[type]?["requestBody"] as JObject).Add(config.CustomValidationAttribute, new JArray());
            }

            validationAttributes.Add(openApi["paths"]?[url]?[type]?["requestBody"][config.CustomValidationAttribute] as JArray);
        }

        return validationAttributes;
    }


    //public JArray GetOrCreateParamValidation(string url, string type, string paramName)
    //{
    //    var parameter = Parameter(url, type, paramName);
    //    if (parameter is not null)
    //    {
    //        var api = openApi?["paths"]?[url]?[type];
    //        if (api[config.CustomValidationAttribute] is null)
    //        {
    //            api[config.CustomValidationAttribute] = new JObject();
    //        }

    //        if (api[config.CustomValidationAttribute][paramName] is null)
    //        {
    //            api[config.CustomValidationAttribute][paramName] = new JArray();
    //        }

    //        return api[config.CustomValidationAttribute][paramName] as JArray;
    //    }

    //    if (openApi
    //            ["paths"]?
    //            [url]?
    //            [type]?
    //            ["requestBody"] is null)
    //    {
    //        openApi
    //            ["paths"]
    //            [url]
    //            [type]
    //            ["requestBody"] = new JArray();
    //    }

    //    return openApi
    //            ["paths"]?
    //            [url]?
    //            [type]?
    //            ["requestBody"] as JArray;
    //}

}
