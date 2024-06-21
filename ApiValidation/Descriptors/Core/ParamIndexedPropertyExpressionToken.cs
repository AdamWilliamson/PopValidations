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
    //private readonly IParamToken<TInput> paramToken;
    public IParamVisitor Visitor { get; protected set; }

    private string? _name = null;
    public string Name => _name + $"[n]";
    public int Index { get; protected set; }
    public int EnumerableIndex { get; protected set; }
    public Type ParamType => typeof(TOutput);
    public IFunctionExpressionToken FunctionToken { get; protected set; }

    public ParamIndexedPropertyExpressionToken(
        IParamVisitor visitor,
        //IParamToken<TInput> paramToken,
        int enumerableIndex)
    {
        this.Visitor = visitor;
        //this.paramToken = paramToken;
        EnumerableIndex = enumerableIndex;
    }

    //public ParamIndexedPropertyExpressionToken(IParamVisitor visitor)
    //{
    //    this.visitor = visitor;
    //}

    public virtual string CombineWithParentProperty(string parentProperty)
    {
        if (string.IsNullOrEmpty(Name))
        {
            return FunctionToken?.CombineWithParentProperty(parentProperty) ?? "<Unknown>";
        }
        if (Index < 0) return FunctionToken?.CombineWithParentProperty(parentProperty) + "." + Name;
        return FunctionToken?.CombineWithParentProperty(parentProperty) + "." + Name;
    }

    public TOutput? Execute(object value)
    {
        throw new NotImplementedException();
    }

    public void SetParamDetails(string name, int index, IFunctionExpressionToken owningFunction)
    {
        _name = name;
        FunctionToken = owningFunction;
        Index = index;
    }

    public IParamToken<TOutput> Clone()
    {
        return new ParamIndexedPropertyExpressionToken<TValidationType, TInput, TOutput>(Visitor, this.Index);
    }

    public void Solidify()
    {
        //paramToken.Solidify();
        var param = Visitor.GetCurrentParamDescriptor();

        SetParamDetails(
            param.Name ?? "<unknown>",
            param.Index,
            Visitor.GetCurrentFunction() ?? throw new Exception("Instatiating Parameter details without Function.")
        );
    }
}