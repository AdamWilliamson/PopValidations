using System;
using PopValidations.Execution.Stores;
using PopValidations.Execution.Stores.Internal;

namespace PopValidations.Scopes;

public abstract class ScopeBase : IValidatorScope, IExpandableEntity
{
    public virtual bool IgnoreScope => false;
    protected readonly ValidationConstructionStore validatorStore;
    public Guid Id { get; } = Guid.NewGuid();
    public IParentScope? Parent => this;
    public abstract string Name { get; }
    public Func<IValidatableStoreItem, IValidatableStoreItem>? Decorator { get; protected set; } = null;
    protected bool IsVital = false;
    public void AsVital() { IsVital = true; }

    public ScopeBase(
        ValidationConstructionStore validatorStore
    )
    {
        this.validatorStore = validatorStore;
    }

    public virtual void ExpandToValidate(ValidationConstructionStore store, object? value)
    {
        InvokeScopeContainer(store, value);
    }

    public void ExpandToDescribe(ValidationConstructionStore store)
    {
        InvokeScopeContainerToDescribe(store);
    }

    protected abstract void InvokeScopeContainer(ValidationConstructionStore store, object? value);
    protected abstract void InvokeScopeContainerToDescribe(ValidationConstructionStore store);
}
