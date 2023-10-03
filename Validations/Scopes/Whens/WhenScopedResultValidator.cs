using System;
using System.Threading.Tasks;
using PopValidations.Execution.Stores;
using PopValidations.FieldDescriptors.Base;
using PopValidations.Validations.Base;

namespace PopValidations.Scopes.Whens;

public sealed class WhenScopedResultValidator<TValidationType, TPassThrough> : ScopeBase
{
    private readonly string whenDescription;
    //private readonly Func<TValidationType, Task<bool>> ifTrue;
    private readonly ScopedData<TValidationType, TPassThrough> scoped;
    private readonly Action<IScopedData<TPassThrough>> rules;
    public override string Name => whenDescription;
    //WhenStringValidator_IfTrue<TValidationType> something;

    public WhenScopedResultValidator(
        //IValidationStore validatorStore,
        string whenDescription,
        Func<TValidationType, bool> ifTrue,
        ScopedData<TValidationType, TPassThrough> scopedData,
        Action<IScopedData<TPassThrough>> rules
    ) : this(
        //validatorStore,
        whenDescription,
        (x) => Task.FromResult(ifTrue.Invoke(x)),
        scopedData,
        rules
        )
    {}

    public WhenScopedResultValidator(
        //IValidationStore validatorStore,
        string whenDescription,
        Func<TValidationType, Task<bool>> ifTrue,
        ScopedData<TValidationType, TPassThrough> scopedData,
        //Func<TValidationType, Task<TPassThrough>> scoped,
        Action<IScopedData<TPassThrough>> rules
    )// : base(validatorStore)
    {
        this.whenDescription = whenDescription;
        //this.ifTrue = ifTrue;
        this.scoped = scopedData;// new ScopedData<TValidationType, TPassThrough>(scoped);
        this.rules = rules;

        Decorator = (item, fieldDescriptor) => new WhenValidationItemDecorator<TValidationType>(
            this,
            item,
            new WhenStringValidator_IfTrue<TValidationType>(ifTrue),
            this.scoped,
            fieldDescriptor
        );
    }

    public override void ReHomeScopes(IFieldDescriptorOutline fieldDescriptorOutline)
    {
        //scoped.ReHome(fieldDescriptorOutline);
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