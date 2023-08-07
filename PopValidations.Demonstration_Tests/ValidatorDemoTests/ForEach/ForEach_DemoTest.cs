using ApprovalTests;
using Newtonsoft.Json.Linq;
using PopValidations.Swashbuckle_Tests.Helpers;
using PopValidations_Tests.TestHelpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PopValidations.Demonstration_Tests.ValidatorDemoTests.ForEach;

public static class ForEach
{
    public record InputObject(List<string?> ArrayOfStrings);

    public class Validator : AbstractValidator<InputObject>
    {
        public Validator()
        {
            // If ArrayOfStrings is Not Null, then Check each item
            DescribeEnumerable(x => x.ArrayOfStrings)
                .Vitally().IsNotNull()
                .ForEach(x => x
                    .Vitally().IsNotNull()
                    .IsNotEmpty() // Only run if Not Null
                );

            // Tests each item, but stops at the first item that fails. this effects the previous ForEach.
            // This won't run the Tests if ArrayOfStrings is null, due to the previous .Vitally().IsNotNull() on this field
            DescribeEnumerable(x => x.ArrayOfStrings)
                .Vitally().ForEach(x => x
                    .IsEqualTo("Test")
                );
        }
    }

    public class TestController : ControllerBase<InputObject> { }
}

public class ForEach_DemoTest
{
    [Fact]
    public async Task Validation()
    {
        // Arrange
        var validationRunner = ValidationRunnerHelper.BasicRunnerSetup(new ForEach.Validator());

        // Act
        var validationResult = await validationRunner.Validate(
            new ForEach.InputObject(ArrayOfStrings: new() { null, null })
        );

        // Assert
        Approvals.VerifyJson(JsonConverter.ToJson(validationResult));
    }

    [Fact]
    public void Description()
    {
        // Arrange
        var descriptionRunner = ValidationRunnerHelper.BasicRunnerSetup(new ForEach.Validator());

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
               ForEach.TestController,
               ForEach.Validator,
               ForEach.InputObject
           >();

        // Act
        var helper = await setup.GetHelper(config);
        JObject json = JObject.Parse(helper.Content);
        var match = json["components"]["schemas"].FirstOrDefault();

        // Assert
        Approvals.VerifyJson(match?.ToString(Newtonsoft.Json.Formatting.None));
    }
}