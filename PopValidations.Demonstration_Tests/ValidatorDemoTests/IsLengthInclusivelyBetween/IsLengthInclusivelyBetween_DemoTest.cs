using ApprovalTests;
using Newtonsoft.Json.Linq;
using PopValidations.Swashbuckle_Tests.Helpers;
using PopValidations_Tests.TestHelpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PopValidations.Demonstration_Tests.ValidatorDemoTests.IsLengthInclusivelyBetween;

public static class IsLengthInclusivelyBetween
{
    public record InputObject(string? NString, List<int> Array);

    public class Validator : AbstractValidator<InputObject>
    {
        public Validator()
        {
            Describe(x => x.NString).IsLengthInclusivelyBetween(1,5);
            Describe(x => x.Array).IsLengthInclusivelyBetween(1, 5);
        }
    }

    public class TestController : ControllerBase<InputObject> { }
}

public class IsLengthInclusivelyBetween_DemoTest
{
    [Fact]
    public async Task Validation()
    {
        // Arrange
        var validationRunner = ValidationRunnerHelper.BasicRunnerSetup(new IsLengthInclusivelyBetween.Validator());

        // Act
        var validationResult = await validationRunner.Validate(
            new IsLengthInclusivelyBetween.InputObject(NString: "6Chars", Array: new() { })
        );

        // Assert
        Approvals.VerifyJson(JsonConverter.ToJson(validationResult));
    }

    [Fact]
    public void Description()
    {
        // Arrange
        var descriptionRunner = ValidationRunnerHelper.BasicRunnerSetup(new IsLengthInclusivelyBetween.Validator());

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
               IsLengthInclusivelyBetween.TestController,
               IsLengthInclusivelyBetween.Validator,
               IsLengthInclusivelyBetween.InputObject
           >();

        // Act
        var helper = await setup.GetHelper(config);
        JObject json = JObject.Parse(helper.Content);
        var match = json["components"]["schemas"].FirstOrDefault();

        // Assert
        Approvals.VerifyJson(match?.ToString(Newtonsoft.Json.Formatting.None));
    }
}