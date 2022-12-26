using PopValidations.Execution.Stores;
using PopValidations.FieldDescriptors.Base;
using PopValidations.Validations.Base;

namespace PopValidations.FieldDescriptors;

public class FieldDescriptor<TValidationType, TFieldType>
    : IFieldDescriptor<TValidationType, TFieldType>
{
    public PropertyExpressionTokenBase<TValidationType, TFieldType> PropertyToken { get; }
    object? RetrievedValue = null;
    bool ValueHasBeenRetrieved = false;
    public ValidationConstructionStore Store { get; }
    public string PropertyName => PropertyToken.Name;

    public string AddTo(string existing)
    {
        return PropertyToken.CombineWithParentProperty(existing);
    }

    protected bool _NextValidationVital { get; set; } = false;
    protected bool _AlwaysVital { get; set; } = false;

    public void NextValidationIsVital()
    {
        _NextValidationVital = true;
    }

    public void IsAlwaysVital()
    {
        _AlwaysVital = true;
    }

    public FieldDescriptor(
        PropertyExpressionTokenBase<TValidationType, TFieldType> propertyToken,
        ValidationConstructionStore store
    )
    {
        PropertyToken = propertyToken;
        Store = store;
    }

    public void AddValidation(IExpandableEntity component)
    {
        Store.AddItem(
            this,
            new FieldDescriptionExpandableWrapper<TValidationType, TFieldType>(
                this,
                _NextValidationVital || _AlwaysVital,
                component
            )
        );
        _NextValidationVital = false;
    }

    public void AddValidation(IValidationComponent validation)
    {
        Store.AddItem(_NextValidationVital || _AlwaysVital, this, validation);
        _NextValidationVital = false;
    }

    public object? GetValue(object? value)
    {
        if (ValueHasBeenRetrieved)
            return RetrievedValue;

        if (value is TValidationType result && result != null)
        {
            RetrievedValue = PropertyToken.Expression.Compile().Invoke(result);
            ValueHasBeenRetrieved = true;
        }
        return RetrievedValue;
    }
}
