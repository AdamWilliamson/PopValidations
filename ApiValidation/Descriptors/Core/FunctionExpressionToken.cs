using ApiValidations.Helpers;
using PopValidations.FieldDescriptors.Base;
using System.Linq.Expressions;

namespace ApiValidations.Descriptors.Core;

public interface IFunctionToken
{
    string Name { get; }
    Type? ReturnType { get; }
    List<ParamDetailsDTO>? Params { get; }
}

public interface IFunctionExpressionToken 
    : IPropertyExpressionToken<Expression>,
        IFunctionToken
{
    new string Name { get; }

    bool Matches(string name, Type returnType, IEnumerable<ParamDetailsDTO> paramList);
}

public class FunctionExpressionToken<TValidationType> : IFunctionExpressionToken
{
    public string Name { get; }
    public Type? ReturnType { get; }
    public List<ParamDetailsDTO>? Params { get; }
    
    public FunctionExpressionToken(string? name, Type? returnType, List<ParamDetailsDTO>? paramList)
    {
        Name = name + $"({string.Join(',', paramList?.Select(x => x.ParamType.Name) ?? [])}):{GenericNameHelper.GetNameWithoutGenericArity(returnType)}";
        ReturnType = returnType;
        Params = paramList;
    }

    public string CombineWithParentProperty(string parentProperty)
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            return parentProperty;
        }

        return parentProperty + '.' + Name;
    }

    public Expression? Execute(object value)
    {
        return null;
    }

    public bool Matches(string name, Type returnType, IEnumerable<ParamDetailsDTO> paramList)
    {
        return name == Name
            && returnType == ReturnType
            && (Params?.Select(p => p.ParamType).AsEnumerable().SequenceEqual(paramList.Select(p => p.ParamType).AsEnumerable()) ?? false)
            && (Params?.All(x => x.MatchesValue(x.ParamType)) ?? false);
    }

}

public interface IParamToken<TParamType> : IPropertyExpressionToken<TParamType>
{
    IFunctionExpressionToken? FunctionToken { get; }
    Type ParamType { get; }
    int Index { get; }
    void SetParamDetails(string name, int index, IFunctionExpressionToken owningFunction);
}

public class ParamExpressionToken<TParamType, TValidationType> 
    : IParamToken<TParamType>
{
    public IFunctionExpressionToken? FunctionToken { get; protected set; }
    public string Name { get; protected set; } = "";
    public Type ParamType => typeof(TParamType);
    public int Index { get; protected set; }

    public void SetParamDetails(string name, int index, IFunctionExpressionToken owningFunction)
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
}

public class ReturnExpressionToken<TReturnType, TValidationType> : IPropertyExpressionToken<TReturnType>
{
    private readonly FunctionExpressionToken<TValidationType> owningFunction;

    public string Name { get; }

    public ReturnExpressionToken(string name, FunctionExpressionToken<TValidationType> owningFunction)
    {
        Name = name;
        this.owningFunction = owningFunction;
    }

    public TReturnType? Execute(object value)
    {
        throw new NotImplementedException("Cannot be executed as a part of a Non-Api Validation");
    }

    public virtual string CombineWithParentProperty(string parentProperty)
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            return owningFunction.CombineWithParentProperty(parentProperty);
        }

        return owningFunction.CombineWithParentProperty(parentProperty) + "." + Name;
    }
}
