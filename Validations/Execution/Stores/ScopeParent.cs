using PopValidations.Scopes;
using System.Diagnostics;

namespace PopValidations.Execution.Stores;

public class ScopeParent
{
    public ScopeParent? PreviousScope { get; }
    public IParentScope? CurrentScope { get; }

    public ScopeParent(IParentScope? currentScope, ScopeParent? PreviousScope)
    {
        CurrentScope = currentScope;
        this.PreviousScope = PreviousScope;

        PrintName();
    }

    private void PrintName()
    {
        if (CurrentScope != null)
            Debug.WriteLine("ScopeParent: " + CurrentScope?.Name);

        if (PreviousScope != null)
            PreviousScope?.PrintName();
    }
}
