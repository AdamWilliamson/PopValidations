using PopValidations.Execution.Stores;
using PopValidations.FieldDescriptors.Base;
using PopValidations.Scopes;
using PopValidations.Scopes.Whens;
using PopValidations.Validations.Base;
using System.Xml;

namespace PopValidations_Functional_Testbed;

public sealed class WhenReturningNotValidatingValidator<TValidationType> : ScopeBase
{
    private readonly List<(bool, IValidationComponent)> rules = new();
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
        rules.Add((isVital, component));
    }

    protected override void InvokeScopeContainer(ValidationConstructionStore store, object? value)
    {

    }

    protected override void InvokeScopeContainerToDescribe(ValidationConstructionStore store)
    {
        foreach (var rule in rules)
        {
            owningStore.AddItem(rule.Item1, outline, rule.Item2);
        }
    }

    public override void ChangeStore(IValidationStore store) { }
}


public interface IReturnDescriptor {
    void AddValidation(IValidationComponent validation);
}
public interface IReturnDescriptor<TReturnType> : IReturnDescriptor {  }

//public class ReturnDescriptor<TReturnType> : {  }

public class ReturnDescriptor<TValidationType> : IReturnDescriptor, IFieldDescriptorOutline
{
    private readonly IValidationStore? store;
    protected object? RetrievedValue = null;
    protected bool ValueHasBeenRetrieved = false;
    protected bool _NextValidationVital { get; set; } = false;
    protected bool _AlwaysVital { get; set; } = false;
    public string? PropertyName => (functionDescriptor?.FunctionPropertyToken.Name ?? String.Empty) + $"::Return";

    IFunctionDescriptionFor<TValidationType>? functionDescriptor { get; set; }
    WhenReturningNotValidatingValidator<TValidationType> when;

    public ReturnDescriptor(IValidationStore store, IFunctionDescriptionFor<TValidationType>? functionDescription)
    {
        //Name = name;
        this.store = store;
        this.functionDescriptor = functionDescription;
        //this.visitor = visitor;
        when = new WhenReturningNotValidatingValidator<TValidationType>(store, this);
        store.AddItem(null, when);
    }

    public virtual string AddTo(string existing)
    {
        return functionDescriptor?.FunctionPropertyToken.CombineWithParentProperty(existing) + $"::Return";
    }

    public void AddValidation(IValidationComponent validation)
    {
        //store?.AddItem(_NextValidationVital || _AlwaysVital, this, validation);
        when.AddValidation(_NextValidationVital || _AlwaysVital, validation);
        _NextValidationVital = false;
    }

    public virtual object? GetValue(object? value)
    {
        return null;
        //if (ValueHasBeenRetrieved)
        //    return RetrievedValue;

        //if (value is TValidationType result && result != null)
        //{
        //    RetrievedValue = functionDescriptor.FunctionPropertyToken.Execute(result);
        //    ValueHasBeenRetrieved = true;
        //}
        //return RetrievedValue;
    }
}



public class ReturnDescriptor<TReturnType, TValidationType> : IReturnDescriptor<TReturnType>, IFieldDescriptorOutline
{
    private readonly IValidationStore? store;
    protected object? RetrievedValue = null;
    protected bool ValueHasBeenRetrieved = false;
    protected bool _NextValidationVital { get; set; } = false;
    protected bool _AlwaysVital { get; set; } = false;
    public string? PropertyName => (functionDescriptor?.FunctionPropertyToken.Name ?? String.Empty) + $"::Return({typeof(TReturnType).Name})";

    IFunctionDescriptionFor<TValidationType>? functionDescriptor { get; set; }

    WhenReturningNotValidatingValidator<TValidationType> when;

    public ReturnDescriptor(IValidationStore store, IFunctionDescriptionFor<TValidationType>? functionDescription)
    {
        //Name = name;
        this.store = store;
        this.functionDescriptor = functionDescription;
        //this.visitor = visitor;
        when = new WhenReturningNotValidatingValidator<TValidationType>(store, this);
        //store?.AddItem(_NextValidationVital || _AlwaysVital, this, validation);
        store.AddItem(null, when);
    }

    public virtual string AddTo(string existing)
    {
        return functionDescriptor?.FunctionPropertyToken.CombineWithParentProperty(existing) + $"::Return({typeof(TReturnType).Name})";
    }

    public void AddValidation(IValidationComponent validation) 
    {
        //store?.AddItem(_NextValidationVital || _AlwaysVital, this, validation);
        when.AddValidation(_NextValidationVital || _AlwaysVital, validation);
        _NextValidationVital = false;
    }

    public virtual object? GetValue(object? value)
    {
        return null;
        //if (ValueHasBeenRetrieved)
        //    return RetrievedValue;

        //if (value is TValidationType result && result != null)
        //{
        //    RetrievedValue = functionDescriptor.FunctionPropertyToken.Execute(result);
        //    ValueHasBeenRetrieved = true;
        //}
        //return RetrievedValue;
    }
}