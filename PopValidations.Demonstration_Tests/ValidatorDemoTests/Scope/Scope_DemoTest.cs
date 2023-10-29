using ApprovalTests;
using Newtonsoft.Json.Linq;
using PopValidations.Swashbuckle_Tests.Helpers;
using PopValidations_Tests.TestHelpers;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PopValidations.Demonstration_Tests.ValidatorDemoTests.Scope;

public static class Scope
{
    //Begin-Request
    public record InputObject(string? Field);
    //End-Request

    //Begin-Validator
    public static class DataRetriever
    {
        public static Task<string> GetValue() 
        { 
            return Task.FromResult("teststring"); 
        }

        public static Task<string> GetMoreValue(InputObject obj) 
        { 
            return Task.FromResult(obj?.Field + " teststring2"); 
        }
    }

    public class Validator : AbstractValidator<InputObject>
    {
        public Validator()
        {
            Scope(
                "Database Value",
                () => DataRetriever.GetValue(),
                (retrievedData) =>
                {
                    Describe(x => x.Field).IsEqualTo(retrievedData);    
                }
            );

            Scope(
                "Second Database Value",
                (validationObject) => DataRetriever.GetMoreValue(validationObject),
                (moreData) =>
                {
                    Describe(x => x.Field).IsEqualTo(moreData);
                }
            );

            Scope(
                "Third Database Value",
                (validationobject) =>  (validationobject?.Field ?? "") + " additional value",
                (moreData) =>
                {
                    Describe(x => x.Field).IsEqualTo(moreData);
                }
            );
        }
    }
    //End-Validator

    public class TestController : ControllerBase<InputObject> { }
}

public class Scope_DemoTest
{
    [Fact]
    public async Task Validation()
    {
        // Arrange
        var validationRunner = ValidationRunnerHelper.BasicRunnerSetup(new Scope.Validator());

        // Act
        var validationResult = await validationRunner.Validate(
            new Scope.InputObject(Field: "a value")
        );

        // Assert
        Approvals.VerifyJson(JsonConverter.ToJson(validationResult));
    }

    [Fact]
    public void Description()
    {
        // Arrange
        var descriptionRunner = ValidationRunnerHelper.BasicRunnerSetup(new Scope.Validator());

        // Act
        var descriptionResult = descriptionRunner.Describe();

        // Assert
        Approvals.VerifyJson(JsonConverter.ToJson(descriptionResult));
    }

    [Fact]
    public async Task OpenApi()
    {
        // Arrange
        var config = new WebApiConfig();
        var setup = new TestSetup<Scope.TestController, Scope.Validator, Scope.InputObject>();

        // Act
        var helper = await setup.GetHelper(config);
        JObject json = JObject.Parse(helper.Content);
        var match = json["components"]["schemas"].FirstOrDefault();

        // Assert
        Approvals.VerifyJson(match?.ToString(Newtonsoft.Json.Formatting.None));
    }
}
