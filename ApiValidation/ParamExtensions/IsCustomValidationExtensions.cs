using ApiValidations.Descriptors;
using PopValidations.Execution.Validations.Base;
using PopValidations.Validations;
using PopValidations.Validations.Base;

namespace ApiValidations;

public static class IsCustomValidationExtensions
{
    public static ParamDescriptor<TParamType, TValidationType> Is<TParamType, TValidationType>(
        this ParamDescriptor<TParamType, TValidationType> fieldDescriptor,
        string descriptionTemplate,
        string errorTemplate,
        Func<TParamType, bool> validationFunc
    )
    {
        var validation = new IsCustomValidation<TParamType>(
            descriptionTemplate,
            errorTemplate,
            validationFunc
        );
        return fieldDescriptor.AddValidation(validation);
    }

    public static ParamDescriptor<TParamType, TValidationType> Is<TParamType, TValidationType>(
        this ParamDescriptor<TParamType, TValidationType> fieldDescriptor,
        string descriptionTemplate,
        string errorTemplate,
        IScopedData<TParamType, bool> scopedValue
    )
    {
        var validation = new IsCustomValidation<TParamType>(
            descriptionTemplate,
            errorTemplate,
            scopedValue
        );
        return fieldDescriptor.AddValidation(validation);
    }

    public static ParamDescriptor<TParamType, TValidationType> Is<TParamType, TValidationType>(
         this ParamDescriptor<TParamType, TValidationType> fieldDescriptor,
         string errorTemplate,
         string descriptionTemplate,
         Func<TParamType, IScopedData<TParamType, bool>> scopedResult
    )
    {
        var validation = new IsCustomScopedValidation<TParamType>(errorTemplate, descriptionTemplate, scopedResult);
        return fieldDescriptor.AddValidation(validation);
    }

    public static ParamDescriptor<TParamType, TValidationType> Is<TParamType, TValidationType>(
        this ParamDescriptor<TParamType, TValidationType> fieldDescriptor,
        string descriptionTemplate,
        string errorTemplate,
        Func<TParamType, bool> validationFunc,
        Action<ValidationOptions> optionsAction
    )
    {
        var validation = new IsCustomValidation<TParamType>(
            descriptionTemplate,
            errorTemplate,
            validationFunc
        );
        optionsAction?.Invoke(new ValidationOptions(validation));
        return fieldDescriptor.AddValidation(validation);
    }

    public static ParamDescriptor<TParamType, TValidationType> Is<TParamType, TValidationType>(
        this ParamDescriptor<TParamType, TValidationType> fieldDescriptor,
        string descriptionTemplate,
        string errorTemplate,
        IScopedData<TParamType, bool> scopedValue,
        Action<ValidationOptions>? optionsAction
    )
    {
        var validation = new IsCustomValidation<TParamType>(
            descriptionTemplate,
            errorTemplate,
            scopedValue
        );
        optionsAction?.Invoke(new ValidationOptions(validation));
        return fieldDescriptor.AddValidation(validation);
    }

    public static ParamDescriptor<TParamType, TValidationType> Is<TParamType, TValidationType>(
         this ParamDescriptor<TParamType, TValidationType> fieldDescriptor,
         string errorTemplate,
         string descriptionTemplate,
         Func<TParamType, IScopedData<TParamType, bool>> scopedResult,
         Action<ValidationOptions>? optionsAction
    )
    {
        var validation = new IsCustomScopedValidation<TParamType>(errorTemplate, descriptionTemplate, scopedResult);
        optionsAction?.Invoke(new ValidationOptions(validation));
        return fieldDescriptor.AddValidation(validation);
    }
}
