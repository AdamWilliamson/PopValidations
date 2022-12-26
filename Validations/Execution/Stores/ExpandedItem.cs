using System;
using System.Threading.Tasks;
using PopValidations.Execution.Stores.Internal;
using PopValidations.Validations.Base;

namespace PopValidations.Execution.Stores;

public class ExpandedItem
{
    private readonly IValidatableStoreItem validatableStoreItem;

    public ExpandedItem(IValidatableStoreItem validatableStoreItem)
    {
        this.validatableStoreItem = validatableStoreItem;
    }

    public ScopeParent? ScopeParent => validatableStoreItem.ScopeParent;

    public string PropertyName => validatableStoreItem.FieldDescriptor?.PropertyName ?? throw new Exception();

    public string FullAddressableName
    {
        get
        {
            return validatableStoreItem.CurrentFieldExecutor?.FieldDescriptor?.PropertyName
                ?? PropertyName;
        }
    }

    public Task<bool> CanValidate(object? instance)
    {
        return validatableStoreItem.CanValidate(instance);
    }

    public ValidationActionResult Validate(object? instance)
    {
        var data = validatableStoreItem.GetValue(instance);
        return validatableStoreItem.Validate(data);
    }

    public DescribeActionResult Describe()
    {
        return validatableStoreItem.Describe();
    }

    public bool IsVital => validatableStoreItem.IsVital;

}
