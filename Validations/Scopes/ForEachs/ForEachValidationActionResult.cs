using System.Collections.Generic;
using System.Linq;
using PopValidations.Execution.Validation;
using PopValidations.Validations.Base;

namespace PopValidations.Scopes.ForEachs;

public class ForEachValidationActionResult : ValidationActionResult
{
    private readonly string propertyTest;

    public ForEachValidationActionResult(string propertyTest)
        : base(nameof(ForEachValidationActionResult), true, "", new())
    {
        this.propertyTest = propertyTest;
    }

    public override List<string>? GetFailedDependantFields(
        string currentProperty,
        ValidationResult currentValidationResult
    )
    {
        currentProperty = currentProperty.Substring(
            0,
            currentProperty.LastIndexOf(propertyTest) + propertyTest.Length
        );

        if (
            currentValidationResult?.Results
                ?.Any(r => r.Property.StartsWith(currentProperty)) == true
        )
        {
            return new List<string>() { currentProperty };
        }
        return null;
    }
}
