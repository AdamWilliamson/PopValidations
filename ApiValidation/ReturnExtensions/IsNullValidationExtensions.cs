using ApiValidations.Descriptors;
using PopValidations.Execution.Validations.Base;
using PopValidations.Validations;

namespace ApiValidations;

public static class IsNullReturnValidationExtensions
{
    public static IReturnDescriptor<TReturnType> IsNull<TReturnType>(
        this IReturnDescriptor<TReturnType> fieldDescriptor,
        Action<ValidationOptions>? optionsAction = null
        )
    {
        var validation = new IsNullValidation();
        optionsAction?.Invoke(new ValidationOptions(validation));
        fieldDescriptor.AddValidation(validation);
        return fieldDescriptor;
    }
}
