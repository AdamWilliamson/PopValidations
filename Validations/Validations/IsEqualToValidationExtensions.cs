using PopValidations.Execution.Validations.Base;
using PopValidations.FieldDescriptors.Base;
using PopValidations.Validations;
using PopValidations.Validations.Base;
using System;

namespace PopValidations;

public static class IsEqualToValidationExtensions
{
    public static IFieldDescriptor<TValidationType, TFieldType> IsEqualTo<TValidationType, TFieldType>(
        this IFieldDescriptor<TValidationType, TFieldType> fieldDescriptor,
        TFieldType value,
        Action<ValidationOptions>? optionsAction = null
    )
        where TFieldType : IComparable
    {
        var scopedData = new ScopedData<TValidationType, TFieldType>(value);
        var validation = new IsEqualToValidation(scopedData);
        optionsAction?.Invoke(new ValidationOptions(validation));
        fieldDescriptor.AddValidation(validation);
        return fieldDescriptor;
    }

    public static IFieldDescriptor<TValidationType, TFieldType> IsEqualTo<TValidationType, TFieldType, TPassThrough>(
        this IFieldDescriptor<TValidationType, TFieldType> fieldDescriptor,
        ScopedData<TValidationType, TPassThrough> scopedData,
        Action<ValidationOptions>? optionsAction = null
    )
        where TPassThrough : IComparable
    {
        var validation = new IsEqualToValidation(scopedData);
        optionsAction?.Invoke(new ValidationOptions(validation));
        fieldDescriptor.AddValidation(validation);
        return fieldDescriptor;
    }
}