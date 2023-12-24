using ApprovalTests;
using Newtonsoft.Json.Linq;
using PopValidations.Swashbuckle_Tests.Helpers;
using PopValidations_Tests.TestHelpers;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PopValidations.Demonstration_Tests.ValidatorDemoTests.ScopeWhen;

public static class ScopeWhen
{
    //Begin-Request
    public record Level1(bool Check, string? DependantField);
    //End-Request

    //Begin-Validator
    public static class DataRetriever
    {
        public static Task<string> GetValue(Level1 v) 
        { 
            return Task.FromResult(v?.DependantField + " GetValue"); 
        }
        public static Task<string> GetMoreValue(Level1 v) 
        { 
            return Task.FromResult(v?.DependantField + " GetMoreValue"); 
        }
    }

    public class Validator : AbstractValidator<Level1>
    {
        public Validator()
        {
            ScopeWhen(
                "When Check is True 1",
                x => Task.FromResult(x.Check),
                "Database Value 1",
                (x) => DataRetriever.GetValue(x),
                (retrievedData) =>
                {
                    Describe(x => x.DependantField).IsEqualTo(retrievedData);
                }
            );

            ScopeWhen(
                "When Check is True 2",
                x => x.Check,
                "Database Value 2",
                (x) => (x?.DependantField ?? "null value") + " thing 1",
                (retrievedData) =>
                {
                    Describe(x => x.DependantField).IsEqualTo(retrievedData);
                }
            );

            ScopeWhen(
                "When Check is True 3",
                x => Task.FromResult(x.Check),
                "Database Value 3",
                (x) => DataRetriever.GetMoreValue(x),
                (moreData) =>
                {
                    Describe(x => x.DependantField).IsEqualTo(moreData);
                }
            );

            ScopeWhen(
                "When Check is True 4",
                x => x.Check,
                "Database Value 4",
                (x) => (x?.DependantField ?? "null value") + " thing 2",
                (moreData) =>
                {
                    Describe(x => x.DependantField).IsEqualTo(moreData);
                }
            );
        }
    }
    //End-Validator

    public class TestController : ControllerBase<Level1> { }
}

public class ScopeWhen_DemoTest
{
    [Fact]
    public async Task Validation()
    {
        // Arrange
        var validationRunner = ValidationRunnerHelper.BasicRunnerSetup(new ScopeWhen.Validator());

        // Act
        var validationResult = await validationRunner.Validate(
            new ScopeWhen.Level1(
                Check: true, 
                DependantField: "1"
            )
        );

        // Assert
        Approvals.VerifyJson(JsonConverter.ToJson(validationResult));
    }

    [Fact]
    public void Description()
    {
        // Arrange
        var descriptionRunner = ValidationRunnerHelper.BasicRunnerSetup(new ScopeWhen.Validator());

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
        var setup = new TestSetup<ScopeWhen.TestController, ScopeWhen.Validator, ScopeWhen.Level1>();

        // Act
        var helper = await setup.GetHelper(config);
        JObject json = JObject.Parse(helper.Content!);
        var match = json["components"]!["schemas"]!.FirstOrDefault();

        // Assert
        Approvals.VerifyJson(match?.ToString(Newtonsoft.Json.Formatting.None));
    }
}
