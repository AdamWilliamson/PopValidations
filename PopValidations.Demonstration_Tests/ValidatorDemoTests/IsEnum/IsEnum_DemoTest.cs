using ApprovalTests;
using Newtonsoft.Json.Linq;
using PopValidations.Swashbuckle_Tests.Helpers;
using PopValidations_Tests.TestHelpers;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PopValidations.Demonstration_Tests.ValidatorDemoTests.IsEnum;

public static class IsEnum
{
    //Begin-Request
    public enum TestEnumeration
    {
        Value1 = 1,
        Value2 = 2
    }

    public record InputObject(string? NString, int? NInt, double? NDouble);
    //End-Request

    //Begin-Validator
    public class Validator : AbstractValidator<InputObject>
    {
        public Validator()
        {
            Describe(x => x.NString).IsEnum(typeof(TestEnumeration));
            Describe(x => x.NInt).IsEnum(typeof(TestEnumeration));
            Describe(x => x.NDouble).IsEnum(typeof(TestEnumeration));
        }
    }
    //End-Validator

    public class TestController : ControllerBase<InputObject> { }
}

public class IsEnum_DemoTest
{
    [Fact]
    public async Task Validation()
    {
        // Arrange
        var validationRunner = ValidationRunnerHelper.BasicRunnerSetup(new IsEnum.Validator());

        // Act
        var validationResult = await validationRunner.Validate(
            new IsEnum.InputObject(NString: "3", NInt: 3, NDouble: 3.3)
        );

        // Assert
        Approvals.VerifyJson(JsonConverter.ToJson(validationResult));
    }

    [Fact]
    public void Description()
    {
        // Arrange
        var descriptionRunner = ValidationRunnerHelper.BasicRunnerSetup(new IsEnum.Validator());

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
               IsEnum.TestController,
               IsEnum.Validator,
               IsEnum.InputObject
           >();

        // Act
        var helper = await setup.GetHelper(config);
        JObject json = JObject.Parse(helper.Content!);
        var match = json["components"]!["schemas"]!.FirstOrDefault();

        // Assert
        Approvals.VerifyJson(match?.ToString(Newtonsoft.Json.Formatting.None));
    }
}