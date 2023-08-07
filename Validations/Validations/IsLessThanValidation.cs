using System;
using System.Threading.Tasks;
using PopValidations.FieldDescriptors.Base;
using PopValidations.Validations.Base;

namespace PopValidations.Validations;

public class IsLessThanValidation : ValidationComponentBase
{
    public override string DescriptionTemplate { get; protected set; } =
        "Must be less than '{{value}}'.";
    public override string ErrorTemplate { get; protected set; } = "Is not less than '{{value}}'.";

    private readonly IScopeData scopedData;

    public IsLessThanValidation(IScopeData scopedData)
    {
        this.scopedData = scopedData;
    }

    public override async Task InitScopes(object? instance)
    {
        await scopedData.Init(instance);
    }

    public override void ReHomeScopes(IFieldDescriptorOutline attemptedScopeFieldDescriptor)
    {
        scopedData.ReHome(attemptedScopeFieldDescriptor);
    }

    public override ValidationActionResult Validate(object? value)
    {
        var RealValue = (IComparable?)scopedData.GetValue();

        if (
            RealValue?.GetType() == value?.GetType()
            && ((RealValue == null && value == null) || RealValue?.CompareTo(value) > 0)
        )
        {
            return CreateValidationSuccessful();
        }
        else
        {
            var valueAsString = RealValue?.ToString() ?? "null";

            return CreateValidationError(("value", valueAsString));
        }
    }

    public override DescribeActionResult Describe()
    {
        var describedValue = scopedData.Describe();

        return CreateDescription(("value", describedValue));
    }
}
