namespace ApiValidations.Descriptors.Core;

public class ParamToken<TParamType, TValidationType> 
    : IParamToken<TParamType>
{
    public IParamVisitor Visitor { get; protected set; }

    public IFunctionExpressionToken? FunctionToken { get; protected set; }
    public string Name { get; protected set; } = "";
    public Type ParamType => typeof(TParamType);
    public int Index { get; protected set; }

    public ParamToken(IParamVisitor visitor)
    {
        this.Visitor = visitor;
    }

    protected void SetParamDetails(string name, int index, IFunctionExpressionToken owningFunction)
    {
        Name = name;
        FunctionToken = owningFunction;
        Index = index;
    }

    public TParamType? Execute(object value)
    {
        throw new NotImplementedException("Cannot be executed as a part of a Non-Api Validation");
    }

    public virtual string CombineWithParentProperty(string parentProperty)
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            return FunctionToken?.CombineWithParentProperty(parentProperty) ?? "";
        }

        return FunctionToken?.CombineWithParentProperty(parentProperty) + "." + Name;
    }

    //public IParamToken<TParamType> Clone()
    //{
    //    return new ParamToken<TParamType, TValidationType>(visitor)
    //    {
    //        Name = this.Name,
    //        Index = this.Index,
    //        FunctionToken = this.FunctionToken
    //    };
    //}

    public void Solidify()
    {
        var param = Visitor.GetCurrentParamDescriptor();

        SetParamDetails(
            param.Name ?? "<unknown>", 
            param.Index, 
            Visitor.GetCurrentFunction() ?? throw new Exception("Instatiating Parameter details without Function.")
        );
    }
}
