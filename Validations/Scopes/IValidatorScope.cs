using PopValidations.Execution.Stores;
using PopValidations.FieldDescriptors.Base;

namespace PopValidations.Scopes;

public interface IValidatorScope : IParentScope, IExpandableEntity
{
    //void ExpandToValidate(ValidationConstructionStore store, object? value);
    //void SetCurrentFieldExecutor(IFieldDescriptorOutline fieldDescriptor);
    //IFieldDescriptorOutline GetCurrentFieldExecutor();
}