using ApprovalTests;
using Newtonsoft.Json.Linq;
using PopValidations.Swashbuckle_Tests.Helpers;
using PopValidations_Tests.TestHelpers;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PopValidations.Demonstration_Tests.ValidatorDemoTests.Include;

public static class Include
{
    public record InputObject(string? NString);

    public class Validator : AbstractValidator<InputObject>
    {
        public Validator()
        {
            Include(new SecondaryValidator());
        }
    }

    public class SecondaryValidator : AbstractSubValidator<InputObject>
    {
        public SecondaryValidator()
        {
            Describe(x => x.NString)
                .IsNotNull();
        }
    }

    public class TestController : ControllerBase<InputObject> { }
}

public class Include_DemoTest
{
    [Fact]
    public async Task Validation()
    {
        // Arrange
        var validationRunner = ValidationRunnerHelper.BasicRunnerSetup(new Include.Validator());

        // Act
        var validationResult = await validationRunner.Validate(
            new Include.InputObject(NString: null)
        );

        // Assert
        Approvals.VerifyJson(JsonConverter.ToJson(validationResult));
    }

    [Fact]
    public void Description()
    {
        // Arrange
        var descriptionRunner = ValidationRunnerHelper.BasicRunnerSetup(new Include.Validator());

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
               Include.TestController,
               Include.Validator,
               Include.InputObject
           >();

        // Act
        var helper = await setup.GetHelper(config);
        JObject json = JObject.Parse(helper.Content);
        var match = json["components"]["schemas"].FirstOrDefault();

        // Assert
        Approvals.VerifyJson(match?.ToString(Newtonsoft.Json.Formatting.None));
    }
}