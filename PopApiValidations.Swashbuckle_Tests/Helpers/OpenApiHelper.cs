using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PopValidations.Execution.Description;
using PopValidations.Swashbuckle;

namespace PopApiValidations.Swashbuckle_Tests.Helpers;

public class OpenApiHelper
{
    public OpenApiConfig Config { get; }
    public JObject? CleanContent { get; }
    public string? Content { get; }
    public JObject? ParsedContent { get; }
    public DescriptionResult? Description { get; private set; }
    public ApiValidationBuilder Builder { get; }

    private string Minify(string? json)
        => (json is null)? string.Empty : JsonConvert.SerializeObject(JsonConvert.DeserializeObject(json));

    public OpenApiHelper(OpenApiConfig config, string? content,  DescriptionResult? description)
    {
        Config = config;
        Content = Minify(content);
        Description = description;
        ParsedContent = (content is null)? null : JObject.Parse(content);
    }
    
    public OpenApiHelper(OpenApiConfig config, string? content, JObject? cleanContent, DescriptionResult? description, ApiValidationBuilder builder)
    {
        Config = config;
        Content = Minify(content);
        CleanContent = cleanContent;
        Description = description;
        Builder = builder;
        ParsedContent = (content is null) ? null : JObject.Parse(content);
    }

    public JToken? Get(string token)
    {
        return ParsedContent?.SelectToken(token);
    }

    public string? GetValue(string token)
    {
        var value = ParsedContent?.SelectToken(token);

        if (value == null) return null;
        return (string?)value;
    }

    public static implicit operator JToken?(OpenApiHelper helper)
    {
        return helper.ParsedContent;
    }
}