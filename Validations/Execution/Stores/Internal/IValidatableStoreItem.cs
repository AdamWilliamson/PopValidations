using System.Threading.Tasks;
using PopValidations.FieldDescriptors.Base;
using PopValidations.Validations.Base;

namespace PopValidations.Execution.Stores.Internal;

public interface IValidatableStoreItem : IStoreItem
{
    bool IsVital { get; }
    IValidatableStoreItem? GetChild();
    FieldExecutor? CurrentFieldExecutor { get; set; }
    void SetParent(FieldExecutor fieldExecutor);
    IValidationComponent Component { get; }
    object? GetValue(object? value);

    Task<bool> CanValidate(object? instance);
    ValidationActionResult Validate(object? value);
    Task InitScopes(object? instance);
    DescribeActionResult Describe();
    void ReHomeScopes(IFieldDescriptorOutline attemptedScopeFieldDescriptor);
}
