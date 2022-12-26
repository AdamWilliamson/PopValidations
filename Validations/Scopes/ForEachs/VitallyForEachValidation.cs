using System.Collections.Generic;
using PopValidations.Validations.Base;

namespace PopValidations.Scopes.ForEachs;

public class VitallyForEachValidation : ValidationComponentBase
{
    private readonly string property;

    public VitallyForEachValidation(string property)
    {
        this.property = property;
    }

    public override string DescriptionTemplate { get; protected set; } = $"";
    public override string ErrorTemplate { get; protected set; } = $"";

    public override ValidationActionResult Validate(object? value)
    {
        return new ForEachValidationActionResult(property);
    }

    public override DescribeActionResult Describe()
    {
        return new DescribeActionResult(
            validator: nameof(VitallyForEachValidation),
            message: DescriptionTemplate,
            new List<KeyValuePair<string, string>>()
        );
    }
}
