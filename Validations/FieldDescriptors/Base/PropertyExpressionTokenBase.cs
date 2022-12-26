using System;
using System.Linq.Expressions;

namespace PopValidations.FieldDescriptors.Base;

public interface IPropertyExpressionToken
{
    string Name { get; }
}

public abstract class PropertyExpressionTokenBase<TInput, TOutput> : IPropertyExpressionToken
{
    public abstract string Name { get; }
    public abstract Expression<Func<TInput, TOutput>> Expression { get; }

    public virtual string CombineWithParentProperty(string parentProperty)
    {
        return parentProperty + "." + Name;
    }
}
