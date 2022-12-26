using System.Collections.Generic;
using PopValidations.Validations.Base;

namespace PopValidations.Execution;

public class MessageProcessor
{
    public MessageProcessor(){}

    public string ProcessErrorMessage(
        string validatorName,
        string message, 
        List<KeyValuePair<string, string>> keyValues
    )
    {
        message = PopValidations.Configuation.Language.CurrentLanguage
            .GetErrorTranslation(validatorName, message);

        foreach (var keyValue in keyValues)
        {
            message = message.Replace($"{{{{{keyValue.Key}}}}}", keyValue.Value);
        }

        return message;
    }

    public string ProcessDescription(
        string validatorName,
        string message,
        List<KeyValuePair<string, string>> keyValues
    )
    {
        message = PopValidations.Configuation.Language.CurrentLanguage
            .GetDescriptionTranslation(validatorName, message);

        foreach (var keyValue in keyValues)
        {
            message = message.Replace(keyValue.Key, keyValue.Value);
        }

        return message;
    }

    internal void ProcessErrorMessage(ValidationActionResult result)
    {
        result.UpdateMessageProcessor(
            ProcessErrorMessage(
                result.Validator,
                result.Message,
                result.KeyValues 
            )
        );
    }

    internal void ProcessDescriptionMessage(DescribeActionResult result)
    {
        result.UpdateDescription(
            ProcessDescription(
                result.Validator,
                result.Message,
                result.KeyValues
            )
        );
    }
}
