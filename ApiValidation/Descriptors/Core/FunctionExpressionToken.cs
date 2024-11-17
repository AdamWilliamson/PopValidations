using ApiValidations.Helpers;
using PopValidations.FieldDescriptors.Base;
using System.Data;
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

    string? GetParamTypeNameFor(int? index);
    string? GetReturnTypeName();
}

public class FunctionExpressionToken<TValidationType> : IFunctionExpressionToken
{
    public string Name { get; }
    public Type? ReturnType { get; }
    public List<ParamDetailsDTO>? Params { get; }

    public string? GetParamTypeNameFor(int? index)
    {
        if (index == null) return null;

        var type = Params?.Single(x => x.Index == index).ParamType;
        if (type is not null)
        {
            return GenericNameHelper.GetNameWithoutGenericArity(type);
        }
        return null;
    }

    public string? GetReturnTypeName()
    {
        return GenericNameHelper.GetNameWithoutGenericArity(ReturnType ?? typeof(void));
    }

    public FunctionExpressionToken(string? name, Type? returnType, List<ParamDetailsDTO>? paramList)
    {
        Name = name + $"({string.Join(',', paramList?.Select(x => GenericNameHelper.GetNameWithoutGenericArity(x.ParamType)) ?? [])})->{GenericNameHelper.GetNameWithoutGenericArity(returnType)}";
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
