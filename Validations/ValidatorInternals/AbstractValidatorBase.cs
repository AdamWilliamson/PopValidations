using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using PopValidations.Execution.Stores;
using PopValidations.FieldDescriptors;
using PopValidations.FieldDescriptors.Base;
using PopValidations.Scopes;
using PopValidations.Scopes.Whens;
using PopValidations.Validations.Base;

namespace PopValidations.ValidatorInternals;

public abstract class AbstractValidatorBase<TValidationType>
    : IParentScope
{
    public IParentScope? Parent { get; }
    public abstract string Name { get; }
    public Guid Id { get; } = Guid.NewGuid();

    protected AbstractValidatorBase(IParentScope? parent, ValidationConstructionStore store)
    {
        Parent = parent;
        Store = store;
    }

    protected IFieldDescriptor<TValidationType, IEnumerable<TFieldType>> DescribeEnumerable<TFieldType>(
        Expression<Func<TValidationType, IEnumerable<TFieldType>>> expr
    )
    {
        var fieldDescriptor = new FieldDescriptor<TValidationType, IEnumerable<TFieldType>>(
            new PropertyExpressionToken<TValidationType, IEnumerable<TFieldType>>(expr),
            Store
        );
        return fieldDescriptor;
    }

    public IFieldDescriptor<TValidationType, TFieldType> Describe<TFieldType>(Expression<Func<TValidationType, TFieldType>> expr)
    {
        var fieldDescriptor = new FieldDescriptor<TValidationType, TFieldType>(
            new PropertyExpressionToken<TValidationType, TFieldType>(expr),
            Store
        );
        return fieldDescriptor;
    }

    public void When(
        string whenDescription,
        Func<TValidationType, Task<bool>> ifTrue,
        Action rules)
    {
        var context = new WhenStringValidator<TValidationType>(
            Store,
            whenDescription,
            ifTrue,
            rules);
        Store.AddItem(null, context);
    }

    public void ScopeWhen<TPassThrough>(
        string whenDescription,
        Func<TValidationType, Task<bool>> ifTrue,
        Func<TValidationType, Task<TPassThrough>> scoped,
        Action<Validations.Base.ScopedData<TValidationType, TPassThrough>> rules)
    {
        var context = new WhenScopedResultValidator<TValidationType, TPassThrough>(
            Store,
            whenDescription,
            ifTrue,
            scoped,
            rules);
        Store.AddItem(null, context);
    }

    public void ScopeWhen<TPassThrough>(
        string whenDescription,
        Func<TValidationType, Task<TPassThrough>> scoped,
        Func<TValidationType, TPassThrough, Task<bool>> ifTrue,
        Action<ScopedData<TValidationType, TPassThrough>> rules)
    {
        var context = new WhenScopeToValidator<TValidationType, TPassThrough>(
            Store,
            whenDescription,
            scoped,
            ifTrue,
            rules);
        Store.AddItem(null, context);
    }

    public ValidationConstructionStore Store { get; }
}