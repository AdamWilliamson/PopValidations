using PopValidations.Execution.Stores;
using PopValidations.Scopes;

namespace ApiValidations.Scopes;

public class ApiScope<TScopedDataType> : ScopeBase
{
    public override string Name => throw new NotImplementedException();

    public override void ChangeStore(IValidationStore store)
    {
        throw new NotImplementedException();
    }

    protected override void InvokeScopeContainer(ValidationConstructionStore store, object? value)
    {
        throw new NotImplementedException();
    }

    protected override void InvokeScopeContainerToDescribe(ValidationConstructionStore store)
    {
        throw new NotImplementedException();
    }
}
