using PopValidations.Execution.Stores;
using PopValidations.Validations.Base;

namespace PopValidations.FieldDescriptors.Base;

public interface IFieldDescriptor<TValidationType, TFieldType> : IFieldDescriptorOutline
{
    IPropertyExpressionToken<TValidationType, TFieldType?> PropertyToken { get; }
    ValidationConstructionStore Store { get; }
    void AddValidation(IExpandableEntity component);
    void AddSelfDescribingEntity(IExpandableEntity component);

    void NextValidationIsVital();
    void AddValidation(IValidationComponent validation);
}