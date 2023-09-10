using System;
using PopValidations.Validations.Base;

namespace PopValidations.Validations;

public class IsCustomValidation<TFieldType> : ValidationComponentBase
{
    public override string DescriptionTemplate { get; protected set; }
    public override string ErrorTemplate { get; protected set; }

    private readonly Func<TFieldType?, bool> customValidationFunc;
    private IScopedData<bool> scopedValue;

    public IsCustomValidation(
        string descriptionTemplate,
        string errorTemplate,
        Func<TFieldType?, bool> customValidationFunc)
        : this(
              descriptionTemplate, 
              errorTemplate, 
              new ScopedData<TFieldType?, bool>(string.Empty, customValidationFunc))
    {
        //DescriptionTemplate = descriptionTemplate;
        //ErrorTemplate = errorTemplate;
        //this.customValidationFunc = customValidationFunc;
    }

    public IsCustomValidation(
        string descriptionTemplate,
        string errorTemplate,
        IScopedData<bool> scopedValue
        )
    {
        DescriptionTemplate = descriptionTemplate;
        ErrorTemplate = errorTemplate;
        this.scopedValue = scopedValue;
    }

    public override ValidationActionResult Validate(object? value)
    {
        try
        {
            if (value == null)
            {
                scopedValue?.SetParent(new ScopedData<TFieldType?>(default));
                if (customValidationFunc != null && customValidationFunc.Invoke(default))
                {
                    return CreateValidationSuccessful();
                } 
                else if (scopedValue != null && scopedValue.GetValue()  is true)//(scopedValue.GetValue() as Func<TFieldType?, bool>)?.Invoke(default) is true)
                {
                    return CreateValidationSuccessful();
                }
            }
            else if (value is TFieldType converted)
            {
                scopedValue?.SetParent(new ScopedData<TFieldType?>(converted));
                if (customValidationFunc != null && customValidationFunc.Invoke(converted))
                {
                    return CreateValidationSuccessful();
                }
                else if (scopedValue != null && scopedValue.GetValue() is true)
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
       
        //var valueAsString = value?.ToString() ?? "null";

        return CreateValidationError(
            ("value", scopedValue?.Describe() ?? String.Empty)
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
