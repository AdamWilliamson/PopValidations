using PopValidations.FieldDescriptors.Base;
using System.Collections.Generic;

namespace PopValidations.Execution.Stores.Internal;

public interface IStoreItem
{
    ScopeParent? ScopeParent { get; set; }
    IFieldDescriptorOutline? FieldDescriptor { get; }
    void UpdateContext(Dictionary<string, object?> context);
}
