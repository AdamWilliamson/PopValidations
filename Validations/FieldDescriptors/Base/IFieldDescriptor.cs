using PopValidations.Execution.Stores;
using PopValidations.Validations.Base;

namespace PopValidations.FieldDescriptors.Base;

public interface IFieldDescriptor<TValidationType, TFieldType> : IFieldDescriptorOutline
{
    PropertyExpressionTokenBase<TValidationType, TFieldType> PropertyToken { get; }
    ValidationConstructionStore Store { get; }
    void AddValidation(IExpandableEntity component);

    void NextValidationIsVital();
    void AddValidation(IValidationComponent validation);
}