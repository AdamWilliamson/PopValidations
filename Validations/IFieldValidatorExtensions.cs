using System;
using System.Collections.Generic;
using PopValidations.Execution;
using PopValidations.FieldDescriptors.Base;
using PopValidations.Scopes.ForEachs;
using PopValidations.ValidatorInternals;

namespace PopValidations;

public static partial class IFieldValidatorExtensions
{
    public static IFieldDescriptor<TValidationType, TFieldType> Vitally<
        TValidationType,
        TFieldType
    >(this IFieldDescriptor<TValidationType, TFieldType> fieldDescriptor)
    {
        fieldDescriptor.NextValidationIsVital();
        return fieldDescriptor;
    }

    public static IFieldDescriptor<TValidationType, TFieldType> AllNextAreVital<
        TValidationType,
        TFieldType
    >(this IFieldDescriptor<TValidationType, TFieldType> fieldDescriptor)
    {
        fieldDescriptor.SetAlwaysVital();
        return fieldDescriptor;
    }


    public static IFieldDescriptor<TValidationType, TFieldType> SetValidator<
        TValidationType,
        TFieldType
    >(
        this IFieldDescriptor<TValidationType, TFieldType> fieldDescriptor,
        ISubValidatorClass<TFieldType> validatorClass
    )
    {
        fieldDescriptor.AddSubValidator(validatorClass);
        return fieldDescriptor;
    }

    public static IFieldDescriptor<TClassType, IEnumerable<TPropertyType>> ForEach<
        TClassType,
        TPropertyType
    >(
        this IFieldDescriptor<TClassType, IEnumerable<TPropertyType>> fieldDescriptor,
        Action<IFieldDescriptor<IEnumerable<TPropertyType>, TPropertyType>> actions
    )
        where TClassType : class
    {

        if (fieldDescriptor is IFieldDescripor_Internal<TClassType, IEnumerable<TPropertyType>> converted)
        {
            var forEachScope = new ForEachScope<TClassType, TPropertyType>(
                converted,
                actions
            );

            fieldDescriptor.AddSelfDescribingEntity(forEachScope);
            return fieldDescriptor;
        }
        else
        {
            throw new PopValidationException(nameof(fieldDescriptor), "Invalid Field Descriptor");
        }
    }
}
