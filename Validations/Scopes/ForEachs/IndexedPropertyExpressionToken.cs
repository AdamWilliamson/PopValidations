using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using PopValidations.FieldDescriptors.Base;

namespace PopValidations.Scopes.ForEachs;

public class IndexedPropertyExpressionToken<TInput, TOutput>
    : PropertyExpressionTokenBase<TInput, TOutput>
    where TInput : IEnumerable<TOutput>
{
    public override Expression<Func<TInput, TOutput>> Expression { get; }
    public override string Name { get; }

    public IndexedPropertyExpressionToken(
        //Expression<Func<TInput, TOutput>> expression, 
        string name,
        int index)
    {
        Expression = (input) => input.ElementAt(index);
        Name = name;
        Index = index;
    }

    public int Index { get; }

    public override string CombineWithParentProperty(string parentProperty)
    {
        if (Index < 0) return parentProperty + $"[n]";
        return parentProperty + $"[{Index}]";
    }
}
