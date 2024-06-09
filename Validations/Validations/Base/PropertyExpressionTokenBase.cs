using System;
using System.Linq.Expressions;

namespace PopValidations.FieldDescriptors.Base;

public interface IPropertyExpressionToken<TOutput>
{
    string Name { get; }
    TOutput? Execute(object value);
    string CombineWithParentProperty(string parentProperty);
}

public abstract class PropertyExpressionTokenBase<TInput, TOutput> : IPropertyExpressionToken<TOutput>
{
    public abstract string Name { get; }
    protected abstract Expression<Func<TInput, TOutput>> Expression { get; }

    public virtual TOutput? Execute(object value) { return Expression.Compile().Invoke((TInput)value); }

    public virtual string CombineWithParentProperty(string parentProperty)
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            return parentProperty;
        }

        return parentProperty + "." + Name;
    }
}
