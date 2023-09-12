using System;
using System.Threading.Tasks;
using PopValidations.Execution.Stores.Internal;
using PopValidations.FieldDescriptors.Base;
using PopValidations.Validations.Base;

namespace PopValidations.Execution.Stores;

public interface IDecorator : IValidatableStoreItem 
{
    IFieldDescriptorOutline DecoratorFieldDescriptor { get; set; }
}

public class Decorator: IDecorator 
{
    public Decorator(
        IValidatableStoreItem itemToDecorate
    )
    {
        ItemToDecorate = itemToDecorate;
    }

    public bool IsVital => ItemToDecorate.IsVital;

    public IFieldDescriptorOutline DecoratorFieldDescriptor { get; set; }

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

    public virtual Task<bool> CanValidate(object? instance)
    {
        return Task.FromResult(true);
    }

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

public interface IExpandableEntity
{
    Func<IValidatableStoreItem, IFieldDescriptorOutline?, IValidatableStoreItem>? Decorator { get; }
    void ReHomeScopes(IFieldDescriptorOutline fieldDescriptorOutline);
    void ExpandToValidate(ValidationConstructionStore store, object? value);
    void ExpandToDescribe(ValidationConstructionStore store);
    void AsVital();
    bool IgnoreScope { get; }
}
