using System;
using System.Threading.Tasks;
using PopValidations.Execution.Stores;
using PopValidations.Validations.Base;

namespace PopValidations.Scopes.Whens;

public sealed class WhenScopedResultValidator<TValidationType, TPassThrough> : ScopeBase
{
    private readonly string whenDescription;
    private readonly Func<TValidationType, Task<bool>> ifTrue;
    private readonly ScopedData<TValidationType, TPassThrough> scoped;
    private readonly Action<ScopedData<TValidationType, TPassThrough>> rules;
    public override string Name => whenDescription;
    WhenStringValidator_IfTrue<TValidationType> something;

    public WhenScopedResultValidator(
        ValidationConstructionStore validatorStore,
        string whenDescription,
        Func<TValidationType, Task<bool>> ifTrue,
        Func<TValidationType, Task<TPassThrough>> scoped,
        Action<ScopedData<TValidationType, TPassThrough>> rules
    ) : base(validatorStore)
    {
        this.whenDescription = whenDescription;
        this.ifTrue = ifTrue;
        this.scoped = new ScopedData<TValidationType, TPassThrough>(scoped);
        this.rules = rules;

        something = new WhenStringValidator_IfTrue<TValidationType>(ifTrue);
        Decorator = (item) => new WhenValidationItemDecorator<TValidationType>(
            item,
            something,
            this.scoped
            );
    }

    protected override void InvokeScopeContainer(ValidationConstructionStore store, object? value)
    {
        rules.Invoke(scoped);
    }

    protected override void InvokeScopeContainerToDescribe(ValidationConstructionStore store)
    {
        rules.Invoke(scoped);
    }
}