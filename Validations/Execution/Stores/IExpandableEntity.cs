using System;
using PopValidations.Execution.Stores.Internal;
using PopValidations.FieldDescriptors.Base;

namespace PopValidations.Execution.Stores;

public interface IExpandableEntity
{
    Func<IValidatableStoreItem, IFieldDescriptorOutline?, IValidatableStoreItem>? Decorator { get; }
    void ReHomeScopes(IFieldDescriptorOutline fieldDescriptorOutline);
    void ExpandToValidate(ValidationConstructionStore store, object? value);
    void ExpandToDescribe(ValidationConstructionStore store);
    void AsVital();
    bool IgnoreScope { get; }
    void ChangeStore(IValidationStore store);
}
