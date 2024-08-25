using static System.Runtime.InteropServices.JavaScript.JSType;

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

public interface IObjectFunctionValidationTestDescription 
{
    Type ApiType { get; }
    string ObjectMap { get; }
    string Function { get; }
    object[] ParamInputs { get; }
    int ParamIndex { get; }
    string Error { get; }
}

public class ObjectFunctionValidationTestDescription<TApi>(
    string objMap,
    string function,
    int paramIndex,
    object[] paramInputs,
    string error
    ) : IObjectFunctionValidationTestDescription
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