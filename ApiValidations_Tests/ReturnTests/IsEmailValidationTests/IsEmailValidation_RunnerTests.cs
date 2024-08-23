using ApiValidations;
using ApiValidations_Tests.TestHelpers;
using ApprovalTests;
using FluentAssertions;
using PopValidations;

namespace ApiValidations_Tests.ReturnTests.IsEmailValidationTests;

public class EmailApi
{
    public string GetEmail() { return string.Empty; }
}

public class IsEmail_TestingValidator : ApiValidator<EmailApi>
{
    public IsEmail_TestingValidator()
    {
        DescribeFunc(x => x.GetEmail()).Return.IsEmail();
    }
}

public class IsEmailValidation_RunnerTests
{
    [Fact]
    public void WhenDescribing_ItReturnsTheValidation()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsEmail_TestingValidator());

        // Act
        var description = runner.Describe();

        // Assert
        description.Results.Should().HaveCount(1);
        description.Results.Should().HaveCount(ValidatableHelper.GetValidatableCount<EmailApi>(ValidatableType.NoExceptions));
        Approvals.VerifyJson(JsonConverter.ToJson(description));
    }
}