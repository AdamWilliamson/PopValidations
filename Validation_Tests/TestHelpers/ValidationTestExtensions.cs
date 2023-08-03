using FluentAssertions;
using PopValidations.Execution.Validation;
using PopValidations.Validations.Base;
using System.Collections.Generic;
using System.Linq;

namespace PopValidations_Tests.TestHelpers;

public static class ValidationTestExtensions
{
    public static List<string> GetErrorsForField(this ValidationResult? results, string fieldName)
    {
        return results?.Errors.Keys.ToList()
            .Where(o => o == fieldName).ToList() ?? new List<string>();
    }

    public static void ContainsAnyError(this ValidationResult? results, string fieldName)
    {
        results.GetErrorsForField(fieldName).Should().NotBeEmpty($"Expected {fieldName} not to be empty");
    }

    public static void ContainsNoError(this ValidationResult? results, string fieldName)
    {
        results.GetErrorsForField(fieldName).Should().BeEmpty($"Expected {fieldName} to be empty");
    }
}