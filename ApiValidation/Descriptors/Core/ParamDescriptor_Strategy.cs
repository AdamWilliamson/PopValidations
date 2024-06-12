namespace ApiValidations.Descriptors.Core;

public class ParamDescriptor_Strategy<TParamType, TValidationType>
    : IParamDescriptor_Strategy<TValidationType, TParamType>
{
    public IParamToken<TParamType> ParamToken { get; protected set; }
    protected object? RetrievedValue = null;
    protected bool ValueHasBeenRetrieved = false;

    public ParamDescriptor_Strategy(
        IParamToken<TParamType> paramToken)
    {
        ParamToken = paramToken;
    }

    public void SetParamDetails(string name, int index, IFunctionExpressionToken function)
    {
        ParamToken.SetParamDetails(name, index, function);
    }

    public IParamDescriptor_Strategy<TValidationType, TParamType> Clone()
    {
        return new ParamDescriptor_Strategy<TParamType, TValidationType>(ParamToken)
        {
            ValueHasBeenRetrieved = ValueHasBeenRetrieved,
            RetrievedValue = RetrievedValue,
        };
    }

    public int? ParamIndex => ParamToken.Index;
    public string PropertyName => (ParamToken.FunctionToken?.Name ?? string.Empty) + $"::({ParamToken.Name},{ParamIndex},{ParamToken.ParamType.Name})";

    public virtual string AddTo(string existing)
    {
        return ParamToken.FunctionToken?.CombineWithParentProperty(existing) + $"::({ParamToken.Name},{ParamIndex},{ParamToken.ParamType.Name})";
    }

    public virtual object? GetValue(object? value)
    {
        return null;
    }
}