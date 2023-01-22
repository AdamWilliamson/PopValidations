using PopValidations.Execution.Validations.Base;
using PopValidations.FieldDescriptors.Base;
using PopValidations.Validations;
using System;

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
}
