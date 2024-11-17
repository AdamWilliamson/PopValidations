using ApiValidations.Descriptors.Core;
using ApiValidations.Execution;
using ApiValidations.Helpers;
using PopValidations.Execution.Stores;
using PopValidations.FieldDescriptors.Base;
using PopValidations.Scopes;
using PopValidations.Scopes.Whens;
using PopValidations.Validations.Base;
using PopValidations.ValidatorInternals;
using System.Reflection;

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
    private readonly IFunctionContext context;

    protected bool _NextValidationVital { get; set; } = false;
    protected bool _AlwaysVital { get; set; } = false;
    public string PropertyName => (_functionDescriptor?.Name ?? string.Empty) + $":Return";

    IFunctionExpressionToken _functionDescriptor { get; set; }
    IFunctionExpressionToken IReturnDescriptor_Internal.FunctionDescriptor => _functionDescriptor;

    public ReturnDescriptor(IValidationStore store, IFunctionExpressionToken functionDescription, IFunctionContext context)
    {
        this.store = store;
        this._functionDescriptor = functionDescription;
        this.context = context;
    }

    public bool IsRunning()
    {
        return store.GetContextItem(ApiValidationConstants.MethodResultKey) is not null;
    }

    public void UpdateContext(Dictionary<string, object?> context)
    {
        // Currently contains no context
    }

    public virtual string AddTo(string existing)
    {
        return _functionDescriptor.CombineWithParentProperty(existing) + $":Return";
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

    public IFunctionContext GetContext() {  return context; }
}

public class ReturnDescriptor<TReturnType, TValidationType> : IReturnDescriptor<TReturnType>, IFieldDescriptorOutline, IReturnDescriptor_Internal
{
    protected object? RetrievedValue = null;
    protected bool ValueHasBeenRetrieved = false;
    private readonly IValidationStore store;
    private readonly IFunctionContext context;

    protected bool _NextValidationVital { get; set; } = false;
    protected bool _AlwaysVital { get; set; } = false;
    public string PropertyName => (_functionDescriptor.Name ?? string.Empty)
        + PopApi.Configuation.ReturnDescription.Invoke(typeof(TReturnType));
        //$":Return({GenericNameHelper.GetNameWithoutGenericArity(typeof(TReturnType))})";

    IFunctionExpressionToken _functionDescriptor { get; set; }
    IFunctionExpressionToken IReturnDescriptor_Internal.FunctionDescriptor => _functionDescriptor;

    public ReturnDescriptor(IValidationStore store, IFunctionExpressionToken functionDescription, IFunctionContext context)
    {
        this.store = store;
        _functionDescriptor = functionDescription;
        this.context = context;
    }

    public bool IsRunning()
    {
        return store.GetContextItem(ApiValidationConstants.MethodResultKey) is not null;
    }

    public void UpdateContext(Dictionary<string, object?> context)
    {
        // Currently contains no context
    }

    public virtual string AddTo(string existing)
    {
        return _functionDescriptor.CombineWithParentProperty(existing) 
            + PopApi.Configuation.ReturnDescription.Invoke(typeof(TReturnType));
        //$":Return({GenericNameHelper.GetNameWithoutGenericArity(typeof(TReturnType))})";
    }

    public void AddValidation(IValidationComponent validation)
    {
        var when = new WhenNotValidatingReturnValidatorScope<TValidationType, TReturnType>(
            this,
            [(rd) => {
                rd.store.AddItem(_NextValidationVital || _AlwaysVital, this, validation);
                rd._NextValidationVital = false;
            }
            ]);

        this.store?.AddItem(null, when);



        //store.AddItem(_NextValidationVital || _AlwaysVital, this, validation);
        //_NextValidationVital = false;
    }

    public IReturnDescriptor<TReturnType> NextValidationIsVital()
    {
        var when = new WhenNotValidatingReturnValidatorScope<TValidationType, TReturnType>(
            this,
            [(rd) => {
                rd._NextValidationVital = true;
            }
            ]);

        this.store?.AddItem(null, when);

        //_NextValidationVital = true;
        return this;
    }

    public IReturnDescriptor<TReturnType> SetAlwaysVital()
    {
        var when = new WhenNotValidatingReturnValidatorScope<TValidationType, TReturnType>(
            this,
            [(rd) => {
                rd._AlwaysVital = true;
            }
            ]);

        this.store?.AddItem(null, when);
        //_AlwaysVital = true;
        return this;
    }

    public void AddSubValidator(ISubValidatorClass<TReturnType> component)
    {
        var when = new WhenNotValidatingReturnValidatorScope<TValidationType, TReturnType>(
            this,
            [(rd) => {
                foreach (var item in component.Store.GetItems())
                {
                    rd.store.AddItemToCurrentScope(this, item);
                }

                component.ChangeStore(rd.store);

                rd._NextValidationVital = false;
            }
            ]);

        this.store?.AddItem(null, when);


       
    }

    public void AddSelfDescribingEntity(IExpandableEntity component)
    {
        var when = new WhenNotValidatingReturnValidatorScope<TValidationType, TReturnType>(
            this,
            [(rd) => {
                if (rd._NextValidationVital || rd._AlwaysVital) component.AsVital();

                rd.store.AddItem(
                    null,
                    component
                );
                rd._NextValidationVital = false;
            }
            ]);
        store?.AddItem(null, when);
    }

    public virtual object? GetValue(object? value)
    {
        return (store.GetContextItem(ApiValidationConstants.MethodResultKey) as HeirarchyReturnMethodInfo)?.ReturnValue;
    }

    public IFunctionContext GetContext() { return context; }
}


public sealed class WhenNotValidatingReturnValidatorScope<TValidationType, TReturnType> : ScopeBase
{
    //private readonly Action rules;
    private readonly ReturnDescriptor<TReturnType, TValidationType> returnDescriptor; 
    private readonly List<Action<ReturnDescriptor<TReturnType, TValidationType>>> addValidationActions;

    public override string Name => string.Empty;
    public override bool IgnoreScope => true;

    public WhenNotValidatingReturnValidatorScope(
        ReturnDescriptor<TReturnType, TValidationType> returnDescriptor,
        List<Action<ReturnDescriptor<TReturnType, TValidationType>>> addValidationActions)
    {
        //this.rules = rules;
        this.returnDescriptor = returnDescriptor;
        this.addValidationActions = addValidationActions;

        Decorator = (item, fieldDescriptor) => new WhenValidationItemDecorator<TValidationType>(
            item,
            // This needs to do true/false, depending on whether its Validating an OBJECT vs validating a Function.
            new WhenStringValidator_IfTrue<TValidationType>(
                (_) => Task.FromResult(returnDescriptor.IsRunning())
            ),
            fieldDescriptor
        );
    }

    protected override void InvokeScopeContainer(ValidationConstructionStore store, object? value)
    {
        foreach (var action in addValidationActions)
        {
            action.Invoke(returnDescriptor);
        }
    }

    protected override void InvokeScopeContainerToDescribe(ValidationConstructionStore store)
    {
        //rules.Invoke();
        //paramDescriptor.
        foreach (var action in addValidationActions)
        {
            action.Invoke(returnDescriptor);
        }
    }

    public override void ChangeStore(IValidationStore store) { }
    public override void UpdateContext(Dictionary<string, object?> context)
    {
        returnDescriptor.UpdateContext(context);
    }
}