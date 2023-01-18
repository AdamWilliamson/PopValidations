using PopValidations.Execution.Validations.Base;
using PopValidations.FieldDescriptors.Base;
using PopValidations.Validations;
using System;

namespace PopValidations;

public static class IsEmptyValidationExtensions
{
    public static IFieldDescriptor<TValidationType, TFieldType?> IsEmpty<TValidationType, TFieldType>(
    this IFieldDescriptor<TValidationType, TFieldType?> fieldDescriptor,
    Action<ValidationOptions>? optionsAction = null
    )
    {
        var validation = new IsEmptyValidation();
        optionsAction?.Invoke(new ValidationOptions(validation));
        fieldDescriptor.AddValidation(validation);
        return fieldDescriptor;
    }
}
