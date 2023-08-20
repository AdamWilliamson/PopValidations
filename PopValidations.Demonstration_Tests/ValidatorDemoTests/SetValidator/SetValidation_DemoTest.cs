using ApprovalTests;
using Newtonsoft.Json.Linq;
using PopValidations.Swashbuckle_Tests.Helpers;
using PopValidations_Tests.TestHelpers;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PopValidations.Demonstration_Tests.ValidatorDemoTests.SetValidator;

public static class SetValidator
{
    public record InputObject(ChildInputObject? Child);
    public record ChildInputObject(string? NString);

    public class Validator : AbstractValidator<InputObject>
    {
        public Validator()
        {
            Describe(x => x.Child)
                .SetValidator(new ChildValidator());
        }
    }

    public class ChildValidator : AbstractSubValidator<ChildInputObject>
    {
        public ChildValidator()
        {
            Describe(x => x.NString)
                .IsNotNull();
        }
    }

    public class TestController : ControllerBase<InputObject> { }
}

public class SetValidation_DemoTest
{
    [Fact]
    public async Task Validation()
    {
        // Arrange
        var validationRunner = ValidationRunnerHelper.BasicRunnerSetup(new SetValidator.Validator());

        // Act
        var validationResult = await validationRunner.Validate(
            new SetValidator.InputObject(Child: new SetValidator.ChildInputObject(NString: null))
        );

        // Assert
        Approvals.VerifyJson(JsonConverter.ToJson(validationResult));
    }

    [Fact]
    public void Description()
    {
        // Arrange
        var descriptionRunner = ValidationRunnerHelper.BasicRunnerSetup(new SetValidator.Validator());

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
               SetValidator.TestController,
               SetValidator.Validator,
               SetValidator.InputObject
           >();

        // Act
        var helper = await setup.GetHelper(config);
        JObject json = JObject.Parse(helper.Content);
        var match = json["components"]["schemas"].FirstOrDefault();

        // Assert
        Approvals.VerifyJson(match?.ToString(Newtonsoft.Json.Formatting.None));
    }
}