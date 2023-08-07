using ApprovalTests;
using ApprovalTests.Namers;
using FluentAssertions;
using FluentAssertions.Execution;
using PopValidations;
using PopValidations_Tests.TestHelpers;
using PopValidations_Tests.ValidationsTests.GenericTestableObjects;
using System.Threading.Tasks;
using Xunit;

namespace PopValidations_Tests.ValidationsTests.NotNullValidationTests.NotNullValidationTests;

#region Object
public record NotNullInputObject(int? NInteger);
#endregion

#region Validator
public class NotNullInputObject_Validator : AbstractValidator<NotNullInputObject>
{
    public NotNullInputObject_Validator()
    {
        Describe(x => x.NInteger).IsNotNull();
    }
}
#endregion

#region Test
public class NotNull_DemoTest
{
    [Fact]
    public async Task GivenAValidator_WithErrors_ThenEveryFieldHasErrors()
    {
        // Arrange
        var validationRunner = ValidationRunnerHelper.BasicRunnerSetup(new NotNullInputObject_Validator());
        var descriptionRunner = ValidationRunnerHelper.BasicRunnerSetup(new NotNullInputObject_Validator());

        // Act
        var validationResult = await validationRunner.Validate(
            new NotNullInputObject(NInteger: null)
        );

        var descriptionResult = descriptionRunner.Describe();

        // Assert
        using (new AssertionScope())
        {
            using (ApprovalResults.ForScenario("Validation"))
            {
                Approvals.VerifyJson(JsonConverter.ToJson(validationResult));
            }

            using (ApprovalResults.ForScenario("Description"))
            {
                Approvals.VerifyJson(JsonConverter.ToJson(descriptionResult));
            }
        }
    }
}
#endregion