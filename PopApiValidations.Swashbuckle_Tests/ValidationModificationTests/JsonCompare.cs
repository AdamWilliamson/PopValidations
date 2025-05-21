using Newtonsoft.Json.Linq;

namespace PopApiValidations.Swashbuckle_Tests.ValidationModificationTests;

public static class JsonCompare
{
    public static string FindDiffString(JToken leftJson, JToken rightJson)
    {
        return FindDiff(leftJson, rightJson).ToString(Newtonsoft.Json.Formatting.None);
    }

    //https://stackoverflow.com/questions/24876082/find-and-return-json-differences-using-newtonsoft-in-c
    public static JObject FindDiff(JToken leftJson, JToken rightJson)
    {
        var difference = new JObject();
        if (JToken.DeepEquals(leftJson, rightJson)) return difference;

        switch (leftJson.Type)
        {
            case JTokenType.Object:
                {
                    var LeftJSON = leftJson as JObject;
                    var RightJSON = rightJson as JObject;
                    var RemovedTags = LeftJSON?.Properties()?.Select(c => c.Name.ToLower()).Except(RightJSON?.Properties()?.Select(c => c.Name.ToLower()) ?? []) ?? [];
                    var AddedTags = RightJSON?.Properties()?.Select(c => c.Name.ToLower()).Except(LeftJSON?.Properties()?.Select(c => c.Name.ToLower()) ?? []) ?? [];
                    var UnchangedTags = LeftJSON?.Properties().Where(c => DeepEqualsIgnoreCase(c.Value, RightJSON?[c.Name])).Select(c => c.Name.ToLower()).ToList() ?? [];

                    foreach (var tag in RemovedTags)
                    {
                        difference[tag] = new JObject
                        {
                            ["-"] = LeftJSON?[tag]
                        };
                    }

                    foreach (var tag in AddedTags)
                    {
                        difference[tag] = new JObject
                        {
                            ["-"] = RightJSON?[tag]
                        };
                    }

                    var ModifiedTags = LeftJSON?.Properties()?.Select(c => c.Name.ToLower()).Except(AddedTags).Except(UnchangedTags) ?? [];

                    foreach (var tag in ModifiedTags)
                    {
                        if (LeftJSON?[tag] is null)
                        {
                            continue;
                        }

                        if (RightJSON?[tag] is null)
                        {
                            difference[tag] = LeftJSON[tag];
                            continue;
                        }

                        var foundDifference = FindDiff(LeftJSON![tag]!, RightJSON![tag]!);
                        if (foundDifference.HasValues)
                        {
                            difference[tag] = foundDifference;
                        }
                    }
                }
                break;
            case JTokenType.Array:
                {
                    var LeftArray = leftJson as JArray;
                    var RightArray = rightJson as JArray;

                    if (LeftArray != null && RightArray != null)
                    {
                        if (LeftArray.Count() == RightArray.Count())
                        {
                            for (int index = 0; index < LeftArray.Count(); index++)
                            {
                                var foundDifference = FindDiff(LeftArray[index], RightArray[index]);
                                if (foundDifference.HasValues)
                                {
                                    difference[$"{index}"] = foundDifference;
                                }
                            }
                        }
                        else
                        {
                            var left = new JArray(LeftArray.Except(RightArray, new JTokenEqualityComparer()));
                            var right = new JArray(RightArray.Except(LeftArray, new JTokenEqualityComparer()));

                            if (left.HasValues)
                            {
                                difference["-"] = left;
                            }

                            if (right.HasValues)
                            {
                                difference["+"] = right;
                            }
                        }
                    }
                }
                break;
            default:
                difference["-"] = leftJson;
                difference["+"] = rightJson;
                break;
        }

        return difference;
    }

    public static bool DeepEqualsIgnoreCase(JToken token1, JToken token2)
    {
        if (token1 == null && token2 == null) return true;
        if (token1 == null || token2 == null) return false;
        if (token1.Type != token2.Type)
        {
            // Special case: allow comparing strings and raw JSON as string
            if ((token1.Type == JTokenType.String && token2.Type == JTokenType.String) ||
                (token1.Type == JTokenType.Property && token2.Type == JTokenType.Property))
            {
                return string.Equals(token1.ToString(), token2.ToString(), StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        switch (token1.Type)
        {
            case JTokenType.Object:
                var obj1 = (JObject)token1;
                var obj2 = (JObject)token2;
                var props1 = new Dictionary<string, JToken>(StringComparer.OrdinalIgnoreCase);
                var props2 = new Dictionary<string, JToken>(StringComparer.OrdinalIgnoreCase);

                foreach (var prop in obj1.Properties())
                    props1[prop.Name] = prop.Value;

                foreach (var prop in obj2.Properties())
                    props2[prop.Name] = prop.Value;

                if (props1.Count != props2.Count)
                    return false;

                foreach (var key in props1.Keys)
                {
                    if (!props2.TryGetValue(key, out var value2))
                        return false;

                    if (!DeepEqualsIgnoreCase(props1[key], value2))
                        return false;
                }
                return true;

            case JTokenType.Array:
                var arr1 = (JArray)token1;
                var arr2 = (JArray)token2;

                if (arr1.Count != arr2.Count)
                    return false;

                for (int i = 0; i < arr1.Count; i++)
                {
                    if (!DeepEqualsIgnoreCase(arr1[i], arr2[i]))
                        return false;
                }
                return true;

            case JTokenType.String:
                return string.Equals(token1.Value<string>(), token2.Value<string>(), StringComparison.OrdinalIgnoreCase);

            default:
                return JToken.DeepEquals(token1, token2);
        }
    }
}

//public static class JSonCompare
//{
//    ///https://stackoverflow.com/questions/24876082/find-and-return-json-differences-using-newtonsoft-in-c
//    /// <summary>
//    /// Deep compare two NewtonSoft JObjects. If they don't match, returns text diffs
//    /// </summary>
//    /// <param name="source">The expected results</param>
//    /// <param name="target">The actual results</param>
//    /// <returns>Text string</returns>

//    public static StringBuilder CompareObjects(JObject source, JObject target)
//    {
//        StringBuilder returnString = new StringBuilder();
//        foreach (KeyValuePair<string, JToken> sourcePair in source)
//        {
//            if (sourcePair.Value.Type == JTokenType.Object)
//            {
//                if (target.GetValue(sourcePair.Key) == null)
//                {
//                    returnString.Append("Key " + sourcePair.Key
//                                        + " not found" + Environment.NewLine);
//                }
//                else if (target.GetValue(sourcePair.Key).Type != JTokenType.Object)
//                {
//                    returnString.Append("Key " + sourcePair.Key
//                                        + " is not an object in target" + Environment.NewLine);
//                }
//                else
//                {
//                    returnString.Append(CompareObjects(sourcePair.Value.ToObject<JObject>(),
//                        target.GetValue(sourcePair.Key).ToObject<JObject>()));
//                }
//            }
//            else if (sourcePair.Value.Type == JTokenType.Array)
//            {
//                if (target.GetValue(sourcePair.Key) == null)
//                {
//                    returnString.Append("Key " + sourcePair.Key
//                                        + " not found" + Environment.NewLine);
//                }
//                else
//                {
//                    returnString.Append(CompareArrays(sourcePair.Value.ToObject<JArray>(),
//                        target.GetValue(sourcePair.Key).ToObject<JArray>(), sourcePair.Key));
//                }
//            }
//            else
//            {
//                JToken expected = sourcePair.Value;
//                var actual = target.SelectToken(sourcePair.Key);
//                if (actual == null)
//                {
//                    returnString.Append("Key " + sourcePair.Key
//                                        + " not found" + Environment.NewLine);
//                }
//                else
//                {
//                    if (!JToken.DeepEquals(expected, actual))
//                    {
//                        returnString.Append("Key " + sourcePair.Key + ": "
//                                            + sourcePair.Value + " !=  "
//                                            + target.Property(sourcePair.Key).Value
//                                            + Environment.NewLine);
//                    }
//                }
//            }
//        }
//        return returnString;
//    }

//    /// <summary>
//    /// Deep compare two NewtonSoft JArrays. If they don't match, returns text diffs
//    /// </summary>
//    /// <param name="source">The expected results</param>
//    /// <param name="target">The actual results</param>
//    /// <param name="arrayName">The name of the array to use in the text diff</param>
//    /// <returns>Text string</returns>

//    private static StringBuilder CompareArrays(JArray source, JArray target, string arrayName = "")
//    {
//        var returnString = new StringBuilder();
//        for (var index = 0; index < source.Count; index++)
//        {

//            var expected = source[index];
//            if (expected.Type == JTokenType.Object)
//            {
//                var actual = (index >= target.Count) ? new JObject() : target[index];
//                returnString.Append(CompareObjects(expected.ToObject<JObject>(),
//                    actual.ToObject<JObject>()));
//            }
//            else
//            {

//                var actual = (index >= target.Count) ? "" : target[index];
//                if (!JToken.DeepEquals(expected, actual))
//                {
//                    if (String.IsNullOrEmpty(arrayName))
//                    {
//                        returnString.Append("Index " + index + ": " + expected
//                                            + " != " + actual + Environment.NewLine);
//                    }
//                    else
//                    {
//                        returnString.Append("Key " + arrayName
//                                            + "[" + index + "]: " + expected
//                                            + " != " + actual + Environment.NewLine);
//                    }
//                }
//            }
//        }
//        return returnString;
//    }

//}