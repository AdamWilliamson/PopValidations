using ApiValidations.Descriptors;
using PopValidations.Execution.Validations.Base;
using PopValidations.Validations;

namespace PopValidations;

public static class IsNotNullValidationExtensions
{
    public static ParamDescriptor<TParamType, TValidationType> IsNotNull<TParamType, TValidationType>(
        this ParamDescriptor<TParamType, TValidationType> fieldDescriptor
        )
    {
        var validation = new IsNotNullValidation();
        return fieldDescriptor.AddValidation(validation);
    }

    public static ParamDescriptor<TParamType, TValidationType> IsNotNull<TParamType, TValidationType>(
        this ParamDescriptor<TParamType, TValidationType> fieldDescriptor,
        Action<ValidationOptions>? optionsAction
        )
    {
        var validation = new IsNotNullValidation();
        optionsAction?.Invoke(new ValidationOptions(validation));
        return fieldDescriptor.AddValidation(validation);
    }
}
