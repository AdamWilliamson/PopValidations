using System.Threading.Tasks;
using PopValidations.Execution.Stores.Internal;

namespace PopValidations.Scopes.Whens;

public class WhenValidationItemDecorator_Scoped<TValidationType, TPassThrough> : WhenValidationItemDecoratorBase<TValidationType>
{
    private readonly WhenStringValidator_IfTruescoped<TValidationType, TPassThrough> ifTrue;

    public WhenValidationItemDecorator_Scoped(
        IValidatableStoreItem itemToDecorate,
        WhenStringValidator_IfTruescoped<TValidationType, TPassThrough> ifTrue
    ) : base(itemToDecorate)
    {
        this.ifTrue = ifTrue;
    }

    public override Task<bool> CanValidate(object? instance)
    {
        if (instance is not TValidationType)
        {
            throw new System.Exception();
        }
        return ifTrue.CanValidate((TValidationType)instance);
    }
}