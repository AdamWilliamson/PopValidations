using ApiValidations.Descriptors;
using PopValidations.Execution.Validations.Base;
using PopValidations.Validations;
using PopValidations.Validations.Base;

namespace PopValidations;

public static class IsCustomReturnValidationExtensions
{
    public static IReturnDescriptor<TReturnType> Is<TReturnType>(
        this IReturnDescriptor<TReturnType> fieldDescriptor,
        string descriptionTemplate,
        string errorTemplate,
        Func<TReturnType, bool> validationFunc,
        Action<ValidationOptions>? optionsAction = null
    )
    {
        var validation = new IsCustomValidation<TReturnType>(
            descriptionTemplate,
            errorTemplate,
            validationFunc
        );
        optionsAction?.Invoke(new ValidationOptions(validation));
        fieldDescriptor.AddValidation(validation);
        return fieldDescriptor;
    }

    public static IReturnDescriptor<TReturnType> Is<TReturnType>(
        this IReturnDescriptor<TReturnType> fieldDescriptor,
        string descriptionTemplate,
        string errorTemplate,
        IScopedData<TReturnType, bool> scopedValue,
        Action<ValidationOptions>? optionsAction = null
    )
    {
        var validation = new IsCustomValidation<TReturnType>(
            descriptionTemplate,
            errorTemplate,
            scopedValue
        );
        optionsAction?.Invoke(new ValidationOptions(validation));
        fieldDescriptor.AddValidation(validation);
        return fieldDescriptor;
    }

    public static ParamDescriptor<TParamType, TValidationType> Is<TParamType, TValidationType>(
         this ParamDescriptor<TParamType, TValidationType> fieldDescriptor,
         string errorTemplate,
         string descriptionTemplate,
         Func<TParamType, IScopedData<TParamType, bool>> scopedResult,
         Action<ValidationOptions>? optionsAction = null
    )
    {
        var validation = new IsCustomScopedValidation<TParamType>(errorTemplate, descriptionTemplate, scopedResult);
        optionsAction?.Invoke(new ValidationOptions(validation));
        fieldDescriptor.AddValidation(validation);
        return fieldDescriptor;
    }
}
