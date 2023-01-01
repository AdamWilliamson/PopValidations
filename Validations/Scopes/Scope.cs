using System;
using PopValidations.Execution.Stores;
using PopValidations.Validations.Base;

namespace PopValidations.Scopes.Whens;

public sealed class Scope<TScopedDataType> : ScopeBase
{
    private readonly IScopedData<TScopedDataType?> scopedData;
    private readonly Action<IScopedData<TScopedDataType?>> rules;

    public override bool IgnoreScope => true;
    public override string Name => "";

    public Scope(
        ValidationConstructionStore validatorStore,
        IScopedData<TScopedDataType?> scopedData,
        Action<IScopedData<TScopedDataType?>> rules
    ) : base(validatorStore)
    {
        this.scopedData = scopedData;
        this.rules = rules;
    }

    protected override async void InvokeScopeContainer(ValidationConstructionStore store, object? value)
    {
        await scopedData.Init(value);
        rules.Invoke(scopedData);
    }

    protected override void InvokeScopeContainerToDescribe(ValidationConstructionStore store)
    {
        rules.Invoke(scopedData);
    }
}