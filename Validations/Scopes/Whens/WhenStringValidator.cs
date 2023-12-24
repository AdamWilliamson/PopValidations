using System;
using System.Threading.Tasks;
using PopValidations.Execution.Stores;

namespace PopValidations.Scopes.Whens;

public sealed class WhenStringValidator<TValidationType> : ScopeBase
{
    private readonly string whenDescription;
    private readonly Action rules;
    public override string Name => whenDescription;

    public WhenStringValidator(
        string whenDescription,
        Func<TValidationType, Task<bool>> ifTrue,
        Action rules
    )
    {
        this.whenDescription = whenDescription;
        this.rules = rules;
        Decorator = (item, fieldDescriptor) => new WhenValidationItemDecorator<TValidationType>(
            item,
            new WhenStringValidator_IfTrue<TValidationType>(ifTrue),
            fieldDescriptor
        );
    }

    protected override void InvokeScopeContainer(ValidationConstructionStore store, object? value)
    {
        rules.Invoke();
    }

    protected override void InvokeScopeContainerToDescribe(ValidationConstructionStore store)
    {
        rules.Invoke();
    }

    public override void ChangeStore(IValidationStore store) { }
}