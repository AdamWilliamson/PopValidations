using PopValidations.Execution.Stores;
using PopValidations.Validations.Base;
using PopValidations.ValidatorInternals;

namespace PopValidations.FieldDescriptors.Base;

public interface IFieldDescriptor<TValidationType, TFieldType> : IFieldDescriptorOutline
{
    IPropertyExpressionToken<TValidationType, TFieldType?> PropertyToken { get; }
    IValidationStore Store { get; }
    void AddSubValidator(ISubValidatorClass<TFieldType> component);
    void AddSelfDescribingEntity(IExpandableEntity component);

    void NextValidationIsVital();
    void AddValidation(IValidationComponent validation);
}