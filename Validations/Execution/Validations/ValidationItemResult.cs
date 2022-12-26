using System.Collections.Generic;
using PopValidations.Execution.Validations;

namespace PopValidations.Execution.Validation;

public class ValidationItemResult
{
    public string Property { get; }
    public List<ValidationOutcome> Outcomes { get; } = new();


    public ValidationItemResult(string property)
    {
        Property = property;
    }
}
