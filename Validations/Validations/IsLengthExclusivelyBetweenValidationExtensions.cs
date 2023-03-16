using PopValidations.FieldDescriptors.Base;
using PopValidations.Validations;
using PopValidations.Validations.Base;
using System;

namespace PopValidations;

public static class IsLengthExclusivelyBetweenValidationExtensions
{
    public static IFieldDescriptor<TValidationType, TPropertyType?> IsLengthExclusivelyBetween<
        TValidationType,
        TPropertyType
    >(
        this IFieldDescriptor<TValidationType, TPropertyType?> fieldDescriptor,
        int start,
        int end,
        Action<BetweenValidationOptions<TPropertyType>>? optionsAction = null
    )
    {
        var startScopedData = new ScopedData<int?>(start);
        var endScopedData = new ScopedData<int?>(end);

        return IsLengthExclusivelyBetween(
            fieldDescriptor,
            startScopedData,
            endScopedData,
            optionsAction
        );
    }

    public static IFieldDescriptor<TValidationType, TPropertyType?> IsLengthExclusivelyBetween<
        TValidationType,
        TPropertyType
    >(
        this IFieldDescriptor<TValidationType, TPropertyType?> fieldDescriptor,
        ScopedData<int?> start,
        ScopedData<int?> end,
        Action<BetweenValidationOptions<TPropertyType>>? optionsAction = null
    )
    {
        var validation = new IsLengthExclusivelyBetweenValidation<TPropertyType>(
            start,
            end
        );
        optionsAction?.Invoke(
            new BetweenValidationOptions<TPropertyType>(validation)
        );
        fieldDescriptor.AddValidation(validation);
        return fieldDescriptor;
    }
}
