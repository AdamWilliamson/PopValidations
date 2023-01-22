using System;
using PopValidations.Validations.Base;

namespace PopValidations.Validations;

public class IsCustomValidation<TFieldType> : ValidationComponentBase
{
    public override string DescriptionTemplate { get; protected set; }
    public override string ErrorTemplate { get; protected set; }

    private readonly Func<TFieldType?, bool> customValidationFunc;

    public IsCustomValidation(
        string descriptionTemplate,
        string errorTemplate,
        Func<TFieldType?, bool> customValidationFunc)
    {
        DescriptionTemplate = descriptionTemplate;
        ErrorTemplate = errorTemplate;
        this.customValidationFunc = customValidationFunc;
    }

    public override ValidationActionResult Validate(object? value)
    {
        try
        {
            if (value == null)
            {
                if (customValidationFunc.Invoke(default))
                {
                    return CreateValidationSuccessful();
                }
            }
            else if (value is TFieldType converted)
            {
                if (customValidationFunc.Invoke(converted))
                {
                    return CreateValidationSuccessful();
                }
            }
            else
            {
                throw new Exception("value is wrong type");
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Custom function exceptioned", ex);
        }
       
        var valueAsString = value?.ToString() ?? "null";

        return CreateValidationError(
            ("value", valueAsString)
        );
    }

    public override DescribeActionResult Describe()
    {
        return CreateDescription();
    }
}
