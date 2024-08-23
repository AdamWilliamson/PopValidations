using System;
using System.Collections.Generic;
using PopValidations.Execution.Stores;
using PopValidations.Execution.Stores.Internal;
using PopValidations.FieldDescriptors.Base;
using PopValidations.ValidatorInternals;

namespace PopValidations;

public abstract class AbstractSubValidator<TValidationType>
    : AbstractValidatorBase<TValidationType>, ISubValidatorClass<TValidationType>
{
    bool IExpandableEntity.IgnoreScope => false;
    Func<IValidatableStoreItem, IFieldDescriptorOutline?, IValidatableStoreItem>? IExpandableEntity.Decorator => null;

    void IExpandableEntity.AsVital() { }

    protected AbstractSubValidator() : base(null, new ValidationSubStore()) { }

    void IExpandableEntity.ReHomeScopes(IFieldDescriptorOutline fieldDescriptorOutline) { }

    void IExpandableEntity.ExpandToValidate(ValidationConstructionStore store, object? value)
    {
        store.AddExpandedItemsForValidation(((IStoreContainer)this).Store, value);
    }

    void IExpandableEntity.ExpandToDescribe(ValidationConstructionStore store)
    {
        store.AddExpandedItemsForDescription(((IStoreContainer)this).Store);
    }

    void IExpandableEntity.ChangeStore(IValidationStore store)
    {
        ((IStoreContainer)this).Store.ReplaceInternalStore(store);
    }

    void IExpandableEntity.UpdateContext(Dictionary<string, object?> context){}
}
