using ApprovalTests;
using Newtonsoft.Json.Linq;
using PopValidations.Swashbuckle_Tests.Helpers;
using PopValidations_Tests.TestHelpers;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PopValidations.Demonstration_Tests.ValidatorDemoTests.IsGreaterThanOrEqualTo;

public static class IsGreaterThanOrEqualTo
{
    //Begin-Request
    public record InputObject(int? NInteger);
    //End-Request

    //Begin-Validator
    public class Validator : AbstractValidator<InputObject>
    {
        public Validator()
        {
            Describe(x => x.NInteger).IsGreaterThanOrEqualTo(5);
        }
    }
    //End-Validator

    public class TestController : ControllerBase<InputObject> { }
}

public class IsGreaterThanOrEqualTo_DemoTest
{
    [Fact]
    public async Task Validation()
    {
        // Arrange
        var validationRunner = ValidationRunnerHelper.BasicRunnerSetup(new IsGreaterThanOrEqualTo.Validator());

        // Act
        var validationResult = await validationRunner.Validate(
            new IsGreaterThanOrEqualTo.InputObject(NInteger: 1)
        );

        // Assert
        Approvals.VerifyJson(JsonConverter.ToJson(validationResult));
    }

    [Fact]
    public void Description()
    {
        // Arrange
        var descriptionRunner = ValidationRunnerHelper.BasicRunnerSetup(new IsGreaterThanOrEqualTo.Validator());

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
               IsGreaterThanOrEqualTo.TestController,
               IsGreaterThanOrEqualTo.Validator,
               IsGreaterThanOrEqualTo.InputObject
           >();

        // Act
        var helper = await setup.GetHelper(config);
        JObject json = JObject.Parse(helper.Content!);
        var match = json["components"]!["schemas"]!.FirstOrDefault();

        // Assert
        Approvals.VerifyJson(match?.ToString(Newtonsoft.Json.Formatting.None));
    }
}