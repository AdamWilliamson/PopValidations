using System;
using System.Collections.Generic;
using PopValidations.FieldDescriptors.Base;
using PopValidations.Scopes.ForEachs;
using PopValidations.ValidatorInternals;

namespace PopValidations;

public static partial class IFieldValidatorExtensions
{
    public static IFieldDescriptor<TValidationType, TFieldType?> Vitally<
        TValidationType,
        TFieldType
    >(this IFieldDescriptor<TValidationType, TFieldType?> fieldDescriptor)
    {
        fieldDescriptor.NextValidationIsVital();
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
        fieldDescriptor.AddValidation(validatorClass);
        return fieldDescriptor;
    }

    public static IFieldDescriptor<TClassType, IEnumerable<TPropertyType?>?> ForEach<
        TClassType,
        TPropertyType
    >(
        this IFieldDescriptor<TClassType, IEnumerable<TPropertyType?>?> fieldDescriptor,
        Action<IFieldDescriptor<IEnumerable<TPropertyType?>, TPropertyType?>> actions
    )
        where TClassType : class
    {
        var forEachScope = new ForEachScope<TClassType, TPropertyType>(
            fieldDescriptor,
            actions
        );
        //fieldDescriptor.Store.AddItemToCurrentScope(null, forEachScope);
        fieldDescriptor.AddSelfDescribingEntity(forEachScope);
        //fieldDescriptor.AddValidation(forEachScope);

        //==
        //var subvalidator = new ForEachItemSubValidator<TClassType, TPropertyType>(
        //    fieldDescriptor,
        //    actions
        //);

        //fieldDescriptor.Store.AddItem(null, subvalidator);

        return fieldDescriptor;
    }
}
