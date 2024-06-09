using ApiValidations.Descriptors;
using PopValidations.Validations;
using PopValidations.Validations.Base;

namespace PopValidations;

public static class IsLengthInclusivelyBetweenValidationExtensions
{
    public static ParamDescriptor<TParamType, TValidationType> IsLengthInclusivelyBetween<
        TParamType, TValidationType
    >(
        this ParamDescriptor<TParamType, TValidationType> fieldDescriptor,
        int start,
        int end
    )
    {
        var startScopedData = new ScopedData<int?>(start);
        var endScopedData = new ScopedData<int?>(end);

        return IsLengthInclusivelyBetween(
            fieldDescriptor,
            startScopedData,
            endScopedData,
            null
        );
    }

    public static ParamDescriptor<TParamType, TValidationType> IsLengthInclusivelyBetween<
        TParamType, TValidationType
    >(
        this ParamDescriptor<TParamType, TValidationType> fieldDescriptor,
        IScopedData<int?> start,
        IScopedData<int?> end
    )
    {
        var validation = new IsLengthInclusivelyBetweenValidation<TParamType>(
            start,
            end
        );
        return fieldDescriptor.AddValidation(validation);
    }

    public static ParamDescriptor<TParamType, TValidationType> IsLengthInclusivelyBetween<
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

        return IsLengthInclusivelyBetween(
            fieldDescriptor,
            startScopedData,
            endScopedData,
            optionsAction
        );
    }

    public static ParamDescriptor<TParamType, TValidationType> IsLengthInclusivelyBetween<
        TParamType, TValidationType
    >(
        this ParamDescriptor<TParamType, TValidationType> fieldDescriptor,
        IScopedData<int?> start,
        IScopedData<int?> end,
        Action<BetweenValidationOptions<TParamType>>? optionsAction
    )
    {
        var validation = new IsLengthInclusivelyBetweenValidation<TParamType>(
            start,
            end
        );
        optionsAction?.Invoke(
            new BetweenValidationOptions<TParamType>(validation)
        );
        return fieldDescriptor.AddValidation(validation);
    }
}
