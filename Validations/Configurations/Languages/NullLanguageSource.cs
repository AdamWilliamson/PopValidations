using System.Collections.Generic;

namespace PopValidations.Configurations.Languages;

public class NullLanguageSource : ILanguageSource
{
    public List<string> GetCultures()
    {
        return new List<string>();
    }

    public string GetDescriptionTranslation(string key, string originalMessage)
    {
        return originalMessage;
    }

    public string GetErrorTranslation(string key, string originalMessage)
    {
        return originalMessage;
    }
}
