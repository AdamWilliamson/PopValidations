using ApiValidations;
using ApiValidations_Tests.TestHelpers;
using Newtonsoft.Json.Linq;
using PopApiValidations.Swashbuckle;
using PopValidations.Execution.Description;
using System.Linq.Expressions;

namespace PopApiValidations.Swashbuckle_Tests.Helpers;

public class TestSetup<TTestController, TRequestValidator>
    where TRequestValidator : ApiValidator<TTestController>, new()
{
    internal ApiWebApplicationFactory Factory { get; } = new();
    private HttpClient? Client { get; set; }
    public DescriptionResult? Description { get; private set; }
    private string? Content { get; set; }
    private OpenApiHelper? Helper { get; set; }

    public TestSetup()
    {
        Factory
            .AddController<TTestController>();
    }

    private void Configurev2(PopApiOpenApiConfig config, TRequestValidator validator)
    {
        Factory
            .AddRealizedValidator(typeof(IApiMainValidator<TTestController>), (x) =>
            {
                return validator;
            })
            .WithConfig(config);

        Client = Factory.CreateClient();
    }

    public void Register(Type t, object o)
    {
        Factory.Register(t, o);
    }

    private async Task GetSwagger()
    {
        if (Client is not null)
        {
            var json = await Client.GetAsync("/swagger/v1/swagger.json");
            Content = await json.Content.ReadAsStringAsync();
        }
    }

    private void Describe()
    {
        var validatorCreated = Activator.CreateInstance(typeof(TRequestValidator)) as TRequestValidator;
        if (validatorCreated == null)
            throw new Exception("Validator unable to be constructed, please provide an instance");

        var runner = ValidationRunnerHelper.BasicRunnerSetup(validatorCreated);

        Description = runner.Describe();
    }

    public async Task<JObject?> GetCleanContent()
    {
        Client = Factory.CreateClient();
        await GetSwagger();

        if (Content is null)
        {
            return null;
        }
        
        return JObject.Parse(Content);
    }

    public async Task<OpenApiHelper> GetHelperv2(
        PopApiOpenApiConfig config,
        JObject cleanOpenApi,
        string url, 
        string type, 
        TRequestValidator validator)
    {
        Configurev2(config, validator);
        await GetSwagger();
        Describe();

        Helper = new OpenApiHelper(
            config, 
            Content, 
            cleanOpenApi, 
            Description, 
            new ApiValidationBuilder(config, JObject.Parse(Content), cleanOpenApi, url, type)
        );
        return Helper;
    }

    internal void ReplaceRegister<TInterface, TOld, TNew>(TNew instance)
    {
        Factory.ReplaceRegister<TInterface, TOld, TNew>(instance);
    }
}
