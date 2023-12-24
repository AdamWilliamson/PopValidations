using System.Threading.Tasks;
using PopValidations.Execution.Stores.Internal;
using PopValidations.FieldDescriptors.Base;

namespace PopValidations.Scopes.Whens;

public class WhenValidationItemDecorator_Scoped<TValidationType, TPassThrough> : WhenValidationItemDecoratorBase<TValidationType>
{
    private readonly WhenStringValidator_IfTruescoped<TValidationType, TPassThrough> ifTrue;

    public WhenValidationItemDecorator_Scoped(
        IValidatableStoreItem itemToDecorate,
        WhenStringValidator_IfTruescoped<TValidationType, TPassThrough> ifTrue,
        IFieldDescriptorOutline? fieldDescriptor
    ) : base(itemToDecorate, fieldDescriptor)
    {
        this.ifTrue = ifTrue;
    }

    public override Task<bool> CanValidate(object? instance)
    {
        if (instance is TValidationType converted)
        {
            return ifTrue.CanValidate(converted);
        }

        if (this.WrappingLevelfieldDescriptor != null)
        {
            var fieldExecutorValue = WrappingLevelfieldDescriptor!.GetValue(instance);

            if (fieldExecutorValue != null)
            {
                return ifTrue.CanValidate((TValidationType)fieldExecutorValue);
            }
            else
            {
#pragma warning disable CS8604 // Possible null reference argument.
                return ifTrue.CanValidate(default);
#pragma warning restore CS8604 // Possible null reference argument.
            }
        }

        throw new System.Exception("When failure due to invalid parameter.");
    }
}