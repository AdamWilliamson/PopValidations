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
        string whenDescription,
        ScopedData<TValidationType, TPassThrough> scoped,
        Func<TValidationType, TPassThrough, bool> ifTrue,
        Action<ScopedData<TValidationType, TPassThrough>> rules
    ) : this(
            whenDescription,
            scoped,
            (x, y) => Task.FromResult(ifTrue.Invoke(x,y)),
            rules
        )
    {}

    public WhenScopeToValidator(
        string whenDescription,
        ScopedData<TValidationType, TPassThrough> scoped,
        Func<TValidationType, TPassThrough, Task<bool>> ifTrue,
        Action<ScopedData<TValidationType, TPassThrough>> rules
    )
    {
        this.whenDescription = whenDescription;
        this.scoped = scoped;
        this.rules = rules;
        something = new WhenStringValidator_IfTruescoped<TValidationType, TPassThrough>
            (
            ifTrue,
            this.scoped
            );
        Decorator = (item, fieldDescriptor) => new WhenValidationItemDecorator_Scoped
        <TValidationType, TPassThrough>
        (
            item,
            something,
            fieldDescriptor
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

    public override void ChangeStore(IValidationStore store) { }
}