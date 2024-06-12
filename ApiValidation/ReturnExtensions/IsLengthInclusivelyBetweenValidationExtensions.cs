using ApiValidations.Descriptors;
using PopValidations.Validations;
using PopValidations.Validations.Base;

namespace ApiValidations;

public static class IsLengthInclusivelyBetweenReturnValidationExtensions
{
    public static IReturnDescriptor<TReturnType> IsLengthInclusivelyBetween<
        TReturnType
    >(
        this IReturnDescriptor<TReturnType> fieldDescriptor,
        int start,
        int end,
        Action<BetweenValidationOptions<TReturnType>>? optionsAction = null
    )
    {
        var startScopedData = new ScopedData<int?>(start);
        var endScopedData = new ScopedData<int?>(end);

        return IsLengthInclusivelyBetween(
            fieldDescriptor,
            startScopedData,
            endScopedData,
            optionsAction
        );
    }

    public static IReturnDescriptor<TReturnType> IsLengthInclusivelyBetween<
        TReturnType
    >(
        this IReturnDescriptor<TReturnType> fieldDescriptor,
        IScopedData<int?> start,
        IScopedData<int?> end,
        Action<BetweenValidationOptions<TReturnType>>? optionsAction = null
    )
    {
        var validation = new IsLengthInclusivelyBetweenValidation<TReturnType>(
            start,
            end
        );
        optionsAction?.Invoke(
            new BetweenValidationOptions<TReturnType>(validation)
        );
        fieldDescriptor.AddValidation(validation);
        return fieldDescriptor;
    }
}
