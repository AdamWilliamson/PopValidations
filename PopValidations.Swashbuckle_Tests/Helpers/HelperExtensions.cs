using Newtonsoft.Json.Linq;

namespace PopValidations.Swashbuckle_Tests.Helpers;

public static class HelperExtensions
{
    public static JToken? GetValidationAttribute(this OpenApiHelper parent, string schemaObject)
    {
        return parent.Get($"components.schemas.{schemaObject}.{parent.Config.CustomValidationAttribute}");
    }

    public static JArray? GetValidationAttributeProperty(
        this OpenApiHelper parent,
        string schemaObject,
        string propertyName)
    {
        return (JArray?)parent.GetValidationAttribute(schemaObject)?.SelectToken(propertyName);
    }
}