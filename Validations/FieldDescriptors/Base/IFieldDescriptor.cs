using PopValidations.Execution.Stores;
using PopValidations.Validations.Base;
using PopValidations.ValidatorInternals;

namespace PopValidations.FieldDescriptors.Base;

public interface IFieldDescripor_Internal<TValidationType, TFieldType>  : IFieldDescriptor<TValidationType, TFieldType>
{
    IPropertyExpressionToken<TFieldType> PropertyToken { get; }
    IValidationStore Store { get; }
}

public interface IFieldDescriptor<TValidationType, TFieldType> : IFieldDescriptorOutline
{
    void AddSubValidator(ISubValidatorClass<TFieldType> component);
    void AddSelfDescribingEntity(IExpandableEntity component);

    void NextValidationIsVital();
    void SetAlwaysVital();
    void AddValidation(IValidationComponent validation);
}