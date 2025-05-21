using Newtonsoft.Json.Linq;

namespace PopApiValidations.Swashbuckle_Tests.Helpers;

public record Pair<T>(AssertionResult Results, JObject OpenApiBase, JObject CleanBase, T OpenApi, T Clean);
public record Pair2(AssertionResult Results, JObject OpenApiBase, JObject CleanBase, List<JObject> OpenApi, List<JObject> Clean);

public class AssertionResult
{
    public string FocusName { get; private set; }
    public bool Success { get; private set; } = true;
    public List<string> Errors { get; private set; } = new();

    public void SetResult(bool result)
    {
        if (result) return;
        Success = false;
    }

    public void SetResult(bool result, string? error)
    {
        if (result) return;
        Success = false;

        if (string.IsNullOrWhiteSpace(error)) return;

        Errors.Add(error);
    }

    public void SetFocusName(string focusName)
    {
        FocusName = focusName;
    }
}

public class OpenApiNavigator(AssertionResult results, JObject openApi, JObject clean)
{
    private Pair<JObject> BasePair()
    {
        return new Pair<JObject>(results, openApi, clean, openApi, clean);
    }

    private Pair2 BasePair2()
    {
        return new Pair2(results, openApi, clean, new (){ openApi }, new (){ clean });
    }

    public Pair2 Parameter2(string url, string type, string name)
    {
        return GetPath2(url, type)
            .Nav(schema => (schema["parameters"] as JArray)?.Single(x => x["name"]!.Value<string>() == name) as JObject);
    }

    public Pair2 ParameterPairByName2(string url, string type, string[] objHeirarchy)
    {
        return Parameter2(url, type, objHeirarchy[0].Replace("[n]", ""));
    }

    public bool CheckIfParameterExists(string url, string type, string[]? paramName)
    {
        if (paramName is null || paramName.Length == 0)
        {
            return false;
        }

        return GetPath(url, type)
            .CanNav(pair =>
                pair["parameters"]?
                    .SingleOrDefault(x => x?["name"]?.Value<string>() == paramName[0].Replace("[n]", "")) as JObject
            );
    }

    public bool CheckIfRequestBody(string url, string type, string[]? paramName)
    {
        if (CheckIfParameterExists(url, type, paramName)) return false;
        return GetPath(url, type).CanNav(schema => schema["requestBody"] as JObject);
    }

    public Pair<JObject> GetPath(string url, string type)
    {
        return BasePair().Nav(pair => pair["paths"]?[url]?[type] as JObject);
    }

    public Pair2 GetPath2(string url, string type)
    {
        return BasePair2().Nav(pair => pair["paths"]?[url]?[type] as JObject);
    }

    public Pair2 GetParamSchemaPair2(string url, string type, string[] objHeirarchy)
    {
        if (CheckIfParameterExists(url, type, objHeirarchy))
        {
            if (objHeirarchy[0].EndsWith("[n]"))
            {
                return ParameterPairByName2(url, type, objHeirarchy).Nav(schema => schema["schema"]?["items"] as JObject);
            }

            return ParameterPairByName2(url, type, objHeirarchy).Nav(schema => schema["schema"] as JObject);
        }

        throw new Exception("Not a parameter?");
    }

    public Pair2 NavToParameterProperty2(string url, string type, params string[] objHeirarchy)
    {
        var pair = GetParamSchemaPair2(url, type, objHeirarchy);

        foreach (var nextProperty in objHeirarchy.Skip(1))
        {
            if (nextProperty.EndsWith("[n]"))
            {
                pair = pair.Nav(schema => schema["properties"]?[nextProperty.Replace("[n]", "")]?["items"] as JObject);
            }
            else
            {
                pair = pair.Nav(schema => schema["properties"]?[nextProperty] as JObject);
            }
        }

        return pair;
    }

    public Pair2 GetRequestBodyParentPair2(string url, string type)
    {
        return BasePair2().Nav(schema => schema["paths"]?[url]?[type] as JObject);
    }

    public Pair2 GetRequestBodyChildrenPair2(string url, string type)
    {
        var result = BasePair2()
            .Nav(schema => schema["paths"]?[url]?[type]?["requestBody"] as JObject)
            .Nav(
                schema => schema["content"]["application/json"]["schema"] as JObject,
                schema => schema["content"]["text/json"]["schema"] as JObject,
                schema => schema["content"]["application/*+json"]["schema"] as JObject
            );

        return result;
    }
}
