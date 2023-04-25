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
                throw new ValidationException("value is wrong type");
            }
        }
        catch (Exception ex)
        {
            throw new ValidationException("Custom function exceptioned", ex);
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

public class IsCustomScopedValidation<TInputType> : ValidationComponentBase
{
    private readonly Func<TInputType, IScopedData<bool>> scopedTest;

    public override string DescriptionTemplate { get; protected set; } = string.Empty;
    public override string ErrorTemplate { get; protected set; } = string.Empty;

    public IsCustomScopedValidation(string error, string description, Func<TInputType, IScopedData<bool>> test)
    {
        ErrorTemplate = error;
        DescriptionTemplate = description;
        this.scopedTest = test;
    }

    public override ValidationActionResult Validate(object? value)
    {
        if (value is not TInputType inputType)
            throw new Exception("Invalid Data Conversion");
        bool valid = false;

        try
        {
            var scopedData = scopedTest.Invoke(inputType);
            scopedData.Init(value).Wait();
            valid = scopedData.GetValue() switch
            {
                bool b => b,
                _ => false
            };

            if (valid)
            {
                return CreateValidationSuccessful();
            }
            else
            {
                var valueAsString = value?.ToString() ?? "null";

                return CreateValidationError(
                        ("value", valueAsString)
                );
            }
        }
        catch(Exception ex)
        {
            throw new Exception("Custom function exceptioned", ex);
        }
    }

    public override DescribeActionResult Describe()
    {
        return CreateDescription();
    }
}
