using ApprovalTests;
using Newtonsoft.Json.Linq;
using PopValidations.Swashbuckle_Tests.Helpers;
using PopValidations_Tests.TestHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PopValidations.Demonstration_Tests.ValidatorDemoTests.Switch;

public static class Switch
{
    //Begin-Request
    public record Level1(bool Bool, string? NString);
    //End-Request

    //Begin-Validator
    public record ValidationResults(bool TestCheck1, bool TestCheck2);

    public static class DataRetriever
    {
        public static Task<ValidationResults> GetValidations(Level1 v)
        {
            return Task.FromResult(
                new ValidationResults(v.Bool, v.NString == null)
            );
        }
    }

    public class Validator : AbstractValidator<Level1>
    {
        public Validator()
        {
            Switch("ScopedData", x => DataRetriever.GetValidations(x))
                // Ignore is purely decorative, to show you've considered specific outcomes,
                // and are meaning to ignore them.
                .Ignore(
                    "Both Checks Are False",
                    (x, data) => data is { TestCheck1: false, TestCheck2: false })
                // For the specified field, show this description, and if this check fails,
                // show this error.
                .Case(
                    x => x.Bool, 
                    "Check 1 and Check 2 should be true", 
                    (x, data) => data is { TestCheck1: true, TestCheck2: true }, 
                    "TestCheck1 and TestCheck2 were not true")
                .Case(
                    x => x.Bool,
                    "Check 1 must be true, but check 2 is false",
                    (x, data) => data is { TestCheck1: true, TestCheck2: false },
                    "TestCheck1 must be true and TestCheck2 must not be true");
        }
    }
    //End-Validator

    public class TestController : ControllerBase<Level1> { }
}

public class Switch_DemoTest
{
    [Fact]
    public async Task Validation()
    {
        string? finalFile = null;

        foreach(var setup in new List<(bool, string?)>() {
            (false, "Not Empty"),
            (true, null),
            (true, "Not Empty")
        })
        {
            // Arrange
            var validationRunner = ValidationRunnerHelper.BasicRunnerSetup(new Switch.Validator());

            // Act
            var validationResult = await validationRunner.Validate(
                new Switch.Level1(
                    Bool: setup.Item1,
                    NString: setup.Item2
                )
            );

            finalFile = string.Join(Environment.NewLine + "OR" + Environment.NewLine, new[] { finalFile, JsonConverter.ToJson(validationResult) }.Where(x => x is not null));
        }
        
        // Assert
        Approvals.VerifyJson(finalFile);
    }

    [Fact]
    public void Description()
    {
        // Arrange
        var descriptionRunner = ValidationRunnerHelper.BasicRunnerSetup(new Switch.Validator());

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
        var setup = new TestSetup<Switch.TestController, Switch.Validator, Switch.Level1>();

        // Act
        var helper = await setup.GetHelper(config);
        JObject json = JObject.Parse(helper.Content);
        var match = json["components"]["schemas"].FirstOrDefault();

        // Assert
        Approvals.VerifyJson(match?.ToString(Newtonsoft.Json.Formatting.None));
    }
}
