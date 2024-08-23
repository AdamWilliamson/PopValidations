using ApiValidations.Helpers;

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

    //public void SetParamDetails(string name, int index, IFunctionExpressionToken function)
    //{
    //    ParamToken.SetParamDetails(name, index, function);
    //}
    public void Solidify() { ParamToken.Solidify();  }

    public IParamDescriptor_Strategy<TValidationType, TParamType> Clone()
    {
        return new ParamDescriptor_Strategy<TParamType, TValidationType>(new ParamToken<TParamType, TValidationType>(ParamToken.Visitor))
        {
            ValueHasBeenRetrieved = ValueHasBeenRetrieved,
            RetrievedValue = RetrievedValue,
        };
    }

    public int? ParamIndex => ParamToken.Index;
    public string PropertyName => (ParamToken.FunctionToken?.Name ?? string.Empty) + $":Param({ParamIndex},{GenericNameHelper.GetNameWithoutGenericArity(ParamToken.ParamType)},{ParamToken.Name})";

    public virtual string AddTo(string existing)
    {
        return ParamToken.FunctionToken?.CombineWithParentProperty(existing) + $":Param({ParamIndex},{GenericNameHelper.GetNameWithoutGenericArity(ParamToken.ParamType)},{ParamToken.Name})";
    }

    public virtual object? GetValue(object? value)
    {
        if (!ParamIndex.HasValue) return null;

        return ParamToken.Visitor.GetParamValue(ParamIndex.Value);
    }
}