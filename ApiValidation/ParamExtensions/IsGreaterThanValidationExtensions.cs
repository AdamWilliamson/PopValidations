using ApiValidations.Descriptors;
using PopValidations.Execution.Validations.Base;
using PopValidations.Validations;
using PopValidations.Validations.Base;

namespace PopValidations;

public static class IsGreaterThanValidationExtensions
{
    public static ParamDescriptor<TParamType, TValidationType> IsGreaterThan<TParamType, TValidationType>(
        this ParamDescriptor<TParamType, TValidationType> fieldDescriptor,
        IComparable value
    )
    {
        var scopedData = new ScopedData<TValidationType, IComparable>(value);
        var validation = new IsGreaterThanValidation(scopedData);
        return fieldDescriptor.AddValidation(validation);
    }

    public static ParamDescriptor<TParamType, TValidationType> IsGreaterThan<TParamType, TValidationType, TPassThrough>(
        this ParamDescriptor<TParamType, TValidationType> fieldDescriptor,
        IScopedData<TPassThrough?> scopedData
    )
        where TPassThrough : IComparable
    {
        var validation = new IsGreaterThanValidation(scopedData);
        return fieldDescriptor.AddValidation(validation);
    }

    public static ParamDescriptor<TParamType, TValidationType> IsGreaterThan<TParamType, TValidationType>(
        this ParamDescriptor<TParamType, TValidationType> fieldDescriptor,
        IComparable value,
        Action<ValidationOptions>? optionsAction = null
    )
    {
        var scopedData = new ScopedData<TValidationType, IComparable>(value);
        var validation = new IsGreaterThanValidation(scopedData);
        optionsAction?.Invoke(new ValidationOptions(validation));
        return fieldDescriptor.AddValidation(validation);
    }
    
    public static ParamDescriptor<TParamType, TValidationType> IsGreaterThan<TParamType, TValidationType, TPassThrough>(
        this ParamDescriptor<TParamType, TValidationType> fieldDescriptor,
        IScopedData<TPassThrough?> scopedData,
        Action<ValidationOptions>? optionsAction = null
    )
        where TPassThrough : IComparable
    {
        var validation = new IsGreaterThanValidation(scopedData);
        optionsAction?.Invoke(new ValidationOptions(validation));
        return fieldDescriptor.AddValidation(validation);
    }
}