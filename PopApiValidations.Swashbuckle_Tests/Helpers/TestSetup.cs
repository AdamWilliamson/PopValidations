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

    //public Type ControllerType => typeof(TTestController);
    //public Type ValidatorType => typeof(TRequestValidator);

    //public string Scenario
    //{
    //    get
    //    {
    //        return $"{typeof(TTestController).FullName?.Split(".").Last().Split("+").First()}";
    //    }
    //}

    public TestSetup()
    {
        Factory
            .AddController<TTestController>();
    }

    //bool described = false;

    //private void Configure<TFuncOutput>(PopApiOpenApiConfig config, JObject cleanOpenApi, string url, string type, Func<ApiValidationBuilder<TTestController>, Expression<Func<TTestController, TFuncOutput>>> expression)
    //{
    //    var validator = new TRequestValidator();
    //    var testBuilder = new ApiValidationBuilder<TTestController>(
    //                    config,
    //                    validator,
    //                    cleanOpenApi,
    //                    url,
    //                    type
    //            );

    //    validator.DescribeFunc(expression.Invoke(testBuilder));


    //    Factory
    //        //.AddValidator<TRequestValidator, TTestController>()
    //        .AddRealizedValidator(typeof(IApiMainValidator<TTestController>), (x) =>
    //        {
    //            return validator;
    //        })
    //        .WithConfig(config);
    //    //IApiMainValidator<TValidationType>

    //    Client = Factory.CreateClient();
    //}

    private void Configurev2(PopApiOpenApiConfig config, TRequestValidator validator)
    {
        Factory
            //.AddValidator<TRequestValidator, TTestController>()
            .AddRealizedValidator(typeof(IApiMainValidator<TTestController>), (x) =>
            {
                return validator;
            })
            .WithConfig(config);
        //IApiMainValidator<TValidationType>

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

    //public async Task<OpenApiHelper> GetHelper<TFuncOutput>(PopApiOpenApiConfig config, JObject cleanOpenApi,
    //    string url, string type, Func<ApiValidationBuilder<TTestController>, Expression<Func<TTestController, TFuncOutput>>> expression)
    //{
    //    Configure(config, cleanOpenApi, url, type, expression);
    //    await GetSwagger();
    //    Describe();

    //    Helper = new OpenApiHelper(config, Content, Description);
    //    return Helper;
    //}

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

        Helper = new OpenApiHelper(config, Content, cleanOpenApi, Description, new ApiValidationBuilder(config, cleanOpenApi, url, type));
        return Helper;
    }
}
