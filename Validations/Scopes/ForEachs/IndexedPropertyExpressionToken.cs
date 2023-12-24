using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using PopValidations.FieldDescriptors.Base;

namespace PopValidations.Scopes.ForEachs;

public class IndexedPropertyExpressionToken<TValidationType,TInput, TOutput>
    : PropertyExpressionTokenBase<TInput, TOutput>
    where TInput : IEnumerable<TOutput>
{
    protected override Expression<Func<TInput, TOutput>> Expression { get; }
    //public IFieldDescriptor<TValidationType, TInput> ParentDescriptor { get; }
    public override string Name { get; }

    public IndexedPropertyExpressionToken(
        string name, 
        int index)
    {
        Expression = (input) => input.ElementAt(index);

        Name = name ?? string.Empty;
        Index = index;
    }

    public int Index { get; }

    public override string CombineWithParentProperty(string parentProperty)
    {
        if (string.IsNullOrEmpty(Name))
        {
            return parentProperty;
        }
        if (Index < 0) return parentProperty + "." + Name;
        return parentProperty +"."+ Name;
    }
}
