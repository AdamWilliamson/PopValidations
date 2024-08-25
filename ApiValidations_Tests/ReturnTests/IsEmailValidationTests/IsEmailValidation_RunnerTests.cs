using ApiValidations;
using ApiValidations_Tests.TestHelpers;
using ApprovalTests;
using FluentAssertions;
using PopValidations;

namespace ApiValidations_Tests.ReturnTests.IsEmailValidationTests;

public class EmailApi
{
    public string GetInvalidEmail() { return string.Empty; }
    public string GetValidEmail() { return "Test@Testcase.com.au"; }
}

public class IsEmail_TestingValidator : ApiValidator<EmailApi>
{
    public IsEmail_TestingValidator()
    {
        DescribeFunc(x => x.GetInvalidEmail()).Return.IsEmail();
        DescribeFunc(x => x.GetValidEmail()).Return.IsEmail();
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
        description.Results.Should().HaveCount(2);
        Approvals.VerifyJson(JsonConverter.ToJson(description));
    }

    [Fact]
    public async Task WhenValidating_ItReturnsTheValidation()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsEmail_TestingValidator());

        // Act
        var validation = await runner.Validate(
            new EmailApi(),
            new ApiValidations.Execution.HeirarchyMethodInfo(
                string.Empty,
                typeof(EmailApi).GetMethod(nameof(EmailApi.GetInvalidEmail))!,
                []
            )
        );

        // Assert
        validation.Errors.Should().HaveCount(1);
        Approvals.VerifyJson(JsonConverter.ToJson(validation));
    }

    [Fact]
    public async Task WhenValidating_ItIsSuccessful()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsEmail_TestingValidator());

        // Act
        var validation = await runner.Validate(
            new EmailApi(),
            new ApiValidations.Execution.HeirarchyMethodInfo(
                string.Empty,
                typeof(EmailApi).GetMethod(nameof(EmailApi.GetValidEmail))!,
                []
            )
        );

        // Assert
        validation.Errors.Should().HaveCount(0);
    }
}