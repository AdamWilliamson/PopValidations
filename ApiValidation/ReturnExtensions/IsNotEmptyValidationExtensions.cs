using ApiValidations.Descriptors;
using PopValidations.Execution.Validations.Base;
using PopValidations.Validations;

namespace ApiValidations;

public static class IsNotEmptyReturnValidationExtensions
{
    public static IReturnDescriptor<TReturnType> IsNotEmpty<TReturnType>(
        this IReturnDescriptor<TReturnType> fieldDescriptor,
        Action<ValidationOptions>? optionsAction = null
    )
    {
        var validation = new IsNotEmptyValidation();
        optionsAction?.Invoke(new ValidationOptions(validation));
        fieldDescriptor.AddValidation(validation);
        return fieldDescriptor;
    }
}
