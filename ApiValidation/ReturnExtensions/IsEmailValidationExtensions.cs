using ApiValidations.Descriptors;
using PopValidations.Execution.Validations.Base;
using PopValidations.Validations;

namespace ApiValidations;

public static class IsEmailReturnValidationExtensions
{
    public static IReturnDescriptor<TReturnType> IsEmail<TReturnType>(
        this IReturnDescriptor<TReturnType> fieldDescriptor,
        Action<ValidationOptions>? optionsAction = null
    )
    {
        var validation = new IsEmailValidation();
        optionsAction?.Invoke(new ValidationOptions(validation));
        fieldDescriptor.AddValidation(validation);
        return fieldDescriptor;
    }
}