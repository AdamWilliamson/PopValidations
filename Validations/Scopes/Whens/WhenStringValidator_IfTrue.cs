using System;
using System.Threading.Tasks;

namespace PopValidations.Scopes.Whens;

public class WhenStringValidator_IfTrue<TValidationType>
{
    Func<TValidationType, Task<bool>> ifTrue;
    bool? result = null;

    public WhenStringValidator_IfTrue(Func<TValidationType, Task<bool>> ifTrue)
    {
        this.ifTrue = ifTrue;
    }

    public async Task<bool> CanValidate(TValidationType input)
    {
        if (result != null) return result.Value;

        return await ifTrue.Invoke(input);
    }
}