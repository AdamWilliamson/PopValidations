using System;
using PopValidations.Execution.Stores.Internal;

namespace PopValidations.Execution.Stores;

public interface IExpandableEntity
{
    Func<IValidatableStoreItem, IValidatableStoreItem>? Decorator { get; }
    void ExpandToValidate(ValidationConstructionStore store, object? value);
    void ExpandToDescribe(ValidationConstructionStore store);
    void AsVital();
    bool IgnoreScope { get; }
}
