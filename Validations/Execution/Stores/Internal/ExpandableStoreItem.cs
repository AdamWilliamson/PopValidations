using System;
using PopValidations.FieldDescriptors.Base;

namespace PopValidations.Execution.Stores.Internal;

public class ExpandableStoreItem : IExpandableStoreItem
{
    public bool IgnoreScope => false;

    public ExpandableStoreItem(
        ScopeParent scopeParent,
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

    public ScopeParent? ScopeParent { get; set; }
    public IExpandableEntity Component { get; }
    public IFieldDescriptorOutline? FieldDescriptor { get; set; }
    public Func<IValidatableStoreItem, IValidatableStoreItem>? Decorator => Component.Decorator;
    //public IParentScope Parent => Component.Parent;

    public void ExpandToValidate(ValidationConstructionStore store, object? value)
    {
        var newValue = FieldDescriptor?.GetValue(value) ?? value;
        Component.ExpandToValidate(store, newValue);
    }

    public void ExpandToDescribe(ValidationConstructionStore store)
    {
        Component.ExpandToDescribe(store);
    }
}
