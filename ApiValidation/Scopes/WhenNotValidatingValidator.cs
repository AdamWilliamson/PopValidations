using PopValidations.Execution.Stores;
using PopValidations.Scopes;
using PopValidations.Scopes.Whens;
//using System.Xml;

namespace ApiValidations.Scopes;

public sealed class WhenNotValidatingValidator<TValidationType> : ScopeBase
{
    private readonly Action rules;
    public override string Name => string.Empty;
    public override bool IgnoreScope => true;

    public WhenNotValidatingValidator(
        Action rules
    )
    {
        this.rules = rules;
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
        rules.Invoke();
    }

    public override void ChangeStore(IValidationStore store) { }
}
