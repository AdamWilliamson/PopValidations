using ApiValidations.Descriptors.Core;
using PopValidations.Execution.Stores;
using PopValidations.FieldDescriptors.Base;
using PopValidations.Scopes;
using PopValidations.Scopes.Whens;
using PopValidations.Validations.Base;
using PopValidations.ValidatorInternals;

namespace ApiValidations.Descriptors;

public sealed class WhenReturningNotValidatingValidator<TValidationType> : ScopeBase
{
    private readonly List<Action<IValidationStore>> rules = new();
    private readonly IValidationStore owningStore;
    private readonly IFieldDescriptorOutline outline;

    public override string Name => string.Empty;
    public override bool IgnoreScope => true;

    public WhenReturningNotValidatingValidator(IValidationStore owningStore, IFieldDescriptorOutline outline)
    {
        Decorator = (item, fieldDescriptor) => new WhenValidationItemDecorator<TValidationType>(
            item,
            new WhenStringValidator_IfTrue<TValidationType>((_) => Task.FromResult(false)),
            fieldDescriptor
        );

        this.owningStore = owningStore;
        this.outline = outline;
    }

    public void AddValidation(bool isVital, IValidationComponent component)
    {
        rules.Add((store) => store.AddItem(isVital, outline, component));
    }

    protected override void InvokeScopeContainer(ValidationConstructionStore store, object? value) { }

    protected override void InvokeScopeContainerToDescribe(ValidationConstructionStore store)
    {
        foreach (var rule in rules)
        {
            rule.Invoke(owningStore);
        }
    }

    public override void ChangeStore(IValidationStore store) { }
}

public sealed class WhenReturningNotValidatingValidator<TValidationType, TReturnType> : ScopeBase
{
    private readonly List<Action<IValidationStore>> rules = new();
    private readonly IValidationStore owningStore;
    private readonly IFieldDescriptorOutline outline;

    public override string Name => string.Empty;
    public override bool IgnoreScope => true;

    public WhenReturningNotValidatingValidator(IValidationStore owningStore, IFieldDescriptorOutline outline)
    {
        Decorator = (item, fieldDescriptor) => new WhenValidationItemDecorator<TValidationType>(
            item,
            new WhenStringValidator_IfTrue<TValidationType>((_) => Task.FromResult(false)),
            fieldDescriptor
        );

        this.owningStore = owningStore;
        this.outline = outline;
    }

    public void AddValidation(bool isVital, IValidationComponent component)
    {
        rules.Add((store) => store.AddItem(isVital, outline, component));
    }

    public void AddSubValidator(ISubValidatorClass<TReturnType> component)
    {
        rules.Add((store) => {
            foreach (var item in component.Store.GetItems())
            {
                owningStore.AddItemToCurrentScope(outline, item);
            }

            component.ChangeStore(owningStore);
        });
    }

    protected override void InvokeScopeContainer(ValidationConstructionStore store, object? value){}

    protected override void InvokeScopeContainerToDescribe(ValidationConstructionStore store)
    {
        foreach (var rule in rules)
        {
            rule.Invoke(owningStore);
        }
    }

    public override void ChangeStore(IValidationStore store) { }
}

public interface IReturnDescriptor: IFieldDescriptorOutline
{
    void AddValidation(IValidationComponent validation);
    
}

public interface IReturnDescriptor<TReturnType> : IReturnDescriptor 
{
    IReturnDescriptor<TReturnType> NextValidationIsVital();
    IReturnDescriptor<TReturnType> SetAlwaysVital();
    void AddSubValidator(ISubValidatorClass<TReturnType> component);
}

public class ReturnDescriptor<TValidationType> : IReturnDescriptor, IFieldDescriptorOutline
{
    protected object? RetrievedValue = null;
    protected bool ValueHasBeenRetrieved = false;
    protected bool _NextValidationVital { get; set; } = false;
    protected bool _AlwaysVital { get; set; } = false;
    public string PropertyName => (functionDescriptor?.Name ?? string.Empty) + $"::Return";

    IFunctionExpressionToken functionDescriptor { get; set; }
    WhenReturningNotValidatingValidator<TValidationType> when;

    public ReturnDescriptor(IValidationStore store, IFunctionExpressionToken functionDescription)
    {
        this.functionDescriptor = functionDescription;
        when = new WhenReturningNotValidatingValidator<TValidationType>(store, this);
        store.AddItem(null, when);
    }

    public virtual string AddTo(string existing)
    {
        return functionDescriptor.CombineWithParentProperty(existing) + $"::Return";
    }

    public void AddValidation(IValidationComponent validation)
    {
        when.AddValidation(_NextValidationVital || _AlwaysVital, validation);
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

public class ReturnDescriptor<TReturnType, TValidationType> : IReturnDescriptor<TReturnType>, IFieldDescriptorOutline
{
    protected object? RetrievedValue = null;
    protected bool ValueHasBeenRetrieved = false;
    protected bool _NextValidationVital { get; set; } = false;
    protected bool _AlwaysVital { get; set; } = false;
    public string PropertyName => (functionDescriptor.Name ?? string.Empty) + $"::Return({typeof(TReturnType).Name})";

    IFunctionExpressionToken functionDescriptor { get; set; }

    WhenReturningNotValidatingValidator<TValidationType, TReturnType> when;

    public ReturnDescriptor(IValidationStore store, IFunctionExpressionToken functionDescription)
    {
        functionDescriptor = functionDescription;
        when = new WhenReturningNotValidatingValidator<TValidationType, TReturnType>(store, this);
        store.AddItem(null, when);
    }

    public virtual string AddTo(string existing)
    {
        return functionDescriptor.CombineWithParentProperty(existing) + $"::Return({typeof(TReturnType).Name})";
    }

    public void AddValidation(IValidationComponent validation)
    {
        when.AddValidation(_NextValidationVital || _AlwaysVital, validation);
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
        when.AddSubValidator(component);

        _NextValidationVital = false;
    }

    public virtual object? GetValue(object? value)
    {
        return null;
    }
}