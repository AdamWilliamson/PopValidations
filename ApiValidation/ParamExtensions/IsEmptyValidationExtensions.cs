using ApiValidations.Descriptors;
using PopValidations.Execution.Validations.Base;
using PopValidations.Validations;

namespace PopValidations;

public static class IsEmptyValidationExtensions
{
    public static ParamDescriptor<TParamType, TValidationType> IsEmpty<TParamType, TValidationType>(
        this ParamDescriptor<TParamType, TValidationType> fieldDescriptor
    )
    {
        var validation = new IsEmptyValidation();
        return fieldDescriptor.AddValidation(validation);
    }

    public static ParamDescriptor<TParamType, TValidationType> IsEmpty<TParamType, TValidationType>(
        this ParamDescriptor<TParamType, TValidationType> fieldDescriptor,
        Action<ValidationOptions>? optionsAction
    )
    {
        var validation = new IsEmptyValidation();
        optionsAction?.Invoke(new ValidationOptions(validation));
        return fieldDescriptor.AddValidation(validation);
    }
}
