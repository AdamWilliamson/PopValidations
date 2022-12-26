using PopValidations.Execution.Validation;

namespace PopValidations.Execution.Validations;

public class DescriptionOutcome
{
    public string PropertyName { get; set; }
    public string? Message { get; }
    public ValidationGroupResult? Group { get; set; }

    public DescriptionOutcome(string propertyName, string? message)
    {
        PropertyName = propertyName;
        Message = message;
    }
}
