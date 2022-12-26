using System;
using PopValidations.Execution.Stores;
using PopValidations.Execution.Stores.Internal;

namespace PopValidations.FieldDescriptors.Base;

public class FieldDescriptionExpandableWrapper<TValidationType, TFieldType>
    : IExpandableEntity
{
    public bool IgnoreScope => false;
    private readonly IFieldDescriptor<TValidationType, TFieldType> fieldDescriptor;
    private IExpandableEntity component;
    public Func<IValidatableStoreItem, IValidatableStoreItem>? Decorator => null;
    object? RetrievedValue = null;
    bool ValueHasBeenRetrieved = false;

    public void AsVital() { IsVital = true; }

    public FieldDescriptionExpandableWrapper(
        IFieldDescriptor<TValidationType, TFieldType> fieldDescriptor,
        bool isVital,
        IExpandableEntity component
    )
    {
        this.fieldDescriptor = fieldDescriptor;
        IsVital = isVital;
        this.component = component;
    }

    public bool IsVital { get; protected set; }

    public void ExpandToValidate(ValidationConstructionStore store, object? value)
    {
        if (IsVital) component.AsVital();

        component.ExpandToValidate(store, value);
    }

    public void ExpandToDescribe(ValidationConstructionStore store)
    {
        component.ExpandToDescribe(store);
    }

    public object? GetValue(object? value)
    {
        if (ValueHasBeenRetrieved) return RetrievedValue;

        if (value is TValidationType result && result != null)
        {
            RetrievedValue = fieldDescriptor.PropertyToken.Expression.Compile().Invoke(result);
            ValueHasBeenRetrieved = true;
        }
        return RetrievedValue;
    }
}