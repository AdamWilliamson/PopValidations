using ApiValidations.Descriptors.Core;
using PopValidations.Execution.Stores;
using PopValidations.FieldDescriptors.Base;
using PopValidations.Validations.Base;
using PopValidations.ValidatorInternals;

namespace ApiValidations.Descriptors;

public class ForEachReturnDescriptor<TEnumeratedFieldType, TReturnType>
    : IReturnDescriptor<TReturnType>, IFieldDescriptorOutline, IReturnDescriptor_Internal
{
    protected object? RetrievedValue = null;
    protected bool ValueHasBeenRetrieved = false;
    private readonly IValidationStore store;
    private readonly int index;

    protected bool _NextValidationVital { get; set; } = false;
    protected bool _AlwaysVital { get; set; } = false;
    public string PropertyName => (_functionDescriptor.Name ?? string.Empty) + $"::Return({typeof(TReturnType).Name})[{(index >= 0 ? index.ToString() : 'n')}]";

    IFunctionExpressionToken _functionDescriptor { get; set; }
    IFunctionExpressionToken IReturnDescriptor_Internal.FunctionDescriptor => _functionDescriptor;

    public ForEachReturnDescriptor(IValidationStore store, IFunctionExpressionToken functionDescription, int index)
    {
        this.store = store;
        _functionDescriptor = functionDescription;
        this.index = index;
    }

    public virtual string AddTo(string existing)
    {
        return _functionDescriptor.CombineWithParentProperty(existing) + $"::Return({typeof(TReturnType).Name})[{(index >= 0 ? index.ToString() : 'n')}]";
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
