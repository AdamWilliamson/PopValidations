using PopValidations.FieldDescriptors.Base;

namespace ApiValidations.Descriptors.Core;

public interface IParamIndexedToken<TValidationType, TInput, TOutput>
    : IPropertyExpressionToken<TOutput>,
        IParamToken<TOutput>
    where TInput : IEnumerable<TOutput>
{ }

public class ParamIndexedPropertyExpressionToken<TValidationType, TInput, TOutput>
    : IParamIndexedToken<TValidationType, TInput, TOutput>
    where TInput : IEnumerable<TOutput>
{
    private readonly IParamToken<TInput> paramToken;

    public string Name => paramToken.Name + $"[n]";
    public Type ParamType => typeof(TOutput);
    public IFunctionExpressionToken FunctionToken => paramToken.FunctionToken;
    public ParamIndexedPropertyExpressionToken(
        IParamToken<TInput> paramToken,
        int index)
    {
        this.paramToken = paramToken;
        Index = index;
    }

    public int Index { get; protected set; }

    public virtual string CombineWithParentProperty(string parentProperty)
    {
        if (string.IsNullOrEmpty(Name))
        {
            return paramToken?.CombineWithParentProperty(parentProperty) ?? "<Unknown>";
        }
        if (Index < 0) return paramToken?.CombineWithParentProperty(parentProperty) + "." + Name;
        return paramToken?.CombineWithParentProperty(parentProperty) + "." + Name;
    }

    public TOutput? Execute(object value)
    {
        throw new NotImplementedException();
    }

    public void SetParamDetails(string name, int index, IFunctionExpressionToken owningFunction)
    {
        paramToken.SetParamDetails(name, index, owningFunction);
    }
}