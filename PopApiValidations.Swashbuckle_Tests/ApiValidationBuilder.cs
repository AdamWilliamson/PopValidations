using ApprovalTests;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PopApiValidations.Swashbuckle;

namespace PopApiValidations.Swashbuckle_Tests;

public class ApiValidationBuilder
{
    private readonly PopApiOpenApiConfig config;
    private readonly JObject? openApi;
    private readonly JObject? clean;
    private readonly string url;
    private readonly string type;
    private AssertionResult errors = new AssertionResult();

    public ApiValidationBuilder(PopApiOpenApiConfig config, JObject? openApi, JObject? clean, string url, string type)
    {
        this.config = config;
        this.openApi = openApi;
        this.clean = clean;
        this.url = url;
        this.type = type;
    }

    public ParamBuilder<TParamType> ParamIs<TParamType>(string paramName)
    {
        return new ParamBuilder<TParamType>(ParamType.Auto, config, new(config, errors, openApi, clean), [paramName], url, type);
    }

    public ParamBuilder<TParamType> ParamIs<TParamType>(string paramName, string childProperty)
    {
        return new ParamBuilder<TParamType>(ParamType.Auto, config, new(config, errors, openApi, clean), [paramName, childProperty], url, type);
    }

    public ParamBuilder<TParamType> ParamIs<TParamType>(params string[] objHeirarchy)
    {
        return new ParamBuilder<TParamType>(ParamType.Auto, config, new(config, errors, openApi, clean), objHeirarchy, url, type);
    }

    public ParamBuilder<TParamType> ParamIs<TParamType>(ParamType paramType, params string[] objHeirarchy)
    {
        return new ParamBuilder<TParamType>(paramType, config, new(config, errors, openApi, clean), objHeirarchy, url, type);
    }

    public void Validate()
    {
        if (!errors.Success) 
        {
            Approvals.AssertEquals(
                clean.ToString(Formatting.Indented),
                openApi.ToString(Formatting.Indented)
            );
        }
    }

    public void Compare()
    {
        Approvals.AssertEquals(
            clean.ToString(Formatting.Indented),
            openApi.ToString(Formatting.Indented)
        );
    }
}
