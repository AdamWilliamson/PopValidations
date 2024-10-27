using ApprovalTests;
using FluentAssertions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PopApiValidations.Swashbuckle;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using Xunit.Abstractions;

namespace PopApiValidations.Swashbuckle_Tests;

//public class Request_CreationValidation : ApiSubValidator<Request>
//{
//    public Request_CreationValidation()
//    {
//        Describe(x => x.IntegerField).Vitally().IsEqualTo(0);
//    }
//}

public record Pair<T>(AssertionResult Results, JObject OpenApiBase, JObject CleanBase, T OpenApi, T Clean);

public class AssertionResult
{
    public bool Success { get; private set; } = true;

    public void SetResult(bool result)
    {
        if (result) return;
        Success = false;
    }
}

public static class NavExtensions
{
    public static Pair<TOut> Nav<TIn, TOut>(this Pair<TIn> start, Func<TIn, TOut?> navFunc)
    {
            return new Pair<TOut>(
            start.Results,
            start.OpenApiBase,
            start.CleanBase,
            navFunc.Invoke(start.OpenApi) ?? throw new Exception("Validated OpenApi navigation failed."),
            navFunc.Invoke(start.Clean) ?? throw new Exception("Clean OpenApi navigation failed.")
        );
    }

    public static Pair<List<TOut>> NavList<TIn, TOut>(this Pair<List<TIn>> start, Func<TIn, TOut?> navFunc)
    {
        return new Pair<List<TOut>>(
            start.Results,
            start.OpenApiBase,
            start.CleanBase,
            start.OpenApi.Select(x => navFunc.Invoke(x) ?? throw new Exception("Validated OpenApi navigation failed.")).ToList(),
            start.Clean.Select(x => navFunc.Invoke(x) ?? throw new Exception("Clean OpenApi navigation failed.")).ToList()
        );
    }

    public static Pair<TOut?> NavNullable<TIn, TOut>(this Pair<TIn> start, Func<TIn, TOut?> navFunc)
    {
        return new Pair<TOut?>(
            start.Results,
            start.OpenApiBase,
            start.CleanBase,
            navFunc.Invoke(start.OpenApi),
            navFunc.Invoke(start.Clean)
        );
    }

    public static bool CanNav<TIn, TOut>(this Pair<TIn> start, Func<TIn, TOut?> navFunc)
    {
        try
        {
            var result = navFunc.Invoke(start.OpenApi);
            return result != null;
        }
        catch
        {
            return false;
        }
    }

    public static Pair<TIn> Modify<TIn>(this Pair<TIn> start, Action<TIn> navFunc)
    {
        navFunc.Invoke(start.Clean);
        return start;
    }

    public static Pair<List<TIn>> ModifyList<TIn>(this Pair<List<TIn>> start, Action<TIn> modifyFunc)
        where TIn : JToken
    {
        start.Modify(list => list.ForEach(listItem => modifyFunc.Invoke(listItem)));

        return start;
    }

    public static Pair<List<TIn>> ModifyList<TIn>(this Pair<List<TIn>> start, params (string, Func<JToken> creation)[] tree)
        where TIn: JToken
    {
        start.Modify(list =>
            list.ForEach(listItem => {
                JToken current = listItem;

                foreach (var child in tree)
                {
                    current = Add(current, child.Item1, child.Item2);
                }
            })
        );

        return start;
    }

    internal static JToken Add(JToken item, string prop, Func<JToken> creation)
    {
        if (item is JObject obj)
        {
            if (obj[prop] == null)
            {
                obj.Add(prop, creation.Invoke());
            }

            return obj[prop];
        }
        else if (item is JArray arr)
        {
            var created = creation.Invoke();
            if (!arr.Contains(created))
            {
                arr.Add(created);
            }
            return created;
        }

        throw new Exception("What is this?");
    }

    internal static void Remove(JToken item, string propOrValue)
    {
        if (item is JObject obj)
        {
            if (obj[propOrValue] != null)
            {
                obj.Remove(propOrValue);
            }
        }
        else if (item is JArray arr)
        {
            if (arr.Contains(propOrValue))
            {
                arr.Remove(propOrValue);
            }
        }
    }

    

    public static Pair<TIn> Modify<TIn>(this Pair<TIn> start, params (string, Func<JToken> creation)[] tree)
        where TIn : JToken
    {
        JToken current = start.Clean;

        foreach(var child in tree)
        {
            current = Add(current, child.Item1, child.Item2);
        }
        
        return start;
    }

    public static Pair<TIn> ModifyRemove<TIn>(this Pair<TIn> start, params string[] properties)
    where TIn : JToken
    {
        JToken current = start.Clean;

        foreach (var prop in properties)
        {
            Remove(current, prop);
        }

        return start;
    }

    internal static Pair<List<TIn>> ModifyListRemove<TIn>(this Pair<List<TIn>> start, params string[] properties)
        where TIn : JToken
    {
        start.Clean.ForEach(i =>
        {
            foreach (var prop in properties)
            {
                Remove(i, prop);
            }
        });

        return start;
    }

    public static Pair<TIn> IfNullReplaceNav<TIn>(this Pair<TIn> start, Func<Pair<TIn>> navFunc)
    {
        if (start.OpenApi != null) return start;

        var result = navFunc.Invoke();
        return new Pair<TIn>(
            start.Results,
            start.OpenApiBase,
            start.CleanBase,
            result.OpenApi,
            result.Clean
        );
    }

    public static Pair<TIn> Assert<TIn>(this Pair<TIn> start, Action<TIn, AssertionResult> assertion)
    {
        //var result = new AssertionResult();
        try
        {
            assertion?.Invoke(start.OpenApi, start.Results);
        }
        catch
        {
            start.Results.SetResult(false);
        }

        //if (!result.Success)
        //{
        //    Approvals.AssertEquals(
        //        start.CleanBase.ToString(Formatting.Indented),
        //        start.OpenApiBase.ToString(Formatting.Indented)
        //    );
        //}

        return start;
    }

    public static Pair<List<TIn>> AssertList<TIn>(this Pair<List<TIn>> start, Action<TIn, AssertionResult> assertion)
    {
        foreach (var item in start.OpenApi)
        {
            assertion?.Invoke(item, start.Results);

            //if (!result.Success)
            //{
            //    Approvals.AssertEquals(
            //        start.CleanBase.ToString(Formatting.Indented),
            //        start.OpenApiBase.ToString(Formatting.Indented)
            //    );
            //}
        }
        return start;
    }


    //public static Pair<TOut> Nav<TIn, TOut>(this (JToken OpenApi, JToken Clean) start,Func<JToken, JToken> navFunc)
    //{
    //    return (
    //        navFunc.Invoke(start.OpenApi),
    //        navFunc.Invoke(start.Clean)
    //    );
    //}

    public static (JObject OpenApi, JObject Clean) NavToObj(this (JToken OpenApi, JToken Clean) start, Func<JToken, JObject> navFunc)
    {
        return (
            navFunc.Invoke(start.OpenApi),
            navFunc.Invoke(start.Clean)
        );
    }

    public static (JArray OpenApi, JArray Clean) NavToArray(this (JToken OpenApi, JToken Clean) start, Func<JToken, JArray> navFunc)
    {
        return (
            navFunc.Invoke(start.OpenApi),
            navFunc.Invoke(start.Clean)
        );
    }
}

public class OpenApiNavigator(PopApiOpenApiConfig config, AssertionResult results, JObject openApi, JObject clean)
{
    private readonly PopApiOpenApiConfig config = config;

    private Pair<JObject> BasePair()
    {
        return new Pair<JObject>(results, openApi, clean, openApi, clean);
    }

    public Pair<JArray> ParametersPair(string url, string type)
    {
        return GetPath(url, type).Nav(schema =>schema["parameters"] as JArray);
    }

    public Pair<JObject> ParameterPairByName(string url, string type, string[] objHeirarchy)
    {
        return ParametersPair(url, type)
            .Nav(pair => pair!.Single(x => x["name"]!.Value<string>() == objHeirarchy[0].Replace("[n]","")) as JObject);
    }

    public Pair<JObject> ParameterOrItemsByName(string url, string type, string[] objHeirarchy)
    {
        if (objHeirarchy[0].EndsWith("[n]"))
        {
            return ParametersPair(url, type)
            .Nav(pair => pair!.Single(x => x["name"]!.Value<string>() == objHeirarchy[0].Replace("[n]", ""))["schema"]?["items"] as JObject);
        }

        return ParametersPair(url, type)
            .Nav(pair => pair!.Single(x => x["name"]!.Value<string>() == objHeirarchy[0]) as JObject);
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
                    .SingleOrDefault(x => x?["name"]?.Value<string>() == paramName[0].Replace("[n]","")) as JObject
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

    public Pair<JObject> GetParamSchemaPair(string url, string type, string[] objHeirarchy)
    {
        if (CheckIfParameterExists(url, type, objHeirarchy))
        {
            if (objHeirarchy[0].EndsWith("[n]"))
            {
                return ParameterPairByName(url, type, objHeirarchy).Nav(schema => schema["schema"]?["items"] as JObject);
            }
            return ParameterPairByName(url, type, objHeirarchy).Nav(schema => schema["schema"] as JObject);
        }

        throw new Exception("Not a parameter?");
    }

    public Pair<JObject> NavToParameterProperty(string url, string type, params string[] objHeirarchy)
    {
        var pair = GetParamSchemaPair(url, type, objHeirarchy);

        foreach (var nextProperty in objHeirarchy.Skip(1))
        {
            if (nextProperty.EndsWith("[n]"))
            {
                pair = pair.Nav(schema => schema["properties"]?[nextProperty.Replace("[n]", "")]?["items"] as JObject);
            }
            else
            {
                pair = pair.Nav(schema => schema["properties"]?[nextProperty] as JObject);// GoToChild(schemas, nextProperty);
            }
        }

        return pair;
    }

    public Pair<List<JObject>> GetRequestBodySchemasPair(string url, string type, params string[] objHeirarchy)
    {
        if (objHeirarchy is null || objHeirarchy.Length == 0 || !CheckIfParameterExists(url, type, objHeirarchy))
        {
            var pair = GetRequestBodyParentPair(url, type)
                .Nav(schema => schema["content"]
                    ?.Values()
                    ?.Select(x => x["schema"] as JObject)
                    ?.Cast<JObject>()?.ToList()
                );

            if (objHeirarchy is not null && objHeirarchy.Length > 0)
            {
                foreach (var nextProperty in objHeirarchy[0..^1])
                {
                    if (nextProperty.EndsWith("[n]"))
                    {
                        pair = pair.NavList(schema => schema["properties"]?[nextProperty.Replace("[n]", "")]?["items"] as JObject);
                    }
                    else
                    {
                        pair = pair.NavList(schema => schema["properties"]?[nextProperty] as JObject);
                    }
                }
            }

            return pair;
        }

        throw new Exception("Not a requestBody?");
    }

    public Pair<JObject> GetRequestBodyParentPair(string url, string type)
    {
        return BasePair().Nav(schema => schema["paths"]?[url]?[type]?["requestBody"] as JObject);
    }

    public Pair<List<JArray>> GetRequestBodyPropertyValidationArrayPair(string url, string type, params string[] objHeirarchy)
    {
        var property = objHeirarchy[^1];
        return GetRequestBodySchemasPair(url, type, objHeirarchy)
            .ModifyList((config.CustomValidationAttribute, () => new JObject()), (property, () => new JArray()))
            .NavList(schema => schema[property] as JArray);
    }

    public Pair<JObject> GoToParamSchemaPair(string url, string type, string[] childProperties)
    {
        var schema = GetParamSchemaPair(url, type, childProperties);

        if (childProperties.Length > 1)
        {
            foreach (var prop in childProperties[1..^1])
            {
                if (prop.EndsWith("[n]"))
                {
                    schema = schema.Nav(schema => schema["properties"]?[prop.Replace("[n]", "")]?["items"] as JObject);
                }
                else
                {
                    schema = schema.Nav(schema => schema["properties"]?[prop] as JObject);
                }
            }
        }

        return schema;
    }

    public Pair<JArray> GetPathValidationsPair(string url, string type, string paramName)
    {
        return BasePair()
            .Nav(schema =>  schema?["paths"]?[url]?[type])
            .Modify(clean =>
            {
                if (clean[config.CustomValidationAttribute] is null)
                {
                    clean[config.CustomValidationAttribute] = new JObject();
                }

                var vobj = clean[config.CustomValidationAttribute] as JObject;

                if (vobj?[paramName] is null)
                {
                    (vobj?[paramName] as JObject)?.Add(paramName, new JArray());
                }
            })
            .Nav(schema => schema?[config.CustomValidationAttribute]?[paramName]?[paramName] as JArray);
    }

    public Pair<JObject> GetPathValidationObjectPair(string url, string type)
    {
        return BasePair().Nav(schema => schema?["paths"]?[url]?[type])
            .Modify(clean =>
            {
                if (clean[config.CustomValidationAttribute] is null)
                {
                    clean[config.CustomValidationAttribute] = new JObject();
                }
            })
            .Nav(schema => schema[config.CustomValidationAttribute] as JObject);
    }

    public Pair<List<JArray>> GetParamOrRequestBodyPropertyValidationArrayPair(string url, string type, params string[] objHeirarchy)
    {
        var endParamName = objHeirarchy[^1];

        if (CheckIfParameterExists(url, type, objHeirarchy))
        {
            return GoToParamSchemaPair(url, type, objHeirarchy[..^1])
                .Modify((config.CustomValidationAttribute, () => new JObject()), (endParamName, () => new JArray()))
                .Nav(schema => 
                    new List<JArray>() { schema?[config.CustomValidationAttribute]?[endParamName] as JArray }
                );
        }

        if (objHeirarchy.Length == 1)
        {
            return GetRequestBodyParentPair(url, type)
                .Modify((config.CustomValidationAttribute, () => new JArray()))
                .Nav(schema => new List<JArray>() { schema[config.CustomValidationAttribute] as JArray });
        }
        else
        {
            return GetRequestBodySchemasPair(url, type, objHeirarchy)
                .ModifyList(
                    (config.CustomValidationAttribute, () => new JObject()),
                    (endParamName, () => new JArray())
                )
                .NavList(schema => schema[endParamName] as JArray);
        }
    }
}
