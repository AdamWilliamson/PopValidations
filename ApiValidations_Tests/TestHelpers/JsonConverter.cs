using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PopValidations.Execution.Description;
using PopValidations.Execution.Validation;

namespace ApiValidations_Tests.TestHelpers;

public static class JsonConverter
{
    public static string ToJson(DescriptionResult description)
    {
        return JsonConvert.SerializeObject(description, new JsonSerializerSettings() 
        {
            //Formatting = Formatting.Indented,
            ContractResolver = new DefaultContractResolver()
            {                 
                NamingStrategy = new DefaultNamingStrategy
                {
                    ProcessDictionaryKeys = true,
                },
            }
        });
    }

    public static string ToJson(ValidationResult validation)
    {
        return JsonConvert.SerializeObject(validation, new JsonSerializerSettings() 
        {
            //Formatting = Formatting.Indented,
            ContractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new DefaultNamingStrategy 
                {
                    ProcessDictionaryKeys = true,
                },
            }
        }).Trim();
    }
}
