using System;
using PopValidations.Execution.Stores;
using PopValidations.Execution.Stores.Internal;
using PopValidations.Validations.Base;

namespace PopValidations.FieldDescriptors.Base;

public class FieldDescriptionNonExpandableWrapper : IExpandableEntity
{
    public bool IgnoreScope => false;
    private readonly IFieldDescriptorOutline fieldDescriptorOutline;
    private IValidationComponent component;
    public Func<IValidatableStoreItem, IValidatableStoreItem>? Decorator => null;
    public void AsVital() { IsVital = true; }

    public FieldDescriptionNonExpandableWrapper(
        IFieldDescriptorOutline fieldDescriptorOutline,
        bool isVital,
        IValidationComponent component)
    {
        this.fieldDescriptorOutline = fieldDescriptorOutline;
        IsVital = isVital;
        this.component = component;
    }

    public bool IsVital { get; protected set; }

    public void ExpandToValidate(ValidationConstructionStore store, object? value)
    {
        store.AddItem(
            IsVital,
            fieldDescriptorOutline,
            component
        );
    }

    public void ExpandToDescribe(ValidationConstructionStore store)
    {
        store.AddItem(
            IsVital,
            fieldDescriptorOutline,
            component
        );
    }
}
