using System;
using System.Threading.Tasks;
using PopValidations.Validations.Base;

namespace PopValidations.Scopes.Whens;

public class WhenStringValidator_IfTruescoped<TValidationType, TScopedData>
{
    Func<TValidationType, TScopedData, Task<bool>> ifTrue;
    private readonly ScopedData<TValidationType, TScopedData> scopedData;
    bool? result = null;

    public WhenStringValidator_IfTruescoped(
        Func<TValidationType, TScopedData, Task<bool>> ifTrue,
        ScopedData<TValidationType, TScopedData> scopedData
        )
    {
        this.ifTrue = ifTrue;
        this.scopedData = scopedData;
    }

    public async Task<bool> CanValidate(TValidationType input)
    {
        if (result != null) return result.Value;
        await scopedData.Init(input);
        var data = scopedData.GetValue();
        if (data is not TScopedData converted) return false;

        return await ifTrue.Invoke(input, converted);
    }
}