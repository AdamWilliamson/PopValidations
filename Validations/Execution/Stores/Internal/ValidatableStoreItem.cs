using System;
using System.Threading.Tasks;
using PopValidations.FieldDescriptors.Base;
using PopValidations.Validations.Base;

namespace PopValidations.Execution.Stores.Internal;

public class ValidatableStoreItem : IValidatableStoreItem
{
    public ValidatableStoreItem(
        bool isVital,
        FieldExecutor? currentFieldExecutor,
        IFieldDescriptorOutline fieldDescriptor,
        ScopeParent? scopeParent,
        IValidationComponent component
    )
    {
        if (component == null)
        {
            throw new Exception();
        }

        //if (currentFieldExecutor == null)
        //{
        //    throw new Exception();
        //}

        IsVital = isVital;
        CurrentFieldExecutor = currentFieldExecutor;
        FieldDescriptor = fieldDescriptor;
        ScopeParent = scopeParent;
        Component = component;
    }

    public bool IsVital { get; }
    public FieldExecutor? CurrentFieldExecutor { get; set; }
    public IFieldDescriptorOutline? FieldDescriptor { get; set; }
    public ScopeParent? ScopeParent { get; set; }

    public void SetParent(FieldExecutor fieldExecutor)
    {
        if (CurrentFieldExecutor == null)
        {
            if (FieldDescriptor == null) throw new Exception("ValidatableStoreItem should not have Null FieldDescriptor");
            CurrentFieldExecutor = new FieldExecutor(fieldExecutor, FieldDescriptor);
        }
        else
        {
            CurrentFieldExecutor.SetParent(fieldExecutor);
        }
    }

    public IValidatableStoreItem? ChildStoreItem { get; }
    public IValidationComponent Component { get; }

    public IValidatableStoreItem? GetChild()
    {
        return ChildStoreItem;
    }

    public object? GetValue(object? value)
    {
        if (CurrentFieldExecutor != null)
        {
            return FieldDescriptor?.GetValue(CurrentFieldExecutor.GetValue(value));
        }
        return FieldDescriptor?.GetValue(value);
    }

    public void ReHomeScopes(IFieldDescriptorOutline attemptedScopeFieldDescriptor)
    {
        if (ChildStoreItem != null)
        {
            ChildStoreItem.ReHomeScopes(attemptedScopeFieldDescriptor);
            return;
        }

        Component?.ReHomeScopes(attemptedScopeFieldDescriptor);
    }

    public Task InitScopes(object? instance)
    {

        if (ChildStoreItem != null)
        {
            return ChildStoreItem.InitScopes(instance);
        }

        return Task.FromResult(Component?.InitScopes(instance));
    }
    public Task<bool> CanValidate(object? instance)
    {

        return Task.FromResult(true);
    }

    public virtual ValidationActionResult Validate(object? value)
    {
        if (ChildStoreItem != null)
        {
            return ChildStoreItem.Validate(value);
        }

        return Component.Validate(value);
    }

    public virtual DescribeActionResult Describe()
    {
        if (ChildStoreItem != null)
        {
            return ChildStoreItem.Describe();
        }

        return Component.Describe();
    }
}
