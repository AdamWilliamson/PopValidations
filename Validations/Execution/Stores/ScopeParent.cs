using PopValidations.Scopes;

namespace PopValidations.Execution.Stores;

public class ScopeParent
{
    public ScopeParent? PreviousScope { get; }
    public IParentScope? CurrentScope { get; }

    public ScopeParent(IParentScope? currentScope, ScopeParent? PreviousScope)
    {
        CurrentScope = currentScope;
        this.PreviousScope = PreviousScope;
    }
}
