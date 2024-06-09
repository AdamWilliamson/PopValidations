using ApiValidations.Descriptors;
using PopValidations.Execution.Validations.Base;
using PopValidations.Validations;

namespace PopValidations;

public static class IsNotNullReturnValidationExtensions
{
    public static IReturnDescriptor<TReturnType> IsNotNull<TReturnType>(
        this IReturnDescriptor<TReturnType> fieldDescriptor,
        Action<ValidationOptions>? optionsAction = null
        )
    {
        var validation = new IsNotNullValidation();
        optionsAction?.Invoke(new ValidationOptions(validation));
        fieldDescriptor.AddValidation(validation);
        return fieldDescriptor;
    }
}
