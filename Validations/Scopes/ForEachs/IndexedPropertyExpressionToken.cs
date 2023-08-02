using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using PopValidations.FieldDescriptors.Base;

namespace PopValidations.Scopes.ForEachs;

public class IndexedPropertyExpressionToken<TValidationType,TInput, TOutput>
    : PropertyExpressionTokenBase<TInput, TOutput>
    where TInput : IEnumerable<TOutput?>
{
    //private readonly IPropertyExpressionToken<TInput, TInput> parentPropertyToken;

    protected override Expression<Func<TInput, TOutput?>> Expression { get; }
    public IFieldDescriptor<TValidationType, TInput> ParentDescriptor { get; }

    //protected Expression<Func<TInput?, TOutput?>> IndexExpression { get; }
    public override string Name { get; }

    //public string ExpressionAsString() { return "[n]"; }

    //public override TOutput? Execute(TInput value)
    //{
    //    var iEnumerable = parentPropertyToken.Execute(value);
    //    return IndexExpression.Compile().Invoke(iEnumerable);
    //}

    public IndexedPropertyExpressionToken(
        //IPropertyExpressionToken<TInput, TInput> parentPropertyToken,
        //IFieldDescriptor<TValidationType, TInput> parentDescriptor,
        string name, 
        int index)
    {
        Expression = (input) => input != null? input.ElementAt(index) : default(TOutput);
        //ParentDescriptor = parentDescriptor;
        //this.parentPropertyToken = parentPropertyToken;

        //Expression = (input) => IndexExpression.Compile().Invoke(parentPropertyToken.Execute(input));
        Name = name;
        Index = index;
    }

    public int Index { get; }

    public override string CombineWithParentProperty(string parentProperty)
    {
        if (Index < 0) return parentProperty + "." + Name;// + $"[n]";
        return parentProperty +"."+ Name;// parentProperty + $"[{Index}]";
    }
}
