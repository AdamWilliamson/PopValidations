using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PopValidations.Execution.Description;
using PopValidations.Execution.Validation;

namespace Validations_Tests.TestHelpers;

public static class JsonConverter
{
    public static string ToJson(DescriptionResult description)
    {
        return JsonConvert.SerializeObject(description, new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy
                {
                    ProcessDictionaryKeys = true,
                },
            }
        });
    }

    public static string ToJson(ValidationResult validation)
    {
        return JsonConvert.SerializeObject(validation, new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy
                {
                    ProcessDictionaryKeys = true,
                }
            }
        });
    }
}
