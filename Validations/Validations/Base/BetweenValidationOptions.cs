using PopValidations.Execution.Validations.Base;
using System;

namespace PopValidations.Validations.Base;

public class BetweenValidationOptions<TPropertyType>
    : ValidationOptions<ILengthBetweenValidation<TPropertyType>>
{
    public BetweenValidationOptions(
        ILengthBetweenValidation<TPropertyType> validationComponent
    ) : base(validationComponent) { }

    public BetweenValidationOptions<TPropertyType> SetComparer(
        IPropertyLengthComparer<TPropertyType> comparer
    )
    {
        if (comparer == null)
            throw new ArgumentNullException(nameof(comparer));

        Component.Comparer = comparer;
        return this;
    }
}
