using System;
using System.Threading.Tasks;
using PopValidations.Execution.Stores;
using PopValidations.Scopes;

namespace PopValidations.Scopes.Whens;

public sealed class WhenStringValidator<TValidationType> : ScopeBase
{
    private readonly string whenDescription;
    private readonly Func<TValidationType, Task<bool>> ifTrue;
    private readonly Action rules;
    public override string Name => whenDescription;
    WhenStringValidator_IfTrue<TValidationType> something;

    public WhenStringValidator(
        ValidationConstructionStore validatorStore,
        string whenDescription,
        Func<TValidationType, Task<bool>> ifTrue,
        Action rules
    ) : base(validatorStore)
    {
        this.whenDescription = whenDescription;
        this.ifTrue = ifTrue;
        this.rules = rules;
        something = new WhenStringValidator_IfTrue<TValidationType>(ifTrue);
        Decorator = (item) => new WhenValidationItemDecorator<TValidationType>(item, something, null);
    }

    protected override void InvokeScopeContainer(ValidationConstructionStore store, object? value)
    {
        rules.Invoke();
    }

    protected override void InvokeScopeContainerToDescribe(ValidationConstructionStore store)
    {
        rules.Invoke();
    }
}