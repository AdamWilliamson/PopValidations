namespace ApiValidations.Descriptors.Core;

public interface IParamDescriptor_Strategy<TValidationType, TParamType>
{
    int? ParamIndex { get; }
    string PropertyName { get; }
    string AddTo(string existing);
    IParamDescriptor_Strategy<TValidationType, TParamType> Clone();
    object? GetValue(object? value);
    //void SetParamDetails(string name, int index, IFunctionExpressionToken function);
    void Solidify();
}
