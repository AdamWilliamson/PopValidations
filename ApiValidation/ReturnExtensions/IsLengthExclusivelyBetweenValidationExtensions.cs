using ApiValidations.Descriptors;
using PopValidations.Validations;
using PopValidations.Validations.Base;

namespace ApiValidations;

public static class IsLengthExclusivelyBetweenReturnValidationExtensions
{
    public static IReturnDescriptor<TReturnType> IsLengthExclusivelyBetween<
        TReturnType
    >(
        this IReturnDescriptor<TReturnType> fieldDescriptor,
        IScopedData<int?> start,
        IScopedData<int?> end,
        Action<BetweenValidationOptions<TReturnType>>? optionsAction = null
    )
    {
        var validation = new IsLengthExclusivelyBetweenValidation<TReturnType>(
            start,
            end
        );
        optionsAction?.Invoke(
            new BetweenValidationOptions<TReturnType>(validation)
        );
        fieldDescriptor.AddValidation(validation);
        return fieldDescriptor;
    }

    public static IReturnDescriptor<TReturnType> IsLengthExclusivelyBetween<
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

        return IsLengthExclusivelyBetween(
            fieldDescriptor,
            startScopedData,
            endScopedData,
            optionsAction
        );
    }
}
