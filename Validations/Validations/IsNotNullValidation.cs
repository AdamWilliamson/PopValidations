﻿using PopValidations.Validations.Base;

namespace PopValidations.Validations;

public class IsNotNullValidation : ValidationComponentBase
{
    public override string DescriptionTemplate { get; protected set; } = $"Must not be null.";
    public override string ErrorTemplate { get; protected set; } = $"Is null.";

    public IsNotNullValidation() { }

    public override ValidationActionResult Validate(object? value)
    {
        if (value != null)
        {
            return CreateValidationSuccessful();
        }
        else
        {
            return CreateValidationError();
        }
    }

    public override DescribeActionResult Describe()
    {
        return CreateDescription();
    }
}
