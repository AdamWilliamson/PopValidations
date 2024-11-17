using ApiValidations.Descriptors;
using PopValidations.Execution.Validations.Base;
using PopValidations.Validations;

namespace ApiValidations;

public static class IsEnumValidationExtensions
{
    public static ParamDescriptor<TParamType, TValidationType> IsEnum<TParamType, TValidationType>(
        this ParamDescriptor<TParamType, TValidationType> fieldDescriptor,
        Type enumType
    )
    {
        var validation = new IsEnumValidation(typeof(TParamType), enumType);
        return fieldDescriptor.AddValidation(validation);
    }

    public static ParamDescriptor<TParamType, TValidationType> IsEnum<TParamType, TValidationType>(
        this ParamDescriptor<TParamType, TValidationType> fieldDescriptor,
        Type enumType,
        Action<ValidationOptions>? optionsAction
    )
    {
        var validation = new IsEnumValidation(typeof(TParamType), enumType);
        optionsAction?.Invoke(new ValidationOptions(validation));
        return fieldDescriptor.AddValidation(validation);
    }
}