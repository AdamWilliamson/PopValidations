using ApiValidations.Descriptors;
using PopValidations.Execution.Validations.Base;
using PopValidations.Validations;

namespace PopValidations;

public static class IsEmptyReturnValidationExtensions
{
    public static IReturnDescriptor<TReturnType> IsEmpty<TReturnType>(
    this IReturnDescriptor<TReturnType> fieldDescriptor,
    Action<ValidationOptions>? optionsAction = null
    )
    {
        var validation = new IsEmptyValidation();
        optionsAction?.Invoke(new ValidationOptions(validation));
        fieldDescriptor.AddValidation(validation);
        return fieldDescriptor;
    }
}
