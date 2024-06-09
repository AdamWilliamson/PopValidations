using ApiValidations.Descriptors;
using PopValidations.Validations;
using PopValidations.Validations.Base;

namespace PopValidations;

public static class IsLengthExclusivelyBetweenValidationExtensions
{
    public static ParamDescriptor<TParamType, TValidationType> IsLengthExclusivelyBetween<
        TParamType, TValidationType
    >(
        this ParamDescriptor<TParamType, TValidationType> fieldDescriptor,
        IScopedData<int?> start,
        IScopedData<int?> end
    )
    {
        var validation = new IsLengthExclusivelyBetweenValidation<TParamType>(
            start,
            end
        );
        return fieldDescriptor.AddValidation(validation);
    }

    public static ParamDescriptor<TParamType, TValidationType> IsLengthExclusivelyBetween<
       TParamType, TValidationType
   >(
       this ParamDescriptor<TParamType, TValidationType> fieldDescriptor,
       int start,
       int end
   )
    {
        var startScopedData = new ScopedData<int?>(start);
        var endScopedData = new ScopedData<int?>(end);

        return IsLengthExclusivelyBetween(
            fieldDescriptor,
            startScopedData,
            endScopedData,
            null
        );
    }

    public static ParamDescriptor<TParamType, TValidationType> IsLengthExclusivelyBetween<
        TParamType, TValidationType
    >(
        this ParamDescriptor<TParamType, TValidationType> fieldDescriptor,
        IScopedData<int?> start,
        IScopedData<int?> end,
        Action<BetweenValidationOptions<TParamType>>? optionsAction
    )
    {
        var validation = new IsLengthExclusivelyBetweenValidation<TParamType>(
            start,
            end
        );
        optionsAction?.Invoke(
            new BetweenValidationOptions<TParamType>(validation)
        );
        return fieldDescriptor.AddValidation(validation);
    }

    public static ParamDescriptor<TParamType, TValidationType> IsLengthExclusivelyBetween<
       TParamType, TValidationType
   >(
       this ParamDescriptor<TParamType, TValidationType> fieldDescriptor,
       int start,
       int end,
       Action<BetweenValidationOptions<TParamType>>? optionsAction
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
