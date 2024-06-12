using ApiValidations.Descriptors;
using PopValidations.Execution.Validations.Base;
using PopValidations.Validations;

namespace ApiValidations;

public static class IsEmailValidationExtensions
{
    public static ParamDescriptor<TParamType, TValidationType> IsEmail<TParamType, TValidationType>(
        this ParamDescriptor<TParamType, TValidationType> fieldDescriptor
    )
    {
        var validation = new IsEmailValidation();
        return fieldDescriptor.AddValidation(validation);
    }

    public static ParamDescriptor<TParamType, TValidationType> IsEmail<TParamType, TValidationType>(
        this ParamDescriptor<TParamType, TValidationType> fieldDescriptor,
        Action<ValidationOptions>? optionsAction
    )
    {
        var validation = new IsEmailValidation();
        optionsAction?.Invoke(new ValidationOptions(validation));
        return fieldDescriptor.AddValidation(validation);
    }
}