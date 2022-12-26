using System.Collections.Generic;

namespace PopValidations.Configurations.Languages;

public interface ILanguageSource
{
    List<string> GetCultures();
    string GetErrorTranslation(string key, string originalMessage);
    string GetDescriptionTranslation(string key, string originalMessage);
}
