using System.Collections.Generic;

namespace PopValidations.Execution.Validations;

public class DescriptionOutcome
{
    public string Validator { get; }
    public string? Message { get; }
    public List<KeyValuePair<string, string>> Values { get; }
    //public ValidationGroupResult? Group { get; set; }

    public DescriptionOutcome(string validator, string? message, List<KeyValuePair<string, string>> values)
    {
        Validator = validator;
        Message = message;
        Values = values;
    }
}
