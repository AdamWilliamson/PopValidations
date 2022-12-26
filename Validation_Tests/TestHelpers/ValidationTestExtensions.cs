using PopValidations.Execution.Validation;
using PopValidations.Validations.Base;
using System.Collections.Generic;

namespace PopValidations_Tests.TestHelpers;

public static class ValidationTestExtensions
{
    public static List<ValidationActionResult> GetErrorsForField(this ValidationResult? results, string fieldName)
    {
        //return results?.FieldErrors.Single(o => o.Property == fieldName).Errors ?? new List<ValidationError>();
        return new();
    }

    public static bool ContainsError(this ValidationResult? results, string fieldName, string error)
    {
        //return results?.GetErrorsForField(fieldName)?.Any(e => e.Error == error) ?? false;
        return new();
    }
}