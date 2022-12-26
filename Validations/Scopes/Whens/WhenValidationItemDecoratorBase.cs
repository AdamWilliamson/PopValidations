using System.Threading.Tasks;
using PopValidations.Execution.Stores;
using PopValidations.Execution.Stores.Internal;
using PopValidations.FieldDescriptors.Base;
using PopValidations.Validations.Base;

namespace PopValidations.Scopes.Whens;

public abstract class WhenValidationItemDecoratorBase<TValidationType> : IValidatableStoreItem
{
    public WhenValidationItemDecoratorBase(
        IValidatableStoreItem itemToDecorate
    )
    {
        ItemToDecorate = itemToDecorate;
    }

    public bool IsVital => ItemToDecorate.IsVital;

    public ScopeParent? ScopeParent
    {
        get { return ItemToDecorate.ScopeParent; }
        set { ItemToDecorate.ScopeParent = value; }
    }

    public FieldExecutor? CurrentFieldExecutor
    {
        get { return ItemToDecorate.CurrentFieldExecutor; }
        set { ItemToDecorate.CurrentFieldExecutor = value; }
    }

    public IFieldDescriptorOutline? FieldDescriptor
    {
        get { return ItemToDecorate.FieldDescriptor; }
        set { ItemToDecorate.FieldDescriptor = value; }
    }

    public IValidationComponent Component => ItemToDecorate.Component;

    public IValidatableStoreItem ItemToDecorate { get; }

    public virtual DescribeActionResult Describe()
    {
        return ItemToDecorate.Describe();
    }

    public IValidatableStoreItem? GetChild()
    {
        return ItemToDecorate.GetChild();
    }

    public object? GetValue(object? value)
    {
        return ItemToDecorate.GetValue(value);
    }

    public void SetParent(FieldExecutor fieldExecutor)
    {
        ItemToDecorate.SetParent(fieldExecutor);
    }

    public abstract Task<bool> CanValidate(object? instance);

    public virtual void ReHomeScopes(IFieldDescriptorOutline attemptedScopeFieldDescriptor)
    {
        ItemToDecorate.ReHomeScopes(attemptedScopeFieldDescriptor);
    }
    public Task InitScopes(object? instance)
    {
        return ItemToDecorate.InitScopes(instance);
    }
    public virtual ValidationActionResult Validate(object? value)
    {
        return ItemToDecorate.Validate(value);
    }
}