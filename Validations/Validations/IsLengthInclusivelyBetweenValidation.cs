using PopValidations.Validations.Base;
using System.Threading.Tasks;

namespace PopValidations.Validations;

public class IsLengthInclusivelyBetweenValidation<TPropertyType>
    : ValidationComponentBase, ILengthBetweenValidation<TPropertyType>
{
    private readonly IScopedData<int?> startValue;
    private readonly IScopedData<int?> endValue;
    public IPropertyLengthComparer<TPropertyType> Comparer { get; set; } =
        new SimplePropertyLengthComparer<TPropertyType>();

    public override string DescriptionTemplate { get; protected set; } =
        "Must be between {{startValue}} and {{endValue}} inclusive.";
    public override string ErrorTemplate { get; protected set; } =
        "Is not between {{startValue}} and {{endValue}} inclusive.";

    public IsLengthInclusivelyBetweenValidation(
        IScopedData<int?> startValue,
        IScopedData<int?> endValue
    )
    {
        this.startValue = startValue;
        this.endValue = endValue;
    }

    public override Task InitScopes(object? instance)
    {
        startValue.Init(instance);
        endValue.Init(instance);
        return Task.CompletedTask;
    }

    public override ValidationActionResult Validate(object? value)
    {
        var start = startValue.GetTypedValue();
        var end = endValue.GetTypedValue();

        if (start == null || end == null)
        {
            throw new ValidationException("Start value should be greater than end value.");
        }
        else
        {
            if (start.Value.CompareTo(end.Value) >= 0)
            {
                throw new ValidationException("Start value should be greater than end value.");
            }
        }

        switch (value)
        {
            case TPropertyType converted:
                if (
                    Comparer.Compare(converted, start.Value) >= 0 && Comparer.Compare(converted, end.Value) <= 0
                )
                {
                    return CreateValidationSuccessful();
                }
                else
                {
                    return CreateValidationError(
                        ("startValue", start.ToString() ?? ""),
                        ("endValue", end.ToString() ?? "")
                    );
                }
            default:
                throw new ValidationException("Type is invalid");
        }
    }

    public override DescribeActionResult Describe()
    {
        return CreateDescription(
            ("startValue", startValue.Describe()),
            ("endValue", endValue.Describe())
        );
    }
}
