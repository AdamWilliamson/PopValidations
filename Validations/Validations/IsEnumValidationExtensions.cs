using PopValidations.Execution.Validations.Base;
using PopValidations.FieldDescriptors.Base;
using PopValidations.Validations;
using System;

namespace PopValidations;

public static class IsEnumValidationExtensions
{
    public static IFieldDescriptor<TValidationType, TFieldType> IsEnum<TValidationType, TFieldType>(
        this IFieldDescriptor<TValidationType, TFieldType> fieldDescriptor,
        Type enumType,
        Action<ValidationOptions>? optionsAction = null
    )
    {
        var validation = new IsEnumValidation<TFieldType>(enumType);
        optionsAction?.Invoke(new ValidationOptions(validation));
        fieldDescriptor.AddValidation(validation);
        return fieldDescriptor;
    }
}