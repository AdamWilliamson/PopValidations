using System;
using System.Collections.Generic;
using System.Linq;
using PopValidations.Configurations.Languages;

namespace PopValidations.Configurations;

public class LangaugeConfiguration
{
    private readonly List<ILanguageSource> languageSources = new();
    public string? CurrentCulture { get; protected set; } = null;
    public ILanguageSource CurrentLanguage { get; protected set; } = new NullLanguageSource();

    public void SetCurrentCulture(string? culture)
    {
        if (culture == null)
        {
            CurrentCulture = culture;
            CurrentLanguage = new NullLanguageSource();
        }
        else
        {
            var foundLanguage = languageSources
                .FirstOrDefault(x =>
                    x.GetCultures().Any(c => c.StartsWith(culture))
                );

            if (foundLanguage != null)
            {
                CurrentCulture = culture;
                CurrentLanguage = foundLanguage;
            }
            else
            {
                throw new Exception("Current Culture not found.");
            }
        }
    }

    public void AddLanguages(params ILanguageSource[] languages)
    {
        languageSources.InsertRange(0, languages);
    }
}
