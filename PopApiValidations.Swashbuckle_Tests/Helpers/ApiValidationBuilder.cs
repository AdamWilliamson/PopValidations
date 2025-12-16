using ApprovalTests;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PopApiValidations.Swashbuckle;

namespace PopApiValidations.Swashbuckle_Tests.Helpers;

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

    /// <summary>
    /// Is used to identify the parameter, and then, its child property selected, to the nth degree.
    /// </summary>
    /// <typeparam name="TParamType"></typeparam>
    /// <param name="objHeirarchy">the param name, and its child property selected, to the nth degree.</param>
    /// <returns>the builder</returns>
    /// <exception cref="ArgumentException"></exception>
    public ParamBuilder<TParamType> ParamIs<TParamType>(string[] objHeirarchy)
    {
        if (objHeirarchy == null || objHeirarchy.Length == 0)
        {
            throw new ArgumentException("objHeirarchy must have at least one element to identify the parameter.");
        }

        return new ParamBuilder<TParamType>(ParamType.Auto, config, new(errors, openApi, clean), objHeirarchy, url, type);
    }

    /// <summary>
    /// Is used to identify the parameter, and then, its child property selected, to the nth degree.
    /// </summary>
    /// <typeparam name="TParamType"></typeparam>
    /// <param name="paramType"></param>
    /// <param name="objHeirarchy"></param>
    /// <returns></returns>
    public ParamBuilder<TParamType> ParamIs<TParamType>(ParamType paramType, string[] objHeirarchy)
    {
        return new ParamBuilder<TParamType>(paramType, config, new(errors, openApi, clean), objHeirarchy, url, type);
    }

    public ReturnBuilder<TParamType> ReturnIs<TParamType>(params string[] objHeirarchy)
    {
        return new ReturnBuilder<TParamType>(config, new(errors, openApi, clean), objHeirarchy ?? [], url, type);
    }

    public void Validate()
    {
        if (!errors.Success)
        {
            Approvals.VerifyJson(JsonCompare.FindDiffString(clean, openApi));
        }
    }
}
