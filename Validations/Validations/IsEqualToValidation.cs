using System;
using PopValidations.FieldDescriptors.Base;
using PopValidations.Validations.Base;

namespace PopValidations.Validations;

public class IsEqualToValidation : ValidationComponentBase
{
    public override string DescriptionTemplate { get; protected set; } = "Must equal to '{{value}}'";
    public override string ErrorTemplate { get; protected set; } = "Is not equal to '{{value}}'";

    private readonly IComparable? Value = null;
    private readonly IScopeData? scopedData = null;

    public IsEqualToValidation(IScopeData scopedData)
    {
        this.scopedData = scopedData;
    }

    public IsEqualToValidation(IComparable? value)
    {
        Value = value;
    }

    public override void ReHomeScopes(IFieldDescriptorOutline attemptedScopeFieldDescriptor)
    {
        scopedData?.ReHome(attemptedScopeFieldDescriptor);
    }

    public override ValidationActionResult Validate(object? value)
    {
        var RealValue = GetData(scopedData, Value);

        if (
            (RealValue == null && value == null)
            || RealValue?.Equals(value) == true
        )
        {
            return CreateValidationSuccessful();
        }
        else
        {
            var valueAsString = RealValue?.ToString() ?? "null";

            return CreateValidationError(
                    ("value", valueAsString)
            );
        }
    }

    public override DescribeActionResult Describe()
    {
        var describedValue = GetDataDescription(scopedData, Value);
        
        return CreateDescription(
                ("value", describedValue)
        );
    }
}
