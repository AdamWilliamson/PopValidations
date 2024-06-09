using ApiValidations.Descriptors;
using PopValidations.Execution.Validations.Base;
using PopValidations.Validations;
using PopValidations.Validations.Base;

namespace PopValidations;

public static class IsGreaterThanReturnValidationExtensions
{
    public static IReturnDescriptor<TReturnType> IsGreaterThan<TReturnType>(
        this IReturnDescriptor<TReturnType> fieldDescriptor,
        IComparable value,
        Action<ValidationOptions>? optionsAction = null
    )
    {
        var scopedData = new ScopedData<TReturnType, IComparable>(value);
        var validation = new IsGreaterThanValidation(scopedData);
        optionsAction?.Invoke(new ValidationOptions(validation));
        fieldDescriptor.AddValidation(validation);
        return fieldDescriptor;
    }
    
    public static IReturnDescriptor<TReturnType> IsGreaterThan<TReturnType, TPassThrough>(
        this IReturnDescriptor<TReturnType> fieldDescriptor,
        IScopedData<TPassThrough?> scopedData,
        Action<ValidationOptions>? optionsAction = null
    )
        where TPassThrough : IComparable
    {
        var validation = new IsGreaterThanValidation(scopedData);
        optionsAction?.Invoke(new ValidationOptions(validation));
        fieldDescriptor.AddValidation(validation);
        return fieldDescriptor;
    }
}