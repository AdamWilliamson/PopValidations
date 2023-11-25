using System.Threading.Tasks;
using PopValidations.Execution.Stores.Internal;
using PopValidations.FieldDescriptors.Base;
using PopValidations.Validations.Base;

namespace PopValidations.Scopes.Whens;

public class WhenValidationItemDecorator<TValidationType> 
    : WhenValidationItemDecoratorBase<TValidationType>
{
    private readonly IValidatorScope scopeParent;
    private readonly WhenStringValidator_IfTrue<TValidationType> ifTrue;
    private readonly IScopeData? scopedData;
    IFieldDescriptorOutline parentScopeFieldDescriptor;

    public WhenValidationItemDecorator(
        IValidatorScope scopeParent,
        IValidatableStoreItem itemToDecorate,
        WhenStringValidator_IfTrue<TValidationType> ifTrue,
        IScopeData? scopedData,
        IFieldDescriptorOutline? fieldDescriptor
    ) : base(itemToDecorate, fieldDescriptor)
    {
        this.scopeParent = scopeParent;
        this.ifTrue = ifTrue;
        this.scopedData = scopedData;
    }

    public override void ReHomeScopes(IFieldDescriptorOutline attemptedScopeFieldDescriptor)
    {
        base.ReHomeScopes(attemptedScopeFieldDescriptor);
    }

    public void SetParent(FieldExecutor fieldExecutor)
    {
        base.SetParent(fieldExecutor);
        parentScopeFieldDescriptor = fieldExecutor;
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
            var fieldExecutorValue = WrappingLevelfieldDescriptor!.GetValue(parentScopeFieldDescriptor?.GetValue(instance) ?? instance);

            if (fieldExecutorValue != null)
            {
                result = await ifTrue.CanValidate((TValidationType)fieldExecutorValue);
            }
            else
            {
                result = await ifTrue.CanValidate(default);
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