using ApiValidations.Descriptors;
using PopValidations.Execution.Validations.Base;
using PopValidations.Validations;

namespace ApiValidations;

public static class IsNotEmptyValidationExtensions
{
    public static ParamDescriptor<TParamType, TValidationType> IsNotEmpty<TParamType, TValidationType>(
        this ParamDescriptor<TParamType, TValidationType> fieldDescriptor
    )
    {
        var validation = new IsNotEmptyValidation();
        return fieldDescriptor.AddValidation(validation);
    }

    public static ParamDescriptor<TParamType, TValidationType> IsNotEmpty<TParamType, TValidationType>(
        this ParamDescriptor<TParamType, TValidationType> fieldDescriptor,
        Action<ValidationOptions>? optionsAction
    )
    {
        var validation = new IsNotEmptyValidation();
        optionsAction?.Invoke(new ValidationOptions(validation));
        return fieldDescriptor.AddValidation(validation);
    }
}
