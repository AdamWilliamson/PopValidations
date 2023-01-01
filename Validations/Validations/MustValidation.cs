using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using PopValidations.Validations.Base;

namespace PopValidations.Validations;

public class MustValidation<TInputType> : ValidationComponentBase
{
    private readonly Func<TInputType, bool>? test = null;
    private readonly Func<TInputType, IScopedData<bool>>? scopedTest = null;

    public override string DescriptionTemplate { get; protected set; } = string.Empty;
    public override string ErrorTemplate { get; protected set; } = string.Empty;

    public MustValidation(string error, string description, Func<TInputType, bool> test)
    {
        ErrorTemplate = error;
        DescriptionTemplate = description;
        this.test = test;
    }

    public MustValidation(string error, string description, Func<TInputType, IScopedData<bool>> test)
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

        if (test is not null) 
        {
            valid = test.Invoke(inputType);
        } else if (scopedTest is not null)
        {
            var scopedData = scopedTest.Invoke(inputType);
            scopedData.Init(value).Wait();
            valid = scopedData.GetValue() switch
            {
                bool b => b,
                _ => false
            };
        }
        
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

    public override DescribeActionResult Describe()
    {
        return CreateDescription();
    }
}
