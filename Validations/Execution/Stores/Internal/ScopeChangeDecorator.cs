using System;
using System.Threading.Tasks;
using PopValidations.FieldDescriptors.Base;
using PopValidations.Validations.Base;

namespace PopValidations.Execution.Stores.Internal;

public class ScopeChangeDecorator : IValidatableStoreItem
{
    private readonly IValidatableStoreItem previous;

    public ScopeChangeDecorator(
        IFieldDescriptorOutline scopeFieldDescriptor,
        IValidatableStoreItem previous)
    {
        if (previous == null) throw new ArgumentNullException(nameof(previous));

        ScopeFieldDescriptor = scopeFieldDescriptor;
        this.previous = previous;
    }

    public bool IsVital => previous.IsVital;

    public ScopeParent? ScopeParent
    {
        get => previous.ScopeParent;
        set => previous.ScopeParent = value;
    }

    public FieldExecutor? CurrentFieldExecutor
    {
        get { return previous.CurrentFieldExecutor; }
        set { previous.CurrentFieldExecutor = value; }
    }

    public IFieldDescriptorOutline? FieldDescriptor
    {
        get { return previous.FieldDescriptor; }
        set { previous.FieldDescriptor = value; }
    }

    public IValidationComponent Component => previous.Component;

    public IFieldDescriptorOutline ScopeFieldDescriptor { get; }

    ScopeParent? IStoreItem.ScopeParent
    {
        get { return previous.ScopeParent; }
        set { previous.ScopeParent = value; }
    }

    public Task<bool> CanValidate(object? instance)
    {
        var value = ScopeFieldDescriptor?.GetValue(instance) ?? instance;
        return previous.CanValidate(value);
    }

    public DescribeActionResult Describe()
    {
        return previous.Describe();
    }

    public IValidatableStoreItem? GetChild()
    {
        return previous.GetChild();
    }

    public object? GetValue(object? value)
    {
        return previous.GetValue(value);
    }

    public Task InitScopes(object? instance)
    {
        return previous.InitScopes(instance);
    }

    public void ReHomeScopes(IFieldDescriptorOutline attemptedScopeFieldDescriptor)
    {
        previous.ReHomeScopes(attemptedScopeFieldDescriptor);
    }

    public void SetParent(FieldExecutor fieldExecutor)
    {
        previous.SetParent(fieldExecutor);
    }

    public ValidationActionResult Validate(object? value)
    {
        return previous.Validate(value);
    }
}
