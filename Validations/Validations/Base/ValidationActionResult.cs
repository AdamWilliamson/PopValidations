using System.Collections.Generic;
using PopValidations.Execution.Validation;

namespace PopValidations.Validations.Base;

public class ValidationActionResult
{
    public ValidationActionResult(
        string validator,
        bool success,
        string message,
        List<KeyValuePair<string, string>> keyValues)
    {
        Validator = validator;
        Success = success;
        Message = message;
        KeyValues = keyValues;
    }

    public static ValidationActionResult Successful(string validator)
    {
        return new ValidationActionResult(validator, true, "", new());
    }

    public string Validator { get; }
    public bool Success { get; }
    public string Message { get; protected set; }
    public List<KeyValuePair<string, string>> KeyValues { get; }

    public virtual List<string>? GetFailedDependantFields(string currentProperty, ValidationResult currentValidationResult)
    {
        return null;
    }

    public void UpdateMessageProcessor(string message)
    {
        Message = message;
    }
}
