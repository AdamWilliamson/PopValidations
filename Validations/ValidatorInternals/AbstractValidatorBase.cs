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

public abstract class AbstractValidatorBase<TValidationType> : IParentScope
{
    public IParentScope? Parent { get; }
    public abstract string Name { get; }
    public Guid Id { get; } = Guid.NewGuid();

    protected AbstractValidatorBase(IParentScope? parent, IValidationStore store)
    {
        Parent = parent;
        Store = store;
    }

    protected IFieldDescriptor<
        TValidationType,
        IEnumerable<TFieldType?>?
    > DescribeEnumerable<TFieldType>(
        Expression<Func<TValidationType, IEnumerable<TFieldType?>?>> expr
    )
    {
        var fieldDescriptor = new FieldDescriptor<TValidationType, IEnumerable<TFieldType?>?>(
            new PropertyExpressionToken<TValidationType, IEnumerable<TFieldType?>?>(expr),
            Store
        );
        return fieldDescriptor;
    }

    public IFieldDescriptor<TValidationType, TFieldType?> Describe<TFieldType>(
        Expression<Func<TValidationType, TFieldType?>> expr
    )
    {
        var fieldDescriptor = new FieldDescriptor<TValidationType, TFieldType?>(
            new PropertyExpressionToken<TValidationType, TFieldType?>(expr),
            Store
        );
        return fieldDescriptor;
    }

    public void Scope<TData>(
        string scopedDataDecsription,
        Func<Task<TData?>> dataRetrievalFunc,
        Action<IScopedData<TData?>> action
    )
    {
        Store.AddItem(
            null,
            new Scope<TData>(
                //Store,
                new ScopedData<TData?>(scopedDataDecsription, dataRetrievalFunc),
                action
            )
        );
    }

    public void Scope<TData>(
        string scopedDataDecsription,
        Func<TValidationType, Task<TData?>> dataRetrievalFunc,
        Action<IScopedData<TData?>> action
    )
    {
        Store.AddItem(
            null,
            new Scope<TData>(
                //Store,
                new ScopedData<TValidationType, TData?>(scopedDataDecsription, dataRetrievalFunc),
                action
            )
        );
    }

    public void Scope<TData>(
        string scopedDataDecsription,
        Func<TValidationType, TData?> dataRetrievalFunc,
        Action<IScopedData<TData?>> action
    )
    {
        Store.AddItem(
            null,
            new Scope<TData>(
                //Store,
                new ScopedData<TValidationType, TData?>(scopedDataDecsription, dataRetrievalFunc),
                action
            )
        );
    }

    public void When(string whenDescription, Func<TValidationType, Task<bool>> ifTrue, Action rules)
    {
        var context = new WhenStringValidator<TValidationType>(
            //Store,
            whenDescription,
            ifTrue,
            rules
        );
        Store.AddItem(
            null, context);
    }

    public void ScopeWhen<TPassThrough>(
        string whenDescription,
        Func<TValidationType, bool> ifTrue,
        string scopedDescription,
        Func<TValidationType, TPassThrough> scoped,
        Action<IScopedData<TPassThrough>> rules
    )
    {
        var context = new WhenScopedResultValidator<TValidationType, TPassThrough>(
            //Store,
            whenDescription,
            ifTrue,
            new ScopedData<TValidationType, TPassThrough>(scopedDescription, scoped),
            rules
        );
        Store.AddItem(
            null, context);
    }

    public void ScopeWhen<TPassThrough>(
        string whenDescription,
        Func<TValidationType, Task<bool>> ifTrue,
        string scopedDescription,
        Func<TValidationType, Task<TPassThrough>> scoped,
        Action<IScopedData<TPassThrough>> rules
    )
    {
        var context = new WhenScopedResultValidator<TValidationType, TPassThrough>(
            //Store,
            whenDescription,
            ifTrue,
            new ScopedData<TValidationType, TPassThrough>(scopedDescription, scoped),
            rules
        );
        Store.AddItem(
            null, context);
    }

    

    public void ScopeWhen<TPassThrough>(
        string scopedDescription,
        Func<TValidationType, Task<TPassThrough>> scoped,
        string whenDescription,
        Func<TValidationType, TPassThrough, Task<bool>> ifTrue,
        Action<ScopedData<TValidationType, TPassThrough>> rules
    )
    {
        var context = new WhenScopeToValidator<TValidationType, TPassThrough>(
            //Store,
            whenDescription,
            new ScopedData<TValidationType, TPassThrough>(scopedDescription, scoped),
            ifTrue,
            rules
        );
        Store.AddItem(
            null, context);
    }

    public void ScopeWhen<TPassThrough>(
       string scopedDescription,
       Func<TValidationType, Task<TPassThrough>> scoped,
       string whenDescription,
       Func<TValidationType, TPassThrough, bool> ifTrue,
       Action<ScopedData<TValidationType, TPassThrough>> rules
   )
    {
        var context = new WhenScopeToValidator<TValidationType, TPassThrough>(
            //Store,
            whenDescription,
            new ScopedData<TValidationType, TPassThrough>(scopedDescription, scoped),
            ifTrue,
            rules
        );
        Store.AddItem(
            null, context);
    }

    public void Include(AbstractSubValidator<TValidationType> subValidator)
    {
        Store.Merge(subValidator.Store);
        subValidator.Store.ReplaceInternalStore(Store);
        //subValidator.ReplaceStore(Store);
    }

    public IValidationStore Store { get; private set; }

    public void ReplaceStore(IValidationStore store)
    {
        Store.ReplaceInternalStore(store);
    }
}
