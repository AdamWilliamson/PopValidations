using PopValidations.Execution.Validations.Base;
using PopValidations.FieldDescriptors.Base;
using PopValidations.Validations;
using PopValidations.Validations.Base;
using System;
using System.Threading.Tasks;

namespace PopValidations;

public static class IsCustomValidationExtensions
{
    public static IFieldDescriptor<TValidationType, TFieldType> Is<
        TValidationType,
        TFieldType
    >(
        this IFieldDescriptor<TValidationType, TFieldType> fieldDescriptor,
        string descriptionTemplate,
        string errorTemplate,
        Func<TFieldType?, bool> validationFunc,
        Action<ValidationOptions>? optionsAction = null
    )
    {
        var validation = new IsCustomValidation<TFieldType>(
            descriptionTemplate,
            errorTemplate,
            validationFunc
        );
        optionsAction?.Invoke(new ValidationOptions(validation));
        fieldDescriptor.AddValidation(validation);
        return fieldDescriptor;
    }

    public static IFieldDescriptor<TValidationType, TFieldType> Is<
        TValidationType,
        TFieldType
    >(
        this IFieldDescriptor<TValidationType, TFieldType> fieldDescriptor,
        string descriptionTemplate,
        string errorTemplate,
        IScopedData<TFieldType?, bool> scopedValue,//Func<TFieldType?, bool> validationFunc,
        Action<ValidationOptions>? optionsAction = null
    )
    {
        var validation = new IsCustomValidation<TFieldType>(
            descriptionTemplate,
            errorTemplate,
            scopedValue
        );
        optionsAction?.Invoke(new ValidationOptions(validation));
        fieldDescriptor.AddValidation(validation);
        return fieldDescriptor;
    }

    public static IFieldDescriptor<TValidationType, TPropertyType?> Is<TValidationType, TPropertyType>(
         this IFieldDescriptor<TValidationType, TPropertyType?> fieldDescriptor,
         string errorTemplate,
         string descriptionTemplate,
         Func<TPropertyType, IScopedData<TPropertyType?, bool>> scopedResult,
         Action<ValidationOptions>? optionsAction = null
    )
    {
        var validation = new IsCustomScopedValidation<TPropertyType>(errorTemplate, descriptionTemplate, scopedResult);
        optionsAction?.Invoke(new ValidationOptions(validation));
        fieldDescriptor.AddValidation(validation);
        return fieldDescriptor;
    }
}
