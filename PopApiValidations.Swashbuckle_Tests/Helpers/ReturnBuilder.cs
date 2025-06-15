using Newtonsoft.Json.Linq;
using PopApiValidations.Swashbuckle;
using System.Linq;

namespace PopApiValidations.Swashbuckle_Tests.Helpers;

public class ReturnBuilder<TParamType>
{
    private readonly PopApiOpenApiConfig config;
    private readonly string url;
    private readonly string type;
    OpenApiNavigator openApiNavigator;
    private readonly string[] objHeirarcy;

    public ReturnBuilder(
        PopApiOpenApiConfig config,
        OpenApiNavigator openApiNavigator,
        string[] objHeirarcy,
        string url,
        string type)
    {
        this.config = config;
        this.url = url;
        this.type = type;
        this.openApiNavigator = openApiNavigator;
        this.objHeirarcy = objHeirarcy;
    }

    public Pair2 AsNav(string url, string type) { return openApiNavigator.GetPath2(url, type);  }

    public bool HasChildren()
    {
        return openApiNavigator.GetPath(url, type)
            .CanNav(schema => 
                (schema["responses"]?["200"]?["content"] as JObject)?.Properties().Any() == true
            );
    }

    public string ToLowerFirstChar(string input)
    {
        return input;
        //if (string.IsNullOrEmpty(input))
        //    return input;

        //return char.ToLower(input[0]) + input.Substring(1);
    }

    private (Pair2 Parent, Pair2 Actual, string FocusName, string[] childHeirarchy) GetBaseSetup()
    {
        string[] childHeirarchy = objHeirarcy ?? [];
        Pair2? ValidationAttributeContainingParent;
        Pair2? Actual;
        string FocusName;

        ValidationAttributeContainingParent = openApiNavigator.GetPath2(url, type);
        Actual = openApiNavigator.NavToResponses(url, type, objHeirarcy);
        FocusName = (childHeirarchy.Count() > 0)? childHeirarchy[^1] : string.Empty;
        childHeirarchy = (childHeirarchy.Count() > 0) ? objHeirarcy[1..] : [];

        Actual.Results.SetFocusName(FocusName);

        return (ValidationAttributeContainingParent, Actual, FocusName, childHeirarchy);
    }

    public ReturnBuilder<TParamType> IsNotNull()
    {
        return Validate(
            attributeMessages: [ () => "Must not be null." ],
            attributeAssertion: (Message: "Must not be null.", Error: "nullable set."),
            requestBodyPropertyAssertions: [
                ([("required", () => true)], (schema, result) => result.SetResult(schema["required"]?.Value<bool>() == false, "required is false"))
            ],
            parentPropertyAssertions: [
                (
                    [
                        ("required", () => new JArray()),
                        (null, () => objHeirarcy[^1])
                    ],
                    (schema, result) => result.SetResult(schema["required"]?.Values().Contains(result.FocusName) == true, "field is missing in parent required list")
                )
            ]
        );
    }

    //public ReturnBuilder<TParamType> IsNull()
    //{
    //    return Validate(
    //       attributeMessages: [() => "Must be null."],
    //       //attributeAssertion: (Message: "Must be empty.", Error: "Complex {0} ValidationAttribute Does not contain value."),
    //       attributeAssertion: (Message: "Must be null.", Error: "nullable not set."),
    //       //arrayPropertyAssertions: [
    //       //    ([("maxLength", () => 0)], (schema, result) => result.SetResult(schema["maxLength"]?.Value<int>() == 0, "maxLength is not 0")),
    //       //    ([("maxItems", () => 0)], (schema, result) => result.SetResult(schema["maxItems"]?.Value<int>() == 0, "maxItems is not 0"))
    //       //],
    //       allTypePropertyAssertions: [
    //           ([("nullable", () => true)], (schema, result) => result.SetResult(schema["nullable"]?.Value<bool>() == true, "nullable is not true")),
    //           ([("enum", () => new JArray()), (null, () => "null")], (schema, result) => result.SetResult(schema["enum"]?.Values().Contains("null") == true, "enum doesnt contain null"))
    //        ],
    //       requestBodyPropertyAssertions: [
    //           ([("required", () => false)], (schema, result) => result.SetResult(schema["required"]?.Value<bool>() == false, "required is not false"))
    //       ]
    //   );
    //}

    private ReturnBuilder<TParamType> Validate(
            List<Func<JToken>> attributeMessages,
            (string Message, string Error) attributeAssertion,
            List<((string? property, Func<JToken> setter)[] creator, Action<JToken, AssertionResult> tester)>? allTypePropertyAssertions = null,
            List<((string? property, Func<JToken> setter)[] creator, Action<JToken, AssertionResult> tester)>? arrayPropertyAssertions = null,
            List<((string? property, Func<JToken> setter)[] creator, Action<JToken, AssertionResult> tester)>? objectPropertyAssertions = null,
            List<((string? property, Func<JToken> setter)[] creator, Action<JToken, AssertionResult> tester)>? requestBodyPropertyAssertions = null,
            List<((string? property, Func<JToken> setter)[] creator, Action<JToken, AssertionResult> tester)>? parentPropertyAssertions = null
        )
    {
        arrayPropertyAssertions ??= new();
        objectPropertyAssertions ??= new();
        allTypePropertyAssertions ??= new();
        requestBodyPropertyAssertions ??= new();
        parentPropertyAssertions ??= new();

        var (ValidationAttributeContainingParent, Actual, FocusName, childHeirarchy) = GetBaseSetup();

        List<(string?, Func<JToken> creation)> realAttributeMessage = [
            (config.CustomValidationAttribute, () => new JObject()),
            (FocusName, () => new JArray())
        ];
        realAttributeMessage.AddRange(attributeMessages.Select(x => ((string)null, x)));

        Action<JObject, AssertionResult> messageAssertion = (schema, result) =>
            result.SetResult(
                schema[config.CustomValidationAttribute]?[FocusName]?.Values().Contains(attributeAssertion.Message) == true,
                string.Format(attributeAssertion.Error, FocusName)
            );

        if (HasChildren())
        {
            if (objHeirarcy.Length == 0)
            {
                foreach (var assertion in requestBodyPropertyAssertions)
                {
                    Actual
                        .Modify(assertion.creator)
                        .Assert(assertion.tester);
                }
            }
            else
            {
                var temp = Actual
                    .Nav(objHeirarcy.Take(Math.Max(objHeirarcy.Length - 1, 0)).ToArray())
                    .Modify(realAttributeMessage.ToArray())
                    .Assert(messageAssertion);

                foreach (var assertion in parentPropertyAssertions)
                {
                    temp
                        .Modify(assertion.creator)
                        .Assert((schema, result) => assertion.tester?.Invoke(schema, result));
                }

                if (temp.Nav(objHeirarcy[^1..]).CheckIf(x => x["type"].Value<string>() != "object"))
                {
                    var properties = temp.Nav(objHeirarcy[^1..]);

                    if (
                        properties.CheckIf(x => x["type"].Value<string>() != "object")
                        && properties.CheckIf(x => x["type"].Value<string>() != "integer")
                    )
                    {
                        foreach (var assertion in arrayPropertyAssertions.Concat(allTypePropertyAssertions ?? []))
                        {
                            properties
                                .Modify(assertion.creator)
                                .Assert((schema, result) => assertion.tester?.Invoke(schema, result));
                        }
                    }
                    else
                    {
                        foreach (var assertion in objectPropertyAssertions.Concat(allTypePropertyAssertions ?? []))
                        {
                            properties
                                .Modify(assertion.creator)
                                .Assert((schema, result) => assertion.tester?.Invoke(schema, result));
                        }
                    }
                }
            }
        }
        else
        {
            ValidationAttributeContainingParent
                .Modify(realAttributeMessage.ToArray())
                .Assert(messageAssertion);
        }

        return this;
    }

    //public ReturnBuilder<TParamType> IsEmpty()
    //{
    //    return Validate(
    //        attributeMessages: [() => "Must be empty."],
    //        attributeAssertion: (Message: "Must be empty.", Error: "Complex {0} ValidationAttribute Does not contain 'Must be empty.'."),
    //        arrayPropertyAssertions: [
    //            ([("maxLength", () => 0)], (schema, result) => result.SetResult(schema["maxLength"]?.Value<int>() == 0, "maxLength is not 0")),
    //            ([("maxItems", () => 0)], (schema, result) => result.SetResult(schema["maxItems"]?.Value<int>() == 0, "maxItems is not 0"))
    //        ]
    //    );
    //}

    //public ReturnBuilder<TParamType> IsNotEmpty()
    //{
    //    return Validate(
    //        attributeMessages: [() => "Must not be empty."],
    //        attributeAssertion: (Message: "Must not be empty.", Error: "Complex {0} ValidationAttribute Does not contain 'Must not be empty.'."),
    //        arrayPropertyAssertions: [
    //            ([("minLength", () => 1)], (schema, result) => result.SetResult(schema["minLength"]?.Value<int>() == 1, "minLength is not 1")),
    //            ([("minItems", () => 1)], (schema, result) => result.SetResult(schema["minItems"]?.Value<int>() == 1, "minItems is not 1"))
    //        ],
    //        requestBodyPropertyAssertions: [
    //            (
    //                [
    //                    ("required", () => new JArray()),
    //                    (null, () => objHeirarcy[^1])
    //                ],
    //                (schema, result) =>
    //                    result.SetResult(schema["required"]?.Values().Contains(result.FocusName) == true, "required is missing " + "")
    //            )
    //        ],
    //        parentPropertyAssertions: [
    //            (
    //                [
    //                    ("required", () => new JArray()),
    //                    (null, () => objHeirarcy[^1])
    //                ], 
    //                (schema, result) => result.SetResult(schema["required"]?.Values().Contains(result.FocusName) == true, "field is missing in parent required list"))
    //        ]
    //    );
    //}
}
