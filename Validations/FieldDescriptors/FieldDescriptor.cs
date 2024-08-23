using PopValidations.Execution.Stores;
using PopValidations.FieldDescriptors.Base;
using PopValidations.Validations.Base;
using PopValidations.ValidatorInternals;
using System.Collections.Generic;

namespace PopValidations.FieldDescriptors;

public class FieldDescriptor<TValidationType, TFieldType>
    : IFieldDescripor_Internal<TValidationType, TFieldType>
{
    public IPropertyExpressionToken<TFieldType> PropertyToken { get; }
    protected object? RetrievedValue = null;
    protected bool ValueHasBeenRetrieved = false;
    public IValidationStore Store { get; }
    public string PropertyName => PropertyToken.Name;

    public virtual string AddTo(string existing)
    {
        return PropertyToken.CombineWithParentProperty(existing);
    }

    protected bool _NextValidationVital { get; set; } = false;
    protected bool _AlwaysVital { get; set; } = false;

    public void NextValidationIsVital()
    {
        _NextValidationVital = true;
    }

    public void SetAlwaysVital()
    {
        _AlwaysVital = true;
    }

    public FieldDescriptor(
        IPropertyExpressionToken<TFieldType> propertyToken,
        IValidationStore store
    )
    {
        PropertyToken = propertyToken;
        Store = store;
    }

    public void UpdateContext(Dictionary<string, object?> context){
        // Currently contains no context
    }

    public void AddSubValidator(ISubValidatorClass<TFieldType> component)
    {
        foreach (var item in component.Store.GetItems())
        {
            Store.AddItemToCurrentScope(this, item);
        }
        
        component.ChangeStore(Store);
        
        _NextValidationVital = false;
    }

    public void AddSelfDescribingEntity(IExpandableEntity component)
    {
        if (_NextValidationVital || _AlwaysVital) component.AsVital();

        Store.AddItem(
            null,
            component
        );    
        _NextValidationVital = false;
    }

    public void AddValidation(IValidationComponent validation)
    {
        Store.AddItem(_NextValidationVital || _AlwaysVital, this, validation);
        _NextValidationVital = false;
    }

    public virtual object? GetValue(object? value)
    {
        if (ValueHasBeenRetrieved)
            return RetrievedValue;

        if (value is TValidationType result && result != null)
        {
            RetrievedValue = PropertyToken.Execute(result);
            ValueHasBeenRetrieved = true;
        }
        return RetrievedValue;
    }
}
