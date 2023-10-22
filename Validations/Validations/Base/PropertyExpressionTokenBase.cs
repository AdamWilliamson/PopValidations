using System;
using System.Linq.Expressions;

namespace PopValidations.FieldDescriptors.Base;

public interface IPropertyExpressionToken<TInput, TOutput>
{
    string Name { get; }
    TOutput? Execute(TInput value);
    string CombineWithParentProperty(string parentProperty);
}

public abstract class PropertyExpressionTokenBase<TInput, TOutput> : IPropertyExpressionToken<TInput, TOutput>
{
    public abstract string Name { get; }
    protected abstract Expression<Func<TInput, TOutput?>> Expression { get; }

    public virtual TOutput? Execute(TInput value) { return Expression.Compile().Invoke(value); }

    public virtual string CombineWithParentProperty(string parentProperty)
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            return parentProperty;
        }

        return parentProperty + "." + Name;
    }
}
