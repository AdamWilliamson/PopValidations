using System.Threading.Tasks;
using PopValidations.Execution.Stores.Internal;
using PopValidations.FieldDescriptors.Base;
using PopValidations.Validations.Base;

namespace PopValidations.Scopes.Whens;

public class WhenValidationItemDecorator<TValidationType> 
    : WhenValidationItemDecoratorBase<TValidationType>
{
    private readonly WhenStringValidator_IfTrue<TValidationType> ifTrue;

    public WhenValidationItemDecorator(
        IValidatableStoreItem itemToDecorate,
        WhenStringValidator_IfTrue<TValidationType> ifTrue,
        IFieldDescriptorOutline? fieldDescriptor
    ) : base(itemToDecorate, fieldDescriptor)
    {
        this.ifTrue = ifTrue;
    }

    public override void ReHomeScopes(IFieldDescriptorOutline attemptedScopeFieldDescriptor)
    {
        base.ReHomeScopes(attemptedScopeFieldDescriptor);
    }

    public override async Task<bool> CanValidate(object? instance)
    {
        if (this.WrappingLevelfieldDescriptor == null && instance is not TValidationType)
        {
            throw new System.Exception("Type validating against, is incorrect and no FieldExecutor has been assigned");
        }

        bool result = false;

        if (instance is TValidationType)
        {
            result = await ifTrue.CanValidate((TValidationType)instance);
        }
        else if (WrappingLevelfieldDescriptor != null)
        {
            var fieldExecutorValue = WrappingLevelfieldDescriptor!.GetValue(/*parentScopeFieldDescriptor?.GetValue(instance) ??*/ instance);

            if (fieldExecutorValue != null)
            {
                result = await ifTrue.CanValidate((TValidationType)fieldExecutorValue);
            }
            else
            {
#pragma warning disable CS8604 // Possible null reference argument.
                result = await ifTrue.CanValidate(default);
#pragma warning restore CS8604 // Possible null reference argument.
            }
        }
        else
        {
            throw new System.Exception("Not a valid instance type, or not correct type");
        }

        if (result)
            return await ItemToDecorate.CanValidate(instance);
        return result;
    }
}