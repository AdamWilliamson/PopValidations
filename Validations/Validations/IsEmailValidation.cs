using PopValidations.Validations.Base;
using System;
using System.Net.Mail;

namespace PopValidations.Validations;

public class IsEmailValidation : ValidationComponentBase
{
    public override string DescriptionTemplate { get; protected set; } = $"Must be a valid email.";
    public override string ErrorTemplate { get; protected set; } = $"Is not a valid email.";

    public IsEmailValidation() { }

    public override ValidationActionResult Validate(object? value)
    {
        try
        {
            MailAddress email = new(value as string ?? "");

            return CreateValidationSuccessful();
        }
        catch (Exception)
        {
            return CreateValidationError();
        }
    }

    public override DescribeActionResult Describe()
    {
        return CreateDescription();
    }
}
