using System;
using System.Threading.Tasks;
using PopValidations.Execution.Stores;
using PopValidations.FieldDescriptors.Base;
using PopValidations.Validations.Base;

namespace PopValidations.Scopes.Whens;

public sealed class WhenScopedResultValidator<TValidationType, TPassThrough> : ScopeBase
{
    private readonly string whenDescription;
    private readonly ScopedData<TValidationType, TPassThrough> scoped;
    private readonly Action<IScopedData<TPassThrough>> rules;
    public override string Name => whenDescription;

    public WhenScopedResultValidator(
        string whenDescription,
        Func<TValidationType, bool> ifTrue,
        ScopedData<TValidationType, TPassThrough> scopedData,
        Action<IScopedData<TPassThrough>> rules
    ) : this(
        whenDescription,
        (x) => Task.FromResult(ifTrue.Invoke(x)),
        scopedData,
        rules
        )
    {}

    public WhenScopedResultValidator(
        string whenDescription,
        Func<TValidationType, Task<bool>> ifTrue,
        ScopedData<TValidationType, TPassThrough> scopedData,
        Action<IScopedData<TPassThrough>> rules
    )
    {
        this.whenDescription = whenDescription;
        this.scoped = scopedData;
        this.rules = rules;

        Decorator = (item, fieldDescriptor) => new WhenValidationItemDecorator<TValidationType>(
            item,
            new WhenStringValidator_IfTrue<TValidationType>(ifTrue),
            fieldDescriptor
        );
    }

    public override void ReHomeScopes(IFieldDescriptorOutline fieldDescriptorOutline)
    {
    }

    protected override void InvokeScopeContainer(ValidationConstructionStore store, object? value)
    {
        scoped.Init(value).Wait();
        rules.Invoke(scoped);
    }

    protected override void InvokeScopeContainerToDescribe(ValidationConstructionStore store)
    {
        rules.Invoke(scoped);
    }

    public override void ChangeStore(IValidationStore store) { }
}