using PopValidations.FieldDescriptors.Base;
using PopValidations.Validations;
using PopValidations.Validations.Base;
using System;

namespace PopValidations;

public static class IsLengthInclusivelyBetweenValidationExtensions
{
    public static IFieldDescriptor<TValidationType, TPropertyType> IsLengthInclusivelyBetween<
        TValidationType,
        TPropertyType
    >(
        this IFieldDescriptor<TValidationType, TPropertyType> fieldDescriptor,
        int start,
        int end,
        Action<BetweenValidationOptions<TPropertyType>>? optionsAction = null
    )
    {
        var startScopedData = new ScopedData<int?>(start);
        var endScopedData = new ScopedData<int?>(end);

        return IsLengthInclusivelyBetween(
            fieldDescriptor,
            startScopedData,
            endScopedData,
            optionsAction
        );
    }

    public static IFieldDescriptor<TValidationType, TPropertyType> IsLengthInclusivelyBetween<
        TValidationType,
        TPropertyType
    >(
        this IFieldDescriptor<TValidationType, TPropertyType> fieldDescriptor,
        IScopedData<int?> start,
        IScopedData<int?> end,
        Action<BetweenValidationOptions<TPropertyType>>? optionsAction = null
    )
    {
        var validation = new IsLengthInclusivelyBetweenValidation<TPropertyType>(
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
