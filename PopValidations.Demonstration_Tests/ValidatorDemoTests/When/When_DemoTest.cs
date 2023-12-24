using ApprovalTests;
using Newtonsoft.Json.Linq;
using PopValidations.Swashbuckle_Tests.Helpers;
using PopValidations_Tests.TestHelpers;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PopValidations.Demonstration_Tests.ValidatorDemoTests.When;

public static class When
{
    //Begin-Request
    public record InputObject(bool Check, string? DependantField);
    //End-Request

    //Begin-Validator
    public class Validator : AbstractValidator<InputObject>
    {
        public Validator()
        {
            When(
                "When Check is True",
                x => Task.FromResult(x.Check == true),
                () =>
                {
                    When(
                        "When 10 == 10",
                        x => Task.FromResult(true),
                        () =>
                        {
                            Describe(x => x.DependantField).IsEqualTo("Test1");
                        }
                    );

                    // All other validations won't execute if this fails.
                    Describe(x => x.DependantField).Vitally().IsNotEmpty();

                    When(
                        "When 5 == 5",
                        x => Task.FromResult(true),
                        () =>
                        {
                            Describe(x => x.DependantField).IsEqualTo("Test2");
                        }
                    );
                }
            );

            Describe(x => x.DependantField).IsEqualTo("Test3");
        }
    }
    //End-Validator

    public class TestController : ControllerBase<InputObject> { }
}

public class When_DemoTest
{
    [Fact]
    public async Task Validation()
    {
        // Arrange
        var validationRunner = ValidationRunnerHelper.BasicRunnerSetup(new When.Validator());

        // Act
        var validationResult = await validationRunner.Validate(
            new When.InputObject(true, DependantField: "")
        );

        // Assert
        Approvals.VerifyJson(JsonConverter.ToJson(validationResult));
    }

    [Fact]
    public void Description()
    {
        // Arrange
        var descriptionRunner = ValidationRunnerHelper.BasicRunnerSetup(new When.Validator());

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
        var setup = new TestSetup<When.TestController, When.Validator, When.InputObject>();

        // Act
        var helper = await setup.GetHelper(config);
        JObject json = JObject.Parse(helper.Content!);
        var match = json["components"]!["schemas"]!.FirstOrDefault();

        // Assert
        Approvals.VerifyJson(match?.ToString(Newtonsoft.Json.Formatting.None));
    }
}
