using PopValidations.FieldDescriptors.Base;
using System.Linq.Expressions;

namespace PopValidations_Functional_Testbed;

public interface IFunctionExpressionToken<TValidationType> : IPropertyExpressionToken<TValidationType, Expression>
{
    Type ReturnType { get; }
    List<ParamDetailsDTO> Params { get; }
}

public class FunctionExpressionToken<TValidationType> : IFunctionExpressionToken<TValidationType>
{
    public string Name { get; protected set; }
    public Type ReturnType { get; }
    public List<ParamDetailsDTO> Params { get; }

    public FunctionExpressionToken(string name, Type returnType, List<ParamDetailsDTO> paramList)
    {
        Name = name + $"({string.Join(',', paramList.Select(x => x.ParamType.Name))}):{returnType.Name}";
        ReturnType = returnType;
        Params = paramList;
    }

    public string CombineWithParentProperty(string parentProperty)
    {
        return parentProperty + '.' + Name;
    }

    public Expression? Execute(TValidationType value)
    {
        return null;
    }
}
