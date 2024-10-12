using Newtonsoft.Json.Linq;
using PopApiValidations.Swashbuckle;

namespace PopApiValidations.Swashbuckle_Tests;

public class ApiValidationBuilder
{
    private readonly PopApiOpenApiConfig config;
    private readonly JObject? openApi;
    private readonly string url;
    private readonly string type;

    public ApiValidationBuilder(PopApiOpenApiConfig config, JObject? openApi, string url, string type)
    {
        this.config = config;
        this.openApi = openApi;
        this.url = url;
        this.type = type;
    }

    public ParamBuilder<TParamType> ParamIs<TParamType>(string paramName)
    {
        return new ParamBuilder<TParamType>(config, new(config, openApi), paramName, url, type);
    }

    public ParamBuilder<TParamType> ParamIs<TParamType>(string paramName, string childProperty)
    {
        return new ParamBuilder<TParamType>(config, new(config, openApi), paramName, childProperty, url, type);
    }

    public ParamBuilder<TParamType> ParamIs<TParamType>(params string[] objHeirarchy)
    {
        return new ParamBuilder<TParamType>(config, new(config, openApi), objHeirarchy, url, type);
    }
}
