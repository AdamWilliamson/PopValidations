using System.Threading.Tasks;
using PopValidations.FieldDescriptors.Base;

namespace PopValidations.Validations.Base;

public interface IValidationComponent
{
    string DescriptionTemplate { get; }
    string ErrorTemplate { get; }

    void SetDescriptionTemplate(string desc);
    void SetErrorTemplate(string message);

    Task InitScopes(object? instance);
    ValidationActionResult Validate(object? value);
    DescribeActionResult Describe();
    void ReHomeScopes(IFieldDescriptorOutline attemptedScopeFieldDescriptor);
}
