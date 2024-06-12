using ApiValidations.Descriptors;
using PopValidations.Execution.Validations.Base;
using PopValidations.Validations;

namespace ApiValidations;

public static class IsEnumReturnValidationExtensions
{
    public static IReturnDescriptor<TReturnType> IsEnum<TReturnType>(
        this IReturnDescriptor<TReturnType> fieldDescriptor,
        Type enumType,
        Action<ValidationOptions>? optionsAction = null
    )
    {
        var validation = new IsEnumValidation<TReturnType>(enumType);
        optionsAction?.Invoke(new ValidationOptions(validation));
        fieldDescriptor.AddValidation(validation);
        return fieldDescriptor;
    }
}