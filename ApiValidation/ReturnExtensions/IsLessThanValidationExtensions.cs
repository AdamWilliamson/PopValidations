using ApiValidations.Descriptors;
using PopValidations.Execution.Validations.Base;
using PopValidations.Validations;
using PopValidations.Validations.Base;

namespace ApiValidations;

public static class IsLessThanReturnValidationExtensions
{
    public static IReturnDescriptor<TReturnType> IsLessThan<TReturnType>(
        this IReturnDescriptor<TReturnType> fieldDescriptor,
        IComparable value,
        Action<ValidationOptions>? optionsAction = null
    )
    {
        var scopedData = new ScopedData<TReturnType, IComparable>(value);
        var validation = new IsLessThanValidation(scopedData);
        optionsAction?.Invoke(new ValidationOptions(validation));
        fieldDescriptor.AddValidation(validation);
        return fieldDescriptor;
    }

    public static IReturnDescriptor<TReturnType> IsLessThan<TReturnType, TPassThrough>(
        this IReturnDescriptor<TReturnType> fieldDescriptor,
        IScopedData<TPassThrough?> scopedData,
        Action<ValidationOptions>? optionsAction = null
    )
        where TPassThrough : IComparable
    {
        var validation = new IsLessThanValidation(scopedData);
        optionsAction?.Invoke(new ValidationOptions(validation));
        fieldDescriptor.AddValidation(validation);
        return fieldDescriptor;
    }
}