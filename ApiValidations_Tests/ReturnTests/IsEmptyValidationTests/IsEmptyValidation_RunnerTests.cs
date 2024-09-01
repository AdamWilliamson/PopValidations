using ApprovalTests;
using FluentAssertions;
using ApiValidations_Tests.TestHelpers;
using ApiValidations;

namespace ApiValidations_Tests.ReturnTests.IsEmptyValidationTests.IsEmptyValidationTests;

public class EmptyApi
{
    public string GetEmpty() { return string.Empty; }
    public string GetNotEmpty() { return "Definitely not an empty string"; }
}

public class IsEmpty_TestingValidator : ApiValidator<EmptyApi>
{
    public IsEmpty_TestingValidator()
    {
        DescribeFunc(x => x.GetEmpty()).Return.IsEmpty();
        DescribeFunc(x => x.GetNotEmpty()).Return.IsEmpty();
    }
}

public class IsEmptyValidation_RunnerTests
{
    [Fact]
    public void WhenDescribing_ItReturnsTheValidation()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsEmpty_TestingValidator());

        // Act
        var description = runner.Describe();

        // Assert
        description.Results.Should().HaveCount(2);
        description.Results.Should().HaveCount(ValidatableHelper.GetValidatableCount<EmptyApi>(ValidatableType.NoExceptions));
        Approvals.VerifyJson(JsonConverter.ToJson(description));
    }

    [Fact]
    public async Task WhenValidating_ItReturnsTheValidation()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsEmpty_TestingValidator());

        // Act
        var validation = await runner.ValidateAndExecute(
            new EmptyApi(),
            new ApiValidations.Execution.HeirarchyMethodInfo(
                string.Empty,
                typeof(EmptyApi).GetMethod(nameof(EmptyApi.GetNotEmpty))!,
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
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsEmpty_TestingValidator());

        // Act
        var validation = await runner.ValidateAndExecute(
            new EmptyApi(),
            new ApiValidations.Execution.HeirarchyMethodInfo(
                string.Empty,
                typeof(EmptyApi).GetMethod(nameof(EmptyApi.GetEmpty))!,
                []
            )
        );

        // Assert
        validation.Errors.Should().HaveCount(0);
    }
}
