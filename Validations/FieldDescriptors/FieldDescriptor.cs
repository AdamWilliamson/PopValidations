using PopValidations.Execution.Stores;
using PopValidations.FieldDescriptors.Base;
using PopValidations.Validations.Base;
using PopValidations.ValidatorInternals;

namespace PopValidations.FieldDescriptors;

public class FieldDescriptor<TValidationType, TFieldType>
    : IFieldDescripor_Internal<TValidationType, TFieldType>
{
    public IPropertyExpressionToken<TValidationType, TFieldType?> PropertyToken { get; }
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

    public void IsAlwaysVital()
    {
        _AlwaysVital = true;
    }

    public FieldDescriptor(
        IPropertyExpressionToken<TValidationType, TFieldType?> propertyToken,
        IValidationStore store
    )
    {
        PropertyToken = propertyToken;
        Store = store;
    }

    public void AddSubValidator(ISubValidatorClass<TFieldType> component)
    {
        foreach (var item in component.Store.GetItems())
        {
            Store.AddItemToCurrentScope(this, item);
            //if (item.ScopeParent is IExpandableEntity expandable) 
            //if (item is IExpandableStoreItem expandable && expandable is not null)
            //{
            //    Store.AddItem(this,
            //        new FieldDescriptionExpandableWrapper<TValidationType, TFieldType>(
            //            this,
            //            _NextValidationVital || _AlwaysVital,
            //            //component
            //            expandable
            //        )
            //    );
            //}
            //else if (item is IValidatableStoreItem validatable)
            ////else if(item.ScopeParent is IValidationComponent validationComponent) 
            //{
            //    Store.AddItem(
            //        _NextValidationVital || _AlwaysVital,
            //        this,
            //        validatable
            //    );
            //}
        }
        component.ChangeStore(Store);
        //Store.AddItem(
        //    this,
        //    new FieldDescriptionExpandableWrapper<TValidationType, TFieldType>(
        //        this,
        //        _NextValidationVital || _AlwaysVital,
        //        component
        //    )
        //);
        _NextValidationVital = false;
    }

    public void AddSelfDescribingEntity(IExpandableEntity component)
    {
        if (_NextValidationVital || _AlwaysVital) component.AsVital();

        Store.AddItem(
            null,
            component
            //new FieldDescriptionExpandableWrapper<TValidationType, TFieldType>(
            //    null,
            //    _NextValidationVital || _AlwaysVital,
            //    component
            //)
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
