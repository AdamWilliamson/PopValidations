using PopValidations.Execution.Validations.Base;
using PopValidations.FieldDescriptors.Base;
using PopValidations.Validations;
using System;

namespace PopValidations;

public static class NotNullValidationExtensions
{
    public static IFieldDescriptor<TValidationType, TFieldType?> NotNull<TValidationType, TFieldType>(
        this IFieldDescriptor<TValidationType, TFieldType?> fieldDescriptor,
        Action<ValidationOptions>? optionsAction = null
        )
    {
        var validation = new NotNullValidation();
        optionsAction?.Invoke(new ValidationOptions(validation));
        fieldDescriptor.AddValidation(validation);
        return fieldDescriptor;
    }
}
