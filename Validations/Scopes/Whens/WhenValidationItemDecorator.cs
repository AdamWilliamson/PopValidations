using System.Threading.Tasks;
using PopValidations.Execution.Stores.Internal;
using PopValidations.Validations.Base;

namespace PopValidations.Scopes.Whens;

public class WhenValidationItemDecorator<TValidationType> 
    : WhenValidationItemDecoratorBase<TValidationType>
{
    private readonly WhenStringValidator_IfTrue<TValidationType> ifTrue;
    private readonly IScopeData? scopedData;

    public WhenValidationItemDecorator(
        IValidatableStoreItem itemToDecorate,
        WhenStringValidator_IfTrue<TValidationType> ifTrue,
        IScopeData? scopedData
    ) : base(itemToDecorate)
    {
        this.ifTrue = ifTrue;
        this.scopedData = scopedData;
    }

    public override async Task<bool> CanValidate(object? instance)
    {
        if (instance is not TValidationType)
        {
            throw new System.Exception();
        }

        var result = await ifTrue.CanValidate((TValidationType)instance);
        scopedData?.Init(instance);

        if (result)
            return await ItemToDecorate.CanValidate(instance);
        return result;
    }
}