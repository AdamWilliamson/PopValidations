using PopValidations.Validations.Base;
using System;
using System.Threading.Tasks;

namespace PopValidations.Validations;

public class IsLengthExclusivelyBetweenValidation<TPropertyType>
    : ValidationComponentBase, ILengthBetweenValidation<TPropertyType>
{
    private readonly IScopedData<int?> startValue;
    private readonly IScopedData<int?> endValue;
    public IPropertyLengthComparer<TPropertyType> Comparer { get; set; } =
        new SimplePropertyLengthComparer<TPropertyType>();

    public override string DescriptionTemplate { get; protected set; } =
        "Must be between {{startValue}} and {{endValue}} exclusive.";
    public override string ErrorTemplate { get; protected set; } =
        "Is not between {{startValue}} and {{endValue}} exclusive.";

    public IsLengthExclusivelyBetweenValidation(
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
            throw new Exception("Start value should be greater than end value.");
        }
        else
        {
            //if (comparer != null)
            //{
            if (start.Value.CompareTo(end.Value) >= 0) //comparer.Compare(start, end) >= 0)
            {
                throw new Exception("Start value should be greater than end value.");
            }
            //}
        }

        //if (comparer != null)
        //{
        switch (value)
        {
            case TPropertyType converted:
                if (
                    Comparer.Compare(converted, start.Value) > 0 && Comparer.Compare(converted, end.Value) < 0
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
                //break;
            default:
                throw new Exception("Type is invalid");
        }

        //return value switch
        //{
        //    TPropertyType converted => comparer.Compare(converted, start) >= 0 && comparer.Compare(converted, end) <= 0,
        //    _ => throw new Exception("Type is invalid")
        //};
        //}

        //switch (value)
        //{
        //    case Array a:
        //        if (typeof(TPropertyType) == start.GetType())
        //            comparer.Compare((TPropertyType)a.Length, start);
        //    case ICollection { Count: 0 }:
        //    case string s when string.IsNullOrWhiteSpace(s):
        //    case IEnumerable e when !e.GetEnumerator().MoveNext():
        //        return CreateValidationSuccessful();
        //}

        //return CreateValidationError();
    }

    public override DescribeActionResult Describe()
    {
        return CreateDescription(
            ("startValue", startValue.Describe()),
            ("endValue", endValue.Describe())
        );
    }
}
