using ApiValidations.Descriptors.Core;
using ApiValidations.Scopes;
using PopValidations.Execution.Stores;
using PopValidations.FieldDescriptors.Base;
using PopValidations.Validations.Base;
using PopValidations.ValidatorInternals;

namespace ApiValidations.Descriptors;

public interface IParamDescriptor{}
public interface IParamDescriptor<TParamType> : IParamDescriptor, IFieldDescriptorOutline {}
public interface IParamDescriptor_Internal<TParamType> 
    : IParamDescriptor<TParamType>
{
    IParamToken<TParamType> ParamToken { get; }
    IParamVisitor ParamVisitor { get; }
}

public class ParamDescriptor<TParamType, TValidationType> 
    : IParamDescriptor_Internal<TParamType>
{
    public static implicit operator TParamType(ParamDescriptor<TParamType, TValidationType> d)
    {
        d.ToImpl();

#pragma warning disable CS8603 // Possible null reference return.
        return default;
#pragma warning restore CS8603 // Possible null reference return.
    }

    public TConvertType Convert<TConvertType>()
    {
        ToImpl();

#pragma warning disable CS8603 // Possible null reference return.
        return default;
#pragma warning restore CS8603 // Possible null reference return.
    }

    protected virtual void ToImpl()
    {
        var function = this.ParamVisitor.GetCurrentFunction() as IFunctionExpressionToken;
        if (function == null) throw new Exception("Executing conversion wiithout current Function");
        var descriptor = this.ParamVisitor.GetCurrentParamDescriptor();
        if (descriptor == null) throw new Exception("Executing conversion without current Function or Param.");

        this.strategy.SetParamDetails(descriptor!.Name ?? "<Unknown>", descriptor!.Index, function);
        if (descriptor == null) throw new Exception("Cannot convert if the system isnt ready.");
        this.Name = descriptor.Name;
        this.ParamIndex = descriptor.Index;

        var when = new WhenNotValidatingValidatorScope<TValidationType>(() =>
        {
            foreach (var action in this.AddValidationActions)
            {
                action.Invoke(this);
            }
        });

        if (this.store == null) throw new Exception("Param Descriptor has no scope for compilation.");
        this.store?.AddItem(null, when);

    }

    List<Action<ParamDescriptor<TParamType, TValidationType>>> AddValidationActions = new();
    protected readonly IValidationStore? store;
    public IParamVisitor ParamVisitor { get; }

    protected readonly IParamDescriptor_Strategy<TValidationType, TParamType> strategy;
    protected object? RetrievedValue = null;
    protected bool ValueHasBeenRetrieved = false;

    protected bool _NextValidationVital { get; set; } = false;
    protected bool _AlwaysVital { get; set; } = false;
    public string PropertyName => strategy.PropertyName;
    public int? ParamIndex { get; protected set; }

    public Type ParamType => typeof(TParamType);

    public IParamToken<TParamType> ParamToken { get; set; }

    public ParamDescriptor(
        IParamToken<TParamType> paramToken,
        IParamVisitor visitor,
        IParamDescriptor_Strategy<TValidationType, TParamType> strategy)
    {
        ParamToken = paramToken ?? throw new ArgumentNullException(nameof(paramToken));
        ParamVisitor = visitor ?? throw new ArgumentNullException(nameof(visitor));
        this.strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
        store = visitor.GetStore();
    }

    public ParamDescriptor(
        ParamDescriptor<TParamType, TValidationType> toClone)
    {
        ParamToken = toClone.ParamToken;
        store = toClone.store;
        RetrievedValue = toClone.RetrievedValue;
        ValueHasBeenRetrieved = toClone.ValueHasBeenRetrieved;
        _NextValidationVital = toClone._NextValidationVital;
        _AlwaysVital = toClone._AlwaysVital;
        Name = toClone.Name;
        IsNullable = toClone.IsNullable;
        AddValidationActions = toClone.AddValidationActions;
        strategy = toClone.strategy.Clone();
        this.ParamVisitor = toClone.ParamVisitor ?? throw new Exception("ParamVisitor is null somehow");
    }

    public bool IsNullable { get; }
    public string? Name { get; private set; }

    public virtual string AddTo(string existing)
    {
        return strategy.AddTo(existing);
    }

    public ParamDescriptor<TParamType, TValidationType> NextValidationIsVital()
    {
        return CloneAndAdd((d) => d._NextValidationVital = true);
    }

    public ParamDescriptor<TParamType, TValidationType> SetAlwaysVital()
    {
        return CloneAndAdd((d) => d._AlwaysVital = true);
    }

    public ParamDescriptor<TParamType, TValidationType> AddValidation(IValidationComponent validation)
    {
        return CloneAndAdd((d) => d.AddValidationImpl(validation));
    }

    public ParamDescriptor<TParamType, TValidationType> AddSubValidator(ISubValidatorClass<TParamType> component)
    {
        return CloneAndAdd((d) =>
        {
            if (d.store == null) throw new Exception("Store is missing from subvalidator");

            foreach (var item in component.Store.GetItems())
            {
                d.store.AddItemToCurrentScope(d, item);
            }

            component.ChangeStore(d.store!);

            d._NextValidationVital = false;
        });
    }

    public ParamDescriptor<TParamType, TValidationType> CloneAndAdd(Action<ParamDescriptor<TParamType, TValidationType>> action)
    {
        var clone = new ParamDescriptor<TParamType, TValidationType>(this) ?? throw new Exception("Failed to Clone");
        clone.AddValidationActions.Add(action);
        return clone;
    }

    private void AddValidationImpl(IValidationComponent validation)
    {
        if (store == null) throw new Exception("Store is missing");
        store?.AddItem(_NextValidationVital || _AlwaysVital, this, validation);
        _NextValidationVital = false;
    }

    public ParamDescriptor<TParamType, TValidationType> AddSelfDescribingEntity(IExpandableEntity component)
    {
        return CloneAndAdd((d) =>
        {
            if (d.store == null) throw new Exception("Store is missing from subvalidator");

            if (_NextValidationVital || _AlwaysVital) component.AsVital();

            d.store.AddItem(
                null,
                component
            );
            _NextValidationVital = false;
        });
    }

    public virtual object? GetValue(object? value)
    {
        return strategy.GetValue(value);
    }
}
