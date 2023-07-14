using PopValidations.Execution.Description;
using PopValidations.Swashbuckle;
using PopValidations.ValidatorInternals;
using PopValidations_Tests.TestHelpers;

namespace PopValidations.Swashbuckle_Tests.Helpers;

public interface ITestSetup
{
    Task<OpenApiHelper> GetHelper(OpenApiConfig config);
    string Scenario { get; }

    Type ControllerType { get; }
    Type ValidatorType { get; }
    Type RequestType { get; }
}

public class TestSetup<TTestController, TRequestValidator, TRequest> : ITestSetup
    where TRequestValidator : IMainValidator<TRequest>, new()
{
    internal ApiWebApplicationFactory Factory { get; } = new();
    private HttpClient Client { get; set; }
    public DescriptionResult Description { get; private set; }
    private string Content { get; set; }
    private OpenApiHelper Helper { get; set; }

    public Type ControllerType => typeof(TTestController);
    public Type ValidatorType => typeof(TRequestValidator);
    public Type RequestType => typeof(TRequest); 

    public string Scenario
    {
        get
        {
            return $"{typeof(TTestController).FullName.Split(".").Last().Split("+").First()}";
        }
    }

    public TestSetup() { }

    private void Configure(OpenApiConfig config)
    {
        Factory
            .AddController<TTestController>()
            .AddValidator<TRequestValidator, TRequest>()
            .WithConfig(config);

        Client = Factory.CreateClient();
    }

    private async Task GetSwagger()
    {
        var json = await Client.GetAsync("/swagger/v1/swagger.json");
        Content = await json.Content.ReadAsStringAsync();
    }

    private void Describe()
    {
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new TRequestValidator());

        Description = runner.Describe();
    }

    public async Task<OpenApiHelper> GetHelper(OpenApiConfig config)
    {
        Configure(config);
        await GetSwagger();
        Describe();

        Helper = new OpenApiHelper(config, Content, Description);
        return Helper;
    }
}
