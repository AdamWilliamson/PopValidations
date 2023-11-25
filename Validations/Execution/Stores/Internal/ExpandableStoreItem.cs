using System;
using PopValidations.FieldDescriptors.Base;

namespace PopValidations.Execution.Stores.Internal;

public class ExpandableStoreItem : IExpandableStoreItem
{
    public bool IgnoreScope => false;

    public ExpandableStoreItem(
        ScopeParent? scopeParent,
        IFieldDescriptorOutline? fieldDescriptor,
        IExpandableEntity component
    )
    {
        ScopeParent = scopeParent;
        FieldDescriptor = fieldDescriptor;
        Component = component;
    }

    public void AsVital()
    {
        Component.AsVital();
    }

    public virtual void ReHomeScopes(IFieldDescriptorOutline fieldDescriptorOutline) {
        Component.ReHomeScopes(fieldDescriptorOutline);
    }

    public ScopeParent? ScopeParent { get; set; }
    public IExpandableEntity Component { get; }
    public IFieldDescriptorOutline? FieldDescriptor { get; set; }
    public Func<IValidatableStoreItem, IFieldDescriptorOutline?, IValidatableStoreItem>? Decorator => Component.Decorator;

    public void ExpandToValidate(ValidationConstructionStore store, object? value)
    {
        var newValue = FieldDescriptor?.GetValue(value) ?? value;
        Component.ExpandToValidate(store, newValue);
    }

    public void ExpandToDescribe(ValidationConstructionStore store)
    {
        Component.ExpandToDescribe(store);
    }

    public void ChangeStore(IValidationStore store)
    {
        Component?.ChangeStore(store);
    }
}

public class WrappingExpandableStoreItem : IExpandableStoreItem
{
    public bool IgnoreScope => false;

    public WrappingExpandableStoreItem(
        ScopeParent? scopeParent,
        IFieldDescriptorOutline? fieldDescriptor,
        IExpandableEntity component
    )
    {
        ScopeParent = scopeParent;
        FieldDescriptor = fieldDescriptor;
        Component = component;
    }

    public void AsVital()
    {
        Component.AsVital();
    }

    public virtual void ReHomeScopes(IFieldDescriptorOutline fieldDescriptorOutline)
    {
        Component.ReHomeScopes(fieldDescriptorOutline);
    }

    public ScopeParent? ScopeParent { get; set; }
    public IExpandableEntity Component { get; }
    public IFieldDescriptorOutline? FieldDescriptor { get; set; }
    public Func<IValidatableStoreItem, IFieldDescriptorOutline?, IValidatableStoreItem>? Decorator => null;

    public void ExpandToValidate(ValidationConstructionStore store, object? value)
    {
        store.AddItem(FieldDescriptor, Component);
    }

    public void ExpandToDescribe(ValidationConstructionStore store)
    {
        store.AddItem(FieldDescriptor, Component);
    }

    public void ChangeStore(IValidationStore store)
    {
        Component?.ChangeStore(store);
    }
}
