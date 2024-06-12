namespace ApiValidations.Descriptors.Core;

public class ParamIndexedDescriptor_Strategy<TValidationType, TEnumeratedParamType, TParamType>
    : IParamDescriptor_Strategy<TValidationType, TParamType>
    where TEnumeratedParamType : IEnumerable<TParamType>
{
    private readonly IParamIndexedToken<TValidationType, TEnumeratedParamType, TParamType> paramToken;

    public ParamIndexedDescriptor_Strategy(
        IParamIndexedToken<TValidationType, TEnumeratedParamType, TParamType> paramToken,
        int? paramIndex
    )
    {

        this.paramToken = paramToken ?? throw new ArgumentException(nameof(paramToken));
        ParamIndex = paramIndex;
    }

    public int? ParamIndex { get; }
    public string PropertyName => (ParamToken?.FunctionToken?.Name ?? string.Empty) + $"::({ParamToken!.Name},{ParamIndex},{ParamToken!.ParamType.Name})";
    public IParamToken<TParamType> ParamToken => paramToken;

    public string AddTo(string existing)
    {
        return ParamToken?.FunctionToken?.CombineWithParentProperty(existing) + $"::({ParamToken!.Name},{ParamIndex},{ParamToken!.ParamType.Name})";
    }

    public IParamDescriptor_Strategy<TValidationType, TParamType> Clone()
    {
        return new ParamIndexedDescriptor_Strategy<TValidationType, TEnumeratedParamType, TParamType>(
            paramToken,
            ParamIndex
        );
    }

    public virtual object? GetValue(object? value)
    {
        throw new NotImplementedException("Shouldn't get Param value through Non-Api Validation.");
    }

    public void SetParamDetails(string name, int index, IFunctionExpressionToken function)
    {
        paramToken.SetParamDetails(name, index, function);
    }
}