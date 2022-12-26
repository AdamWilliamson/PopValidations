using PopValidations.Execution.Validation;

namespace PopValidations.Execution.Validations;

public class ValidationOutcome
{
    private readonly bool succeeded;
    public bool IsFailed => !succeeded;
    public bool IsSucceeded => succeeded;
    public string PropertyName { get; set; }
    public string? Message { get; }
    public ValidationGroupResult? Group { get; set; }

    public ValidationOutcome(string propertyName, bool succeeded, string? message)
    {
        PropertyName = propertyName;
        this.succeeded = succeeded;
        Message = message;
    }
}
