using ApiValidations.Descriptors;
using PopValidations.Execution.Validations.Base;
using PopValidations.Validations;
using PopValidations.Validations.Base;

namespace ApiValidations;

public static class IsLessThanOrEqualToValidationExtensions
{
    public static ParamDescriptor<TParamType, TValidationType> IsLessThanOrEqualTo<TParamType, TValidationType>(
        this ParamDescriptor<TParamType, TValidationType> fieldDescriptor,
        IComparable value
    )
    {
        var scopedData = new ScopedData<TValidationType, IComparable>(value);
        var validation = new IsLessThanOrEqualToValidation(scopedData);
        return fieldDescriptor.AddValidation(validation);
    }

    public static ParamDescriptor<TParamType, TValidationType> IsLessThanOrEqualTo<TParamType, TValidationType, TPassThrough>(
        this ParamDescriptor<TParamType, TValidationType> fieldDescriptor,
        IScopedData<TPassThrough?> scopedData
    )
        where TPassThrough : IComparable
    {
        var validation = new IsLessThanOrEqualToValidation(scopedData);
        return fieldDescriptor.AddValidation(validation);
    }

    public static ParamDescriptor<TParamType, TValidationType> IsLessThanOrEqualTo<TParamType, TValidationType>(
        this ParamDescriptor<TParamType, TValidationType> fieldDescriptor,
        IComparable value,
        Action<ValidationOptions>? optionsAction
    )
    {
        var scopedData = new ScopedData<TValidationType, IComparable>(value);
        var validation = new IsLessThanOrEqualToValidation(scopedData);
        optionsAction?.Invoke(new ValidationOptions(validation));
        return fieldDescriptor.AddValidation(validation);
    }

    public static ParamDescriptor<TParamType, TValidationType> IsLessThanOrEqualTo<TParamType, TValidationType, TPassThrough>(
        this ParamDescriptor<TParamType, TValidationType> fieldDescriptor,
        IScopedData<TPassThrough?> scopedData,
        Action<ValidationOptions>? optionsAction
    )
        where TPassThrough : IComparable
    {
        var validation = new IsLessThanOrEqualToValidation(scopedData);
        optionsAction?.Invoke(new ValidationOptions(validation));
        return fieldDescriptor.AddValidation(validation);
    }
}