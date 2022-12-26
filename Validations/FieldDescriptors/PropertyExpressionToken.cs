using System;
using System.Linq.Expressions;
using PopValidations.FieldDescriptors.Base;
using PopValidations.Helpers;

namespace PopValidations.FieldDescriptors;

public class PropertyExpressionToken<TInput, TOutput> : PropertyExpressionTokenBase<TInput, TOutput>
{
    public override Expression<Func<TInput, TOutput>> Expression { get; }
    public override string Name { get; }

    public PropertyExpressionToken(
        Expression<Func<TInput, TOutput>> expression)
    {
        Expression = expression;
        Name = ExpressionUtilities.GetPropertyPath(expression);
    }

    public PropertyExpressionToken(
        Expression<Func<TInput, TOutput>> expression,
        string name)
    {
        Expression = expression;
        Name = name;
    }
}
