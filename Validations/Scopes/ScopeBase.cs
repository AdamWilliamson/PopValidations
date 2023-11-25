using System;
using PopValidations.Execution.Stores;
using PopValidations.Execution.Stores.Internal;
using PopValidations.FieldDescriptors.Base;

namespace PopValidations.Scopes;

public abstract class ScopeBase : IValidatorScope
{
    public virtual bool IgnoreScope => false;
    public Guid Id { get; } = Guid.NewGuid();
    public IParentScope? Parent => this;
    public abstract string Name { get; }
    public Func<IValidatableStoreItem, IFieldDescriptorOutline?, IValidatableStoreItem>? Decorator { get; protected set; } = null;
    protected bool IsVital = false;
    public void AsVital() { IsVital = true; }
    protected IFieldDescriptorOutline FieldDescriptor { get; set; }

    public virtual void ReHomeScopes(IFieldDescriptorOutline fieldDescriptorOutline) { }

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

    public abstract void ChangeStore(IValidationStore store);
}
