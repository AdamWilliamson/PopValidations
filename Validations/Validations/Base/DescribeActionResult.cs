using System.Collections.Generic;

namespace PopValidations.Validations.Base;

public class DescribeActionResult
{
    public DescribeActionResult(
        string validator,
        string message,
        List<KeyValuePair<string, string>> keyValues)
    {
        Validator = validator;
        Message = message;
        KeyValues = keyValues;
    }

    public string Validator { get; }
    public string Message { get; protected set; }
    public List<KeyValuePair<string, string>> KeyValues { get; }

    internal void UpdateDescription(string message)
    {
        Message = message;
    }
}
