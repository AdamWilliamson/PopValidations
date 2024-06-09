using ApiValidations.Descriptors;
using PopValidations.Execution.Validations.Base;
using PopValidations.Validations;

namespace PopValidations;

public static class IsNullValidationExtensions
{
    public static ParamDescriptor<TParamType, TValidationType> IsNull<TParamType, TValidationType>(
        this ParamDescriptor<TParamType, TValidationType> fieldDescriptor
        )
    {
        var validation = new IsNullValidation();
        return fieldDescriptor.AddValidation(validation);
    }

    public static ParamDescriptor<TParamType, TValidationType> IsNull<TParamType, TValidationType>(
        this ParamDescriptor<TParamType, TValidationType> fieldDescriptor,
        Action<ValidationOptions>? optionsAction
        )
    {
        var validation = new IsNullValidation();
        optionsAction?.Invoke(new ValidationOptions(validation));
        return fieldDescriptor.AddValidation(validation);
    }
}
