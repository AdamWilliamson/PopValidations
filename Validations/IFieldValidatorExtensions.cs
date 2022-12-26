using System;
using System.Collections.Generic;
using PopValidations.Execution.Validations.Base;
using PopValidations.FieldDescriptors.Base;
using PopValidations.Scopes.ForEachs;
using PopValidations.Validations;
using PopValidations.Validations.Base;
using PopValidations.ValidatorInternals;

namespace Validations;

public static partial class IFieldValidatorExtensions
{
    public static IFieldDescriptor<TValidationType, TFieldType> Vitally<TValidationType, TFieldType>(this IFieldDescriptor<TValidationType, TFieldType> fieldDescriptor)
    {
        fieldDescriptor.NextValidationIsVital();
        return fieldDescriptor;
    }

    public static IFieldDescriptor<TValidationType, TFieldType> NotNull<TValidationType, TFieldType>(
    this IFieldDescriptor<TValidationType, TFieldType> fieldDescriptor,
    Action<ValidationOptions>? optionsAction = null
    )
    {
        var validation = new NotNullValidation();
        optionsAction?.Invoke(new ValidationOptions(validation));
        fieldDescriptor.AddValidation(validation);
        return fieldDescriptor;
    }

    public static IFieldDescriptor<TValidationType, TFieldType> IsEqualTo<TValidationType, TFieldType>(
        this IFieldDescriptor<TValidationType, TFieldType> fieldDescriptor,
        TFieldType value,
        Action<ValidationOptions>? optionsAction = null
        )
        where TFieldType : IComparable
    {
        var validation = new IsEqualToValidation(value);
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

    public static IFieldDescriptor<TValidationType, TFieldType> SetValidator<TValidationType, TFieldType>(
        this IFieldDescriptor<TValidationType, TFieldType> fieldDescriptor,
        ISubValidatorClass validatorClass
    )
    {
        fieldDescriptor.AddValidation(validatorClass);
        return fieldDescriptor;
    }

    public static IFieldDescriptor<TClassType, IEnumerable<TPropertyType>> ForEach<TClassType, TPropertyType>(
            this IFieldDescriptor<TClassType, IEnumerable<TPropertyType>> fieldDescriptor,
            Action<IFieldDescriptor<IEnumerable<TPropertyType>, TPropertyType>> actions
            )
    {
        var forEachScope = new ForEachScope<TClassType, TPropertyType>(
            fieldDescriptor,
            //null,
            actions
        );
        fieldDescriptor.AddValidation(forEachScope);
        return fieldDescriptor;
    }
}