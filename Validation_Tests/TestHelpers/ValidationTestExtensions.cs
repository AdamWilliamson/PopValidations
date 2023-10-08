using FluentAssertions;
using PopValidations.Execution.Validation;
using System.Collections.Generic;
using System.Linq;

namespace PopValidations_Tests.TestHelpers;

public static class ValidationTestExtensions
{
    public static List<string> GetErrorsForField(this ValidationResult? results, List<string> fieldDepth)
    {        
        return results?.Errors
            .Where(o => o.Key.Split('.').Count() == fieldDepth.Count && fieldDepth.All(f => o.Key.Contains(f, System.StringComparison.InvariantCultureIgnoreCase)))
            .SelectMany(o => o.Value)
            .ToList()
            ?? new List<string>();
    }

    public static List<string> GetErrorsForField(this ValidationResult? results, string fieldName)
    {
        return results?.Errors
            .Where(o => string.Compare(o.Key, fieldName, true) == 0)
            .SelectMany(o => o.Value)
            .ToList()
            ?? new List<string>();
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