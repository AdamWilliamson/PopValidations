using ApprovalTests;
using Newtonsoft.Json.Linq;
using PopValidations.Swashbuckle_Tests.Helpers;
using PopValidations_Tests.TestHelpers;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PopValidations.Demonstration_Tests.ValidatorDemoTests.IsNotEmpty;

public static class IsNotEmpty
{
    //Begin-Request
    public record InputObject(string String, string? NString);
    //End-Request

    //Begin-Validator
    public class Validator : AbstractValidator<InputObject>
    {
        public Validator()
        {
            Describe(x => x.String).IsNotEmpty();
            Describe(x => x.NString).IsNotEmpty();
        }
    }
    //End-Validator

    public class TestController : ControllerBase<InputObject> { }
}

public class IsNotEmpty_DemoTest
{
    [Fact]
    public async Task Validation()
    {
        // Arrange
        var validationRunner = ValidationRunnerHelper.BasicRunnerSetup(new IsNotEmpty.Validator());

        // Act
        var validationResult = await validationRunner.Validate(
            new IsNotEmpty.InputObject(String: "", NString: null)
        );

        // Assert
        Approvals.VerifyJson(JsonConverter.ToJson(validationResult));
    }

    [Fact]
    public void Description()
    {
        // Arrange
        var descriptionRunner = ValidationRunnerHelper.BasicRunnerSetup(new IsNotEmpty.Validator());

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
        var setup = new TestSetup<
               IsNotEmpty.TestController,
               IsNotEmpty.Validator,
               IsNotEmpty.InputObject
           >();

        // Act
        var helper = await setup.GetHelper(config);
        JObject json = JObject.Parse(helper.Content);
        var match = json["components"]["schemas"].FirstOrDefault();

        // Assert
        Approvals.VerifyJson(match?.ToString(Newtonsoft.Json.Formatting.None));
    }
}