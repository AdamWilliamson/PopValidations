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
    public record Base(string? DependantField);
    public record Level1(bool Check, string? DependantField, Level2? Child) : Base(DependantField);
    public record Level2(bool Check, string? DependantField, Level3? Child) : Base(DependantField);
    public record Level3(bool Check, string? DependantField, Level4? Child) : Base(DependantField);
    public record Level4(bool Check, string? DependantField, Level5? Child) : Base(DependantField);
    public record Level5(bool Check, string? DependantField);

    public static class DataRetriever
    {
        public static Task<string> GetValue(Base v) 
        { 
            return Task.FromResult(v?.DependantField + " GetValue"); 
        }
        public static Task<string> GetMoreValue(Base v) 
        { 
            return Task.FromResult(v?.DependantField + " GetMoreValue"); 
        }
    }

    public class Validator : AbstractValidator<Level1>
    {
        public Validator()
        {
            Include(new SubValidator());
        }
    }

    public class SubValidator : AbstractSubValidator<Level1>
    {
        public SubValidator()
        {
            Describe(x => x.DependantField).IsEqualTo("0");

            ScopeWhen(
                "When Check is True",
                x => Task.FromResult(x.Child != null),
                "Database Value",
                (x) => DataRetriever.GetValue(x),
                (retrievedData) =>
                {
                    Describe(x => x.DependantField).IsEqualTo(retrievedData);

                    ScopeWhen(
                        "When 10 == 10",
                        x => Task.FromResult(x.Check == true),
                        "Another DB Value",
                        (x) => DataRetriever.GetMoreValue(x),
                        (moreData) =>
                        {
                            Describe(x => x.DependantField).IsEqualTo(moreData);
                        }
                    );

                    //Describe(x => x.Child).SetValidator(new SubValidator());
                }
            );
        }
    }

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
                DependantField: "1", 
                Child: new ScopeWhen.Level2(
                    Check: true,
                    DependantField: "2",
                    Child: null
                )
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
        JObject json = JObject.Parse(helper.Content);
        var match = json["components"]["schemas"].FirstOrDefault();

        // Assert
        Approvals.VerifyJson(match?.ToString(Newtonsoft.Json.Formatting.None));
    }
}
