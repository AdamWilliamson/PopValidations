using PopValidations.Validations.Base;
using System;
using System.Collections;

namespace PopValidations.Validations;

public class IsNotEmptyValidation : ValidationComponentBase
{
    public override string DescriptionTemplate { get; protected set; } = $"Must not be empty.";
    public override string ErrorTemplate { get; protected set; } = $"Is empty.";

    public IsNotEmptyValidation() { }

    public override ValidationActionResult Validate(object? value)
    {
        switch (value)
        {
            case null:
            case Array { Length: 0 }:
            case ICollection { Count: 0 }:
            case string s when string.IsNullOrWhiteSpace(s):
            case IEnumerable e when !e.GetEnumerator().MoveNext():
                return CreateValidationError();
        }

        return CreateValidationSuccessful();
    }

    public override DescribeActionResult Describe()
    {
        return CreateDescription();
    }

}
