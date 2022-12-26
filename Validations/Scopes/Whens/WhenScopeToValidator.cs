using System;
using System.Threading.Tasks;
using PopValidations.Execution.Stores;
using PopValidations.Scopes;
using PopValidations.Validations.Base;

namespace PopValidations.Scopes.Whens;

public sealed class WhenScopeToValidator<TValidationType, TPassThrough>
    : ScopeBase, IValidatorScope, IExpandableEntity
{
    private readonly string whenDescription;
    private readonly ScopedData<TValidationType, TPassThrough> scoped;
    private readonly Action<ScopedData<TValidationType, TPassThrough>> rules;

    public override string Name => whenDescription;
    WhenStringValidator_IfTruescoped<TValidationType, TPassThrough> something;

    public WhenScopeToValidator(
        ValidationConstructionStore validatorStore,
        string whenDescription,
        Func<TValidationType, Task<TPassThrough>> scoped,
        Func<TValidationType, TPassThrough, Task<bool>> ifTrue,
        Action<ScopedData<TValidationType, TPassThrough>> rules
    ) : base(validatorStore)
    {
        this.whenDescription = whenDescription;
        this.scoped = new ScopedData<TValidationType, TPassThrough>(scoped);
        this.rules = rules;
        something = new WhenStringValidator_IfTruescoped<TValidationType, TPassThrough>
            (
            ifTrue,
            this.scoped
            );
        Decorator = (item) => new WhenValidationItemDecorator_Scoped
        <TValidationType, TPassThrough>
        (
            item,
            something
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