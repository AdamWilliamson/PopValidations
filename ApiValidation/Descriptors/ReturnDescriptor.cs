using ApiValidations.Descriptors.Core;
using ApiValidations.Helpers;
using PopValidations.Execution.Stores;
using PopValidations.FieldDescriptors.Base;
using PopValidations.Validations.Base;
using PopValidations.ValidatorInternals;

namespace ApiValidations.Descriptors;

public interface IReturnDescriptor: IFieldDescriptorOutline
{
    void AddValidation(IValidationComponent validation);
}

public interface IReturnDescriptor<TReturnType> : IReturnDescriptor 
{
    IReturnDescriptor<TReturnType> NextValidationIsVital();
    IReturnDescriptor<TReturnType> SetAlwaysVital();
    void AddSubValidator(ISubValidatorClass<TReturnType> component);
    void AddSelfDescribingEntity(IExpandableEntity component);
}

public class ReturnDescriptor<TValidationType> : IReturnDescriptor, IFieldDescriptorOutline, IReturnDescriptor_Internal
{
    protected object? RetrievedValue = null;
    protected bool ValueHasBeenRetrieved = false;
    private readonly IValidationStore store;

    protected bool _NextValidationVital { get; set; } = false;
    protected bool _AlwaysVital { get; set; } = false;
    public string PropertyName => (_functionDescriptor?.Name ?? string.Empty) + $"::Return";

    IFunctionExpressionToken _functionDescriptor { get; set; }
    IFunctionExpressionToken IReturnDescriptor_Internal.FunctionDescriptor => _functionDescriptor;

    public ReturnDescriptor(IValidationStore store, IFunctionExpressionToken functionDescription)
    {
        this.store = store;
        this._functionDescriptor = functionDescription;
    }

    public virtual string AddTo(string existing)
    {
        return _functionDescriptor.CombineWithParentProperty(existing) + $"::Return";
    }

    public void AddValidation(IValidationComponent validation)
    {
        store.AddItem(_NextValidationVital || _AlwaysVital, this, validation);
        _NextValidationVital = false;
    }

    public IReturnDescriptor NextValidationIsVital()
    {
        _NextValidationVital = true;
        return this;
    }

    public IReturnDescriptor SetAlwaysVital()
    {
        _AlwaysVital = true;
        return this;
    }

    public virtual object? GetValue(object? value)
    {
        return null;
    }
}

public class ReturnDescriptor<TReturnType, TValidationType> : IReturnDescriptor<TReturnType>, IFieldDescriptorOutline, IReturnDescriptor_Internal
{
    protected object? RetrievedValue = null;
    protected bool ValueHasBeenRetrieved = false;
    private readonly IValidationStore store;

    protected bool _NextValidationVital { get; set; } = false;
    protected bool _AlwaysVital { get; set; } = false;
    public string PropertyName => (_functionDescriptor.Name ?? string.Empty) + $"::Return({GenericNameHelper.GetNameWithoutGenericArity(typeof(TReturnType))})";

    IFunctionExpressionToken _functionDescriptor { get; set; }
    IFunctionExpressionToken IReturnDescriptor_Internal.FunctionDescriptor => _functionDescriptor;

    public ReturnDescriptor(IValidationStore store, IFunctionExpressionToken functionDescription)
    {
        this.store = store;
        _functionDescriptor = functionDescription;
    }

    public virtual string AddTo(string existing)
    {
        return _functionDescriptor.CombineWithParentProperty(existing) + $"::Return({GenericNameHelper.GetNameWithoutGenericArity(typeof(TReturnType))})";
    }

    public void AddValidation(IValidationComponent validation)
    {
        store.AddItem(_NextValidationVital || _AlwaysVital, this, validation);
        _NextValidationVital = false;
    }

    public IReturnDescriptor<TReturnType> NextValidationIsVital()
    {
        _NextValidationVital = true;
        return this;
    }

    public IReturnDescriptor<TReturnType> SetAlwaysVital()
    {
        _AlwaysVital = true;
        return this;
    }

    public void AddSubValidator(ISubValidatorClass<TReturnType> component)
    {
        foreach (var item in component.Store.GetItems())
        {
            store.AddItemToCurrentScope(this, item);
        }

        component.ChangeStore(store);

        _NextValidationVital = false;
    }

    public void AddSelfDescribingEntity(IExpandableEntity component)
    {
        if (_NextValidationVital || _AlwaysVital) component.AsVital();

        store.AddItem(
            null,
            component
        );
        _NextValidationVital = false;
    }

    public virtual object? GetValue(object? value)
    {
        return null;
    }
}