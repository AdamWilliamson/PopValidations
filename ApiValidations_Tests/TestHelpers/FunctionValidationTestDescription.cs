namespace ApiValidations_Tests.TestHelpers;

public class FunctionValidationTestDescription<TApi>(
    string function,
    int paramIndex,
    object[] paramInputs,
    string error
    )
{
    public Type ApiType => typeof(TApi);
    public string Function => function;
    public object[] ParamInputs => paramInputs;
    public int ParamIndex => paramIndex;
    public string Error => error;

    public static implicit operator object[](FunctionValidationTestDescription<TApi> d) =>
        new object[] { d };
}


public class ObjectFunctionValidationTestDescription<TApi>(
    string objMap,
    string function,
    int paramIndex,
    object[] paramInputs,
    string error
    )
{
    public Type ApiType => typeof(TApi);
    public string ObjectMap => objMap;
    public string Function => function;
    public object[] ParamInputs => paramInputs;
    public int ParamIndex => paramIndex;
    public string Error => error;

    public static implicit operator object[](ObjectFunctionValidationTestDescription<TApi> d) =>
        new object[] { d };
}