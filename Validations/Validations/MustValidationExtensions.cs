using PopValidations.Execution.Validations.Base;
using PopValidations.FieldDescriptors.Base;
using PopValidations.Validations;
using PopValidations.Validations.Base;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PopValidations;

public static class MustValidationExtensions
{
    //public static IFieldDescriptor<TValidationType, TPropertyType?> Must<TValidationType, TPropertyType>(
    //    this IFieldDescriptor<TValidationType, TPropertyType?> fieldDescriptor,
    //    string errorTemplate, 
    //    string descriptionTemplate,
    //    Expression<Func<TPropertyType, bool>> testFunc,
    //    Action<ValidationOptions>? optionsAction = null
    //)
    //{
    //    var validation = new MustValidation<TPropertyType>(errorTemplate, descriptionTemplate, testFunc);
    //    optionsAction?.Invoke(new ValidationOptions(validation));
    //    fieldDescriptor.AddValidation(validation);
    //    return fieldDescriptor;
    //}

    public static IFieldDescriptor<TValidationType, TPropertyType?> Must<TValidationType, TPropertyType>(
         this IFieldDescriptor<TValidationType, TPropertyType?> fieldDescriptor,
         string errorTemplate,
         string descriptionTemplate,
         Func<TPropertyType, IScopedData<bool>> scopedResult,
         Action<ValidationOptions>? optionsAction = null
    )
    {
        var validation = new MustValidation<TPropertyType>(errorTemplate, descriptionTemplate, scopedResult);
        optionsAction?.Invoke(new ValidationOptions(validation));
        fieldDescriptor.AddValidation(validation);
        return fieldDescriptor;
    }
}