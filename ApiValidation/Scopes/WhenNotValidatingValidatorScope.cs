using ApiValidations.Descriptors;
using PopValidations.Execution.Stores;
using PopValidations.Scopes;
using PopValidations.Scopes.Whens;

namespace ApiValidations.Scopes;

public sealed class WhenNotValidatingValidatorScope<TValidationType, TParamType> : ScopeBase
{
    //private readonly Action rules;
    private readonly ParamDescriptor<TParamType, TValidationType> paramDescriptor;
    private readonly List<Action<ParamDescriptor<TParamType, TValidationType>>> addValidationActions;

    public override string Name => string.Empty;
    public override bool IgnoreScope => true;

    public WhenNotValidatingValidatorScope(
        //Action rules,
        ParamDescriptor<TParamType, TValidationType> paramDescriptor
,
        List<Action<ParamDescriptor<TParamType, TValidationType>>> addValidationActions)
    {
        //this.rules = rules;
        this.paramDescriptor = paramDescriptor;
        this.addValidationActions = addValidationActions;
        Decorator = (item, fieldDescriptor) => new WhenValidationItemDecorator<TValidationType>(
            item,
            new WhenStringValidator_IfTrue<TValidationType>((_) => Task.FromResult(false)),
            fieldDescriptor
        );
    }

    protected override void InvokeScopeContainer(ValidationConstructionStore store, object? value)
    {

    }

    protected override void InvokeScopeContainerToDescribe(ValidationConstructionStore store)
    {
        //rules.Invoke();
        //paramDescriptor.
        foreach (var action in addValidationActions)
        {
            action.Invoke(paramDescriptor);
        }
    }

    public override void ChangeStore(IValidationStore store) { }
}
