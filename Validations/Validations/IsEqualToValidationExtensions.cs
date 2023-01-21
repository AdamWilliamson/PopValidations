using PopValidations.Execution.Validations.Base;
using PopValidations.FieldDescriptors.Base;
using PopValidations.Validations;
using PopValidations.Validations.Base;
using System;

namespace PopValidations;

public static class IsEqualToValidationExtensions
{
    //public static IFieldDescriptor<TValidationType, string> IsEqualTo<TValidationType>(
    //    this IFieldDescriptor<TValidationType, string> fieldDescriptor,
    //    string value,
    //    Action<ValidationOptions>? optionsAction = null
    //)
    //{
    //    var scopedData = new ScopedData<TValidationType, string>(value);
    //    var validation = new IsEqualToValidation(scopedData);
    //    optionsAction?.Invoke(new ValidationOptions(validation));
    //    fieldDescriptor.AddValidation(validation);
    //    return fieldDescriptor;
    //}

    public static IFieldDescriptor<TValidationType, string?> IsEqualTo<TValidationType>(
    this IFieldDescriptor<TValidationType, string?> fieldDescriptor,
    string value,
    Action<ValidationOptions>? optionsAction = null
)
    {
        var scopedData = new ScopedData<TValidationType, string>(value);
        var validation = new IsEqualToValidation(scopedData);
        optionsAction?.Invoke(new ValidationOptions(validation));
        fieldDescriptor.AddValidation(validation);
        return fieldDescriptor;
    }

    public static IFieldDescriptor<TValidationType, TFieldType> IsEqualTo<TValidationType, TFieldType>(
        this IFieldDescriptor<TValidationType, TFieldType> fieldDescriptor,
        IComparable value,
        Action<ValidationOptions>? optionsAction = null
    )
        where TFieldType : IComparable
    {
        var scopedData = new ScopedData<TValidationType, IComparable>(value);
        var validation = new IsEqualToValidation(scopedData);
        optionsAction?.Invoke(new ValidationOptions(validation));
        fieldDescriptor.AddValidation(validation);
        return fieldDescriptor;
    }

    public static IFieldDescriptor<TValidationType, TFieldType> IsEqualTo<TValidationType, TFieldType, TPassThrough>(
        this IFieldDescriptor<TValidationType, TFieldType> fieldDescriptor,
        IScopedData<TPassThrough?> scopedData,
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