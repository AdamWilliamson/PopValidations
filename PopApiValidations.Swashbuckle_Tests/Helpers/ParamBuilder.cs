using Newtonsoft.Json.Linq;
using PopApiValidations.Swashbuckle;
using System.Linq;

namespace PopApiValidations.Swashbuckle_Tests.Helpers;

public enum ParamType
{
    Auto,
    FromBody,
    FromQuery,
    FromUrl
}

public class ParamBuilder<TParamType>
{
    private readonly PopApiOpenApiConfig config;
    private readonly string url;
    private readonly string type;
    OpenApiNavigator openApiNavigator;
    private readonly string[] objHeirarcy;
    readonly Type[] BasicDataTypes = [typeof(int), typeof(double), typeof(string), typeof(bool), typeof(DateTime), typeof(DateTimeOffset)];
    bool isArray = false;
    public ParamType ParamType { get; }

    public ParamBuilder(
        ParamType paramType,
        PopApiOpenApiConfig config,
        OpenApiNavigator openApiNavigator,
        string[] objHeirarcy,
        string url,
        string type)
    {
        ParamType = paramType;
        this.config = config;
        this.url = url;
        this.type = type;
        this.openApiNavigator = openApiNavigator;
        this.objHeirarcy = objHeirarcy;
        isArray = objHeirarcy?.LastOrDefault()?.EndsWith("[n]") ?? false;
    }

    public Pair2 AsNav(string url, string type) { return openApiNavigator.GetPath2(url, type);  }

    public bool HasChildren()
    {
        return
            openApiNavigator.CheckIfRequestBody(url, type, objHeirarcy) && objHeirarcy?.Any() == true
            || openApiNavigator.CheckIfRequestBody(url, type, objHeirarcy) && objHeirarcy!.Length > 1;
    }

    public bool IsTargettingRequestBody()
    {
        return
            openApiNavigator.CheckIfRequestBody(url, type, objHeirarcy) && objHeirarcy?.Any() == false
            //|| openApiNavigator.CheckIfRequestBody(url, type, objHeirarcy) && objHeirarcy!.Length == 1;
            ;
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
        string[] childHeirarchy = objHeirarcy;
        Pair2? ValidationAttributeContainingParent;
        Pair2? Actual;
        string FocusName;

        if (openApiNavigator.CheckIfRequestBody(url, type, objHeirarcy))
        {
            ValidationAttributeContainingParent = openApiNavigator.GetRequestBodyParentPair2(url, type);
            Actual = openApiNavigator.GetRequestBodyChildrenPair2(url, type);
            if (HasChildren())
                FocusName = ToLowerFirstChar(childHeirarchy[^1]);
            else
                FocusName = "requestBody";
        }
        else
        {
            ValidationAttributeContainingParent = openApiNavigator.GetPath2(url, type);
            Actual = openApiNavigator.NavToParameterProperty2(url, type, objHeirarcy);
            FocusName = childHeirarchy[^1];
            childHeirarchy = objHeirarcy[1..];
        }

        Actual.Results.SetFocusName(FocusName);

        return (ValidationAttributeContainingParent, Actual, FocusName, childHeirarchy);
    }

    public ParamBuilder<TParamType> IsNotNull()
    {
        return Validate(
           attributeMessages: [() => "Must not be null."],
           //attributeAssertion: (Message: "Must be empty.", Error: "Complex {0} ValidationAttribute Does not contain value."),
           attributeAssertion: (Message: "Must not be null.", Error: "nullable set."),
           //arrayPropertyAssertions: [
           //    ([("maxLength", () => 0)], (schema, result) => result.SetResult(schema["maxLength"]?.Value<int>() == 0, "maxLength is not 0")),
           //    ([("maxItems", () => 0)], (schema, result) => result.SetResult(schema["maxItems"]?.Value<int>() == 0, "maxItems is not 0"))
           //],
           //allTypePropertyAssertions: [
           //    ([("required", () => true)], (schema, result) => result.SetResult(schema["required"]?.Value<bool>() == true, "required is not true"))
           // ],
           requestBodyPropertyAssertions: [
               ([("required", () => true)], (schema, result) => result.SetResult(schema["required"]?.Value<bool>() == false, "required is false"))
           ],
            parentPropertyAssertions: [
                (
                    [
                        ("required", () => new JArray()),
                        (null, () => objHeirarcy[^1])
                    ],
                    (schema, result) => result.SetResult(schema["required"]?.Values().Contains(result.FocusName) == true, "field is missing in parent required list"))
            ]
        );


        //var (ValidationAttributeContainingParent, Actual, FocusName, childHeirarchy) = GetBaseSetup();

        //(string, Func<JToken> creation)[] attributeMessage = {
            
        //};

        //Action<JObject, AssertionResult> assertion = (schema, result) =>
        //    result.SetResult(
        //        schema[config.CustomValidationAttribute]?[FocusName]?.Values().Contains("Must not be null.") == true,
        //        $"Complex {FocusName} ValidationAttribute Does not contain value."
        //    );

        //if (openApiNavigator.CheckIfRequestBody(url, type, objHeirarcy))
        //{
        //    ValidationAttributeContainingParent
        //        .Modify(("required", () => true))
        //        .Assert((schema, result) => result.SetResult(schema["required"]?.Value<bool>() == true, "required is not true"));
        //}
        //else
        //{
        //    ValidationAttributeContainingParent
        //        .Modify(
        //            (config.CustomValidationAttribute, () => new JObject()),
        //            (FocusName, () => new JArray()),
        //            (null, () => "Must not be null.")
        //        )
        //        .Assert(assertion);
        //}

        //if (HasChildren())
        //{
        //    Actual
        //        .Nav(objHeirarcy.Take(Math.Max(objHeirarcy.Length - 1, 0)).ToArray())
        //        //.Modify(attributeMessage)
        //        //.Assert(assertion)
        //        .ModifyRemove2("nullable")
        //        .Assert((schema, result) => result.SetResult(schema["nullable"] == null, "nullable exists."));
        //}
        //else
        //{
        //    ValidationAttributeContainingParent
        //        .Modify(attributeMessage)
        //        .Assert(assertion);

        //    // RequestBody, has a special required property.
        //    if (openApiNavigator.CheckIfRequestBody(url, type, objHeirarcy))
        //    {
        //        ValidationAttributeContainingParent
        //            .Modify(("required", () => true))
        //            .Assert((schema, result) => result.SetResult(schema["required"]?.Value<bool>() == true, "required is not true"));
        //    }
        //}

        //return this;
    }

    public ParamBuilder<TParamType> IsNull()
    {
        return Validate(
           attributeMessages: [() => "Must be null."],
           //attributeAssertion: (Message: "Must be empty.", Error: "Complex {0} ValidationAttribute Does not contain value."),
           attributeAssertion: (Message: "Must be null.", Error: "nullable not set."),
           //arrayPropertyAssertions: [
           //    ([("maxLength", () => 0)], (schema, result) => result.SetResult(schema["maxLength"]?.Value<int>() == 0, "maxLength is not 0")),
           //    ([("maxItems", () => 0)], (schema, result) => result.SetResult(schema["maxItems"]?.Value<int>() == 0, "maxItems is not 0"))
           //],
           allTypePropertyAssertions: [
               ([("nullable", () => true)], (schema, result) => result.SetResult(schema["nullable"]?.Value<bool>() == true, "nullable is not true")),
               ([("enum", () => new JArray()), (null, () => "null")], (schema, result) => result.SetResult(schema["enum"]?.Values().Contains("null") == true, "enum doesnt contain null"))
            ],
           requestBodyPropertyAssertions: [
               ([("required", () => false)], (schema, result) => result.SetResult(schema["required"]?.Value<bool>() == false, "required is not false"))
           ]
       );




        //var (ValidationAttributeContainingParent, Actual, FocusName, childHeirarchy) = GetBaseSetup();

        //(string, Func<JToken> creation)[] attributeMessage = {
        //    (config.CustomValidationAttribute, () => new JObject()),
        //    (FocusName, () => new JArray()),
        //    (null, () => "Must be null.")
        //};

        //(string, Func<JToken> creation)[] nullableEnumerable = {
        //    ("enum", () => new JArray()),
        //    (null, () => "null")
        //};

        //(string, Func<JToken> creation)[] nullable = {
        //    ("nullable", () => true)
        //};

        //Action<JObject, AssertionResult> assertion = (schema, result) =>
        //    result.SetResult(
        //        schema[config.CustomValidationAttribute]?[FocusName]?.Values().Contains("Must be null.") == true,
        //        $"Complex {FocusName} ValidationAttribute Does not contain value."
        //    );

        //if (HasChildren())
        //{
        //    Actual
        //        .Nav(objHeirarcy.Take(Math.Max(objHeirarcy.Length - 1, 0)).ToArray())
        //        .Modify(attributeMessage)
        //        .Assert(assertion)
        //        .Nav(objHeirarcy[^1..])
        //        .Modify(nullableEnumerable)
        //        .Assert((schema, result) => result.SetResult(schema["enum"]?.Values().Contains("null") == true, "enum doesnt contain null"))
        //        .Modify(nullable)
        //        .Assert((schema, result) => result.SetResult(schema["nullable"]?.Value<bool>() == true, "nullable is not true"));
        //}
        //else
        //{
        //    ValidationAttributeContainingParent
        //        .Modify(attributeMessage)
        //        .Assert(assertion)
        //        //.Modify(nullableEnumerable)
        //        //.Assert((schema, result) => result.SetResult(schema["enum"]?.Value<JArray>().Contains("null") == true))
        //        //.Modify(nullable)
        //        //.Assert((schema, result) => result.SetResult(schema["nullable"]?.Value<bool>() == true))
        //        ;

        //    // RequestBody, has a special required property.
        //    if (openApiNavigator.CheckIfRequestBody(url, type, objHeirarcy))
        //    {
        //        ValidationAttributeContainingParent
        //            .Modify(("required", () => false))
        //            .Assert((schema, result) => result.SetResult(schema["required"]?.Value<bool>() == false, "required is not false"));
        //    }
        //}

        //return this;
    }

    private ParamBuilder<TParamType> Validate(
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
            if (IsTargettingRequestBody())
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

    public ParamBuilder<TParamType> IsEmpty()
    {
        return Validate(
            attributeMessages: [() => "Must be empty."],
            attributeAssertion: (Message: "Must be empty.", Error: "Complex {0} ValidationAttribute Does not contain 'Must be empty.'."),
            arrayPropertyAssertions: [
                ([("maxLength", () => 0)], (schema, result) => result.SetResult(schema["maxLength"]?.Value<int>() == 0, "maxLength is not 0")),
                ([("maxItems", () => 0)], (schema, result) => result.SetResult(schema["maxItems"]?.Value<int>() == 0, "maxItems is not 0"))
            ]
        );

        //var (ValidationAttributeContainingParent, Actual, FocusName, childHeirarchy) = GetBaseSetup();

        //(string, Func<JToken> creation)[] attributeMessage = {
        //    (config.CustomValidationAttribute, () => new JObject()),
        //    (FocusName, () => new JArray()),
        //    (null, () => "Must be empty.")
        //};

        //(string, Func<JToken> creation)[] maxLength = {
        //    ("maxLength", () => 0)
        //};

        //(string, Func<JToken> creation)[] maxItems = {
        //    ("maxItems", () => 0)
        //};

        //Action<JObject, AssertionResult> messageAssertion = (schema, result) =>
        //    result.SetResult(
        //        schema[config.CustomValidationAttribute]?[FocusName]?.Values().Contains("Must be empty.") == true,
        //        $"Complex {FocusName} ValidationAttribute Does not contain value."
        //    );

        //if (HasChildren())
        //{
        //    var temp = Actual
        //        .Nav(objHeirarcy.Take(Math.Max(objHeirarcy.Length - 1, 0)).ToArray())
        //        .Modify(attributeMessage)
        //        .Assert(messageAssertion);

        //    if (temp.Nav(objHeirarcy[^1..]).CheckIf(x => x["type"].Value<string>() != "object")){
        //        temp.Nav(objHeirarcy[^1..])
        //            .Modify(maxLength)
        //            .Assert((schema, result) => result.SetResult(schema["maxLength"]?.Value<int>() == 0, "maxLength is not 0"))
        //            .Modify(maxItems)
        //            .Assert((schema, result) => result.SetResult(schema["maxItems"]?.Value<int>() == 0, "maxItems is not 0"));
        //    }
        //}
        //else
        //{
        //    ValidationAttributeContainingParent
        //        .Modify(attributeMessage)
        //        .Assert(messageAssertion);
        //}

        //return this;
    }

    public ParamBuilder<TParamType> IsNotEmpty()
    {
        return Validate(
            attributeMessages: [() => "Must not be empty."],
            attributeAssertion: (Message: "Must not be empty.", Error: "Complex {0} ValidationAttribute Does not contain 'Must not be empty.'."),
            arrayPropertyAssertions: [
                ([("minLength", () => 1)], (schema, result) => result.SetResult(schema["minLength"]?.Value<int>() == 1, "minLength is not 1")),
                ([("minItems", () => 1)], (schema, result) => result.SetResult(schema["minItems"]?.Value<int>() == 1, "minItems is not 1"))
            ],
            requestBodyPropertyAssertions: [
                (
                    [
                        ("required", () => new JArray()),
                        (null, () => objHeirarcy[^1])
                    ],
                    (schema, result) =>
                        result.SetResult(schema["required"]?.Values().Contains(result.FocusName) == true, "required is missing " + "")
                )
            ],
            parentPropertyAssertions: [
                (
                    [
                        ("required", () => new JArray()),
                        (null, () => objHeirarcy[^1])
                    ], 
                    (schema, result) => result.SetResult(schema["required"]?.Values().Contains(result.FocusName) == true, "field is missing in parent required list"))
            ]
        );



        //var (ValidationAttributeContainingParent, Actual, FocusName, childHeirarchy) = GetBaseSetup();

        //(string, Func<JToken> creation)[] attributeMessage = {
        //    (config.CustomValidationAttribute, () => new JObject()),
        //    (FocusName, () => new JArray()),
        //    (null, () => "Must not be empty.")
        //};

        //(string, Func<JToken> creation)[] minLength = {
        //    ("minLength", () => 1),
        //};
        //(string, Func<JToken> creation)[] requiredSelf = {
        //    ("required", () => true),
        //};
        //(string, Func<JToken> creation)[] requiredProperty = {
        //    ("required", () => new JArray()),
        //    (null, () => objHeirarcy[^1])
        //};
        //(string, Func<JToken> creation)[] minItems = {
        //    ("minItems", () => 1)
        //};

        //Action<JObject, AssertionResult> messageAssertion = (schema, result) =>
        //    result.SetResult(
        //        schema[config.CustomValidationAttribute]?[FocusName]?.Values().Contains("Must not be empty.") == true,
        //        $"Complex {FocusName} ValidationAttribute Does not contain value."
        //    );

        //if (HasChildren())
        //{
        //    Actual = Actual
        //        .Nav(objHeirarcy.Take(Math.Max(objHeirarcy.Length - 1, 0)).ToArray());

        //    if (IsTargettingRequestBody())
        //    {
        //        Actual 
        //            .Modify(requiredProperty)
        //            .Assert((schema, result) =>
        //                result.SetResult(schema["required"]?.Values().Contains(FocusName) == true, "required is missing " + FocusName)
        //            );
        //    }

        //    Actual = Actual
        //        .Modify(attributeMessage)
        //        .Assert(messageAssertion)
        //        .Nav(objHeirarcy[^1..])
        //        .Modify(minLength)
        //        .Assert((schema, result) => result.SetResult(schema["minLength"]?.Value<int>() == 1, "minLength is not 1"))
        //        .Modify(minItems)
        //        .Assert((schema, result) => result.SetResult(schema["minItems"]?.Value<int>() == 1, "minItems is not 1"));

        //    if (!IsTargettingRequestBody()) 
        //    { 
        //        Actual
        //            .Modify(requiredSelf)
        //            .Assert((schema, result) =>
        //                result.SetResult(schema["required"]?.Value<bool>() == true, "required is not true")
        //            );
        //    }
        //}
        //else
        //{
        //    ValidationAttributeContainingParent
        //        .Modify(attributeMessage)
        //        .Assert(messageAssertion);
        //}

        //return this;
    }
}
