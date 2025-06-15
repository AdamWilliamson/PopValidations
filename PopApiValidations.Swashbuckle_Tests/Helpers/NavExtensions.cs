using Newtonsoft.Json.Linq;

namespace PopApiValidations.Swashbuckle_Tests.Helpers;

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

    //public static Pair2 Nav<TIn, TOut>(this Pair2 start, Func<TIn, TOut?> navFunc)
    //{
    //    return new Pair2(
    //        start.Results,
    //        start.OpenApiBase,
    //        start.CleanBase,
    //        navFunc.Invoke(start.OpenApi) ?? throw new Exception("Validated OpenApi navigation failed."),
    //        navFunc.Invoke(start.Clean) ?? throw new Exception("Clean OpenApi navigation failed.")
    //    );
    //}

    public static Pair2 Nav(this Pair2 start, Func<JObject, JObject?> navFunc)
    {
        return new Pair2(
            start.Results,
            start.OpenApiBase,
            start.CleanBase,
            start.OpenApi.Select(o => navFunc.Invoke(o) ?? throw new Exception("Validated OpenApi navigation failed.")).ToList(),
            start.Clean.Select(o => navFunc.Invoke(o) ?? throw new Exception("Clean OpenApi navigation failed.")).ToList()
        );
    }

    public static Pair2 Nav(this Pair2 start, Func<JObject, List<JObject?>> navFunc)
    {
        return new Pair2(
            start.Results,
            start.OpenApiBase,
            start.CleanBase,
            start.OpenApi.SelectMany(o => navFunc.Invoke(o) ?? throw new Exception("Validated OpenApi navigation failed.")).ToList(),
            start.Clean.SelectMany(o => navFunc.Invoke(o) ?? throw new Exception("Clean OpenApi navigation failed.")).ToList()
        );
    }

    public static Pair2 Nav(this Pair2 start, params Func<JObject, JObject?>[] navFuncs)
    {
        return new Pair2(
            start.Results,
            start.OpenApiBase,
            start.CleanBase,
            navFuncs.SelectMany(fnc => start.OpenApi.Select((o, i) => fnc.Invoke(o) ?? throw new Exception("Validated OpenApi navigation failed."))).ToList(),
            navFuncs.SelectMany(fnc => start.Clean.Select((o, i) => fnc.Invoke(o) ?? throw new Exception("Clean OpenApi navigation failed."))).ToList()
        );
    }

    private static string ToLowerFirstChar(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return char.ToLower(input[0]) + input.Substring(1);
    }

    public static Pair2 Nav(this Pair2 start, string[] childProperties)
    {
        Pair2 temp = start;

        foreach (var prop in childProperties.Select(x => ToLowerFirstChar(x)))
        {
            if (prop.EndsWith("[n]"))
            {
                temp = temp.Nav(schema => schema["properties"]?[prop.Replace("[n]", "")]?["items"] as JObject);
            }
            else
            {
                temp = temp.Nav(schema => schema["properties"]?[prop] as JObject);
            }
        }

        return temp;
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

    //public static Pair<TIn> Modify<TIn>(this Pair<TIn> start, Action<TIn> navFunc)
    //{
    //    navFunc.Invoke(start.Clean);
    //    return start;
    //}

    public static Pair2 Modify(this Pair2 start, Action<JObject> modifyFunc)
    {
        start.Clean.ForEach(listItem => modifyFunc.Invoke(listItem));

        return start;
    }

    public static Pair2 Modify(this Pair2 start, params (string, Func<JToken> creation)[] tree)
    {
        start.Modify(listItem =>
            {
                JToken current = listItem;

                foreach (var child in tree)
                {
                    current = Add(current, child.Item1, child.Item2);
                }
            }
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

    //public static Pair2 ModifyRemove2(this Pair2 start, params string[] properties)
    //{
    //    foreach (var current in start.Clean)
    //    {
    //        foreach (var prop in properties)
    //        {
    //            Remove(current, prop);
    //        }
    //    }

    //    return start;
    //}

    public static Pair2 Assert(this Pair2 start, Action<JObject, AssertionResult> assertion, string? description = null)
    {
        try
        {
            start.OpenApi.ForEach(o => assertion?.Invoke(o, start.Results));
        }
        catch
        {
            start.Results.SetResult(false, description);
        }

        return start;
    }

    public static bool CheckIf(this Pair2 start, Func<JObject, bool> assertion, string? description = null)
    {
        try
        {
            return start.OpenApi.All(o => assertion?.Invoke(o) == true);
        }
        catch
        {
            return false;
        }
    }

    //public static Pair<List<TIn>> AssertList<TIn>(this Pair<List<TIn>> start, Action<TIn, AssertionResult> assertion)
    //{
    //    foreach (var item in start.OpenApi)
    //    {
    //        assertion?.Invoke(item, start.Results);

    //        //if (!result.Success)
    //        //{
    //        //    Approvals.AssertEquals(
    //        //        start.CleanBase.ToString(Formatting.Indented),
    //        //        start.OpenApiBase.ToString(Formatting.Indented)
    //        //    );
    //        //}
    //    }
    //    return start;
    //}
}
