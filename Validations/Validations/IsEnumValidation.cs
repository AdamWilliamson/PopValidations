using System;
using System.Linq;
using PopValidations.Validations.Base;

namespace PopValidations.Validations;

public class IsEnumValidation : ValidationComponentBase
{
    private readonly Type enumType;

    public override string DescriptionTemplate { get; protected set; } = "Must be one of '{{enumNames}}' or '{{enumValues}}'";
    public override string ErrorTemplate { get; protected set; } = "'{{value}}' Is not a valid value.";

    public IsEnumValidation(Type enumType)
    {
        if (enumType == null || !enumType.IsEnum)
        {
            throw new ArgumentException("Type is not of an Enum");
        }

        this.enumType = enumType;
    }

    public override ValidationActionResult Validate(object? value)
    {
        if (
            value != null
            && Enum.TryParse(enumType, value?.ToString(), true, out var result)
            && result != null
            && Enum.IsDefined(enumType, result)
        )
        {
            return CreateValidationSuccessful();
        }
        else
        {
            var valueAsString = value?.ToString() ?? "null";

            return CreateValidationError(
                ("value", valueAsString),
                ("enumNames", string.Join(',', Enum.GetNames(enumType))),
                (
                    "enumValues",
                    string.Join(
                        ',',
                        Enum.GetValues(enumType).Cast<Enum>().Select(x => x.ToString("d")).ToArray()
                    )
                )
            );
        }
    }

    public override DescribeActionResult Describe()
    {
        return CreateDescription(
            ("enumNames", string.Join(',', Enum.GetNames(enumType))),
            (
                "enumValues",
                string.Join(
                    ',',
                    Enum.GetValues(enumType).Cast<Enum>().Select(x => x.ToString("d")).ToArray()
                )
            )
        );
    }
}
