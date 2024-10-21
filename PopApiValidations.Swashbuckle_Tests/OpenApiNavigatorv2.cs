using FluentAssertions;
using PopApiValidations.Swashbuckle;
using System.Text.Json.Nodes;

namespace PopApiValidations.Swashbuckle_Tests;

public class OpenApiNavigatorv2(PopApiOpenApiConfig config, JsonObject openApi)
{
    private readonly PopApiOpenApiConfig config = config;

    public Dictionary<string, JsonObject> Parameters(string url, string type)
    {
        return openApi?["paths"]?[url]?[type]?["parameters"]?.AsArray()
                .Cast<JsonObject>()
                ?.Where(x => x["name"] is not null)
                ?.ToDictionary(x => x["name"].As<string>(), x => x.AsObject())
            ?? new();
    }

    public JsonObject? Parameter(string url, string type, string paramName)
    {
        var parameters = Parameters(url, type);

        if (parameters is not null && parameters.ContainsKey(paramName))
            return Parameters(url, type)[paramName];

        return null;
    }

    public JsonObject? GetParamValidationOwner(string url, string type, string paramName)
    {
        var parameter = Parameter(url, type, paramName);
        if (parameter is JsonObject convertedParam)
        {
            return convertedParam;
        }

        return openApi["paths"]?[url]?[type]?["requestBody"]?.AsObject();
    }

    public List<JsonObject> GetParamObjects(string url, string type, string paramName)
    {
        var parameter = Parameter(url, type, paramName);
        if (parameter is JsonObject convertedParam)
        {
            return new List<JsonObject>() { convertedParam.AsObject() };
        }

        return openApi["paths"]?[url]?[type]?["requestBody"]?["content"]?.AsArray()
            .Cast<JsonObject>().ToList() 
            ?? new();
    }

    public List<JsonObject> GetParamValidationObjectSchemas(string url, string type, string paramName)
    {
        var parameter = Parameter(url, type, paramName);
        if (parameter is JsonObject convertedParam && convertedParam["schema"]?.GetValueKind() == System.Text.Json.JsonValueKind.Object)
        {
            return new List<JsonObject>() { convertedParam["schema"]!.AsObject() };
        }

        return openApi["paths"]?[url]?[type]?["requestBody"]?["content"]?.AsArray()
            .Select(x => x?["schema"]).Cast<JsonObject>().ToList() 
            ?? new();
    }

    public List<JsonObject> GetParamValidationObjectSchemas(string url, string type, string[] objHeirarchy)
    {
        var schemas = GetParamValidationObjectSchemas(url, type, objHeirarchy[0]);

        foreach(var nextProperty in objHeirarchy.Skip(1))
        {
            schemas = GoToChild(schemas, nextProperty);
        }

        return schemas;
    }

    public List<JsonObject> GetParamPropertyValidationObjects(string url, string type, string paramName)
    {
        var schemas = GetParamValidationObjectSchemas(url, type, paramName);

        foreach(var schema in schemas)
        {
            if (schema[config.CustomValidationAttribute] == null)
            {
                schema[config.CustomValidationAttribute] = new JsonObject();
            }
        }

        return schemas.Select(x => x[config.CustomValidationAttribute]).Cast<JsonObject>().ToList();
    }

    public List<JsonObject> GoToChild(List<JsonObject> items, string chidProperty)
    {
        return items.Select(x => x["properties"]?[chidProperty]).Cast<JsonObject>().ToList();
    }

    public List<JsonObject> GoToChild(List<JsonObject> items, string[] chidProperties)
    {
        var nextItems = items.ToList();

        foreach(var prop in chidProperties)
        {
            nextItems = nextItems.Select(x => x["properties"]?[prop]).Cast<JsonObject>().ToList();
        }

        return nextItems;
    }

    public List<JsonObject> GoToSchema(string url, string type, string[] childProperties)
    {
        var restOftheProps = childProperties.Skip(1).ToList();
        var baseObject = childProperties[0];

        var schemas = GetParamValidationObjectSchemas(url, type, baseObject);

        return GoToChild(schemas, restOftheProps.ToArray());
    }

    public JsonArray? GetPathValidations(string url, string type, string paramName)
    {
        var path = openApi?["paths"]?[url]?[type];

        if (path == null) return null;

        if (path[config.CustomValidationAttribute] is null)
        {
            path[config.CustomValidationAttribute] = new JsonObject();
        }

        var vobj = path[config.CustomValidationAttribute]?.AsObject();

        if (vobj == null) return null;

        if (vobj[paramName] is JsonObject valAttr)
        {
            valAttr.Add(paramName, new JsonObject());
        }

        return vobj[paramName]?[paramName]?.AsArray();// as JsonArray;
    }

    public JsonObject? GetPathValidationObject(string url, string type)
    {
        var path = openApi?["paths"]?[url]?[type];

        if (path == null) return null;

        if (path[config.CustomValidationAttribute] is null)
        {
            path[config.CustomValidationAttribute] = new JsonObject();
        }

        return path[config.CustomValidationAttribute]?.AsObject();
    }

    public List<JsonArray> GetParamValidationsArray(string url, string type, string paramName)
    {
        //var schemas = GetParamSchemas(url, type, paramName);
        var validationAttributes = new List<JsonArray>();

        if (Parameter(url, type, paramName) is not null) 
        {
            var pathObj = openApi?["paths"]?[url]?[type]?.AsObject();

            if (pathObj == null) return new ();

            if (pathObj[config.CustomValidationAttribute] is null)
            {
                pathObj.Add(config.CustomValidationAttribute, new JsonObject());
            }

            var validationAttr = pathObj[config.CustomValidationAttribute]!.AsObject();

            if (validationAttr[paramName] is null)
            {
                validationAttr.Add(paramName, new JsonArray());
            }

            validationAttributes.Add(validationAttr[paramName]!.AsArray());
        }


        var requestBody = openApi?["paths"]?[url]?[type]?["requestBody"]?.AsObject();

        if (!validationAttributes.Any() && requestBody is not null)
        {
            if (requestBody[config.CustomValidationAttribute] is null)
            {
                requestBody.Add(config.CustomValidationAttribute, new JsonArray());
            }

            validationAttributes.Add(requestBody[config.CustomValidationAttribute]!.AsArray());
        }

        return validationAttributes;
    }
}
