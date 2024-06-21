using PopValidations.FieldDescriptors.Base;

namespace ApiValidations.Descriptors.Core;

public interface IParamToken<TParamType> : IPropertyExpressionToken<TParamType>
{
    IParamVisitor Visitor { get; }
    IFunctionExpressionToken? FunctionToken { get; }
    Type ParamType { get; }
    int Index { get; }
    void Solidify();
}
