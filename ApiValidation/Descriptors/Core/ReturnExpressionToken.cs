using PopValidations.FieldDescriptors.Base;

namespace ApiValidations.Descriptors.Core;

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
