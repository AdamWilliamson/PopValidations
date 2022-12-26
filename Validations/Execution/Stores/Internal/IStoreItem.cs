using PopValidations.FieldDescriptors.Base;

namespace PopValidations.Execution.Stores.Internal;

public interface IStoreItem
{
    ScopeParent? ScopeParent { get; set; }
    IFieldDescriptorOutline? FieldDescriptor { get; set; }
}
