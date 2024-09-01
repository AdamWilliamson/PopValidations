using ApprovalTests;
using FluentAssertions;
using ApiValidations;
using ApiValidations_Tests.TestHelpers;

namespace ApiValidations_Tests.ReturnTests.IsGreaterThanValidationTests;

public class IsGreaterThanApi
{
    public int Get() { return 0; }
    public int GetGreaterThan0() { return 1; }
}

public class IsGreaterThan_TestingValidator : ApiValidator<IsGreaterThanApi>
{
    public IsGreaterThan_TestingValidator()
    {
        DescribeFunc(x => x.Get()).Return.IsGreaterThan(long.MaxValue);
        DescribeFunc(x => x.GetGreaterThan0()).Return.IsGreaterThan(0);
    }
}

public class IsGreaterThanValidation_RunnerTests
{
    [Fact]
    public void WhenDescribing_ItReturnsTheValidation()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsGreaterThan_TestingValidator());

        // Act
        var description = runner.Describe();

        // Assert
        description.Results.Should().HaveCount(2);
        description.Results.Should().HaveCount(ValidatableHelper.GetValidatableCount<IsGreaterThanApi>(ValidatableType.NoExceptions));
        Approvals.VerifyJson(JsonConverter.ToJson(description));
    }

    [Fact]
    public async Task WhenValidating_ItReturnsTheValidation()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsGreaterThan_TestingValidator());

        // Act
        var validation = await runner.ValidateAndExecute(
            new IsGreaterThanApi(),
            new ApiValidations.Execution.HeirarchyMethodInfo(
                string.Empty,
                typeof(IsGreaterThanApi).GetMethod(nameof(IsGreaterThanApi.Get))!,
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
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsGreaterThan_TestingValidator());

        // Act
        var validation = await runner.ValidateAndExecute(
            new IsGreaterThanApi(),
            new ApiValidations.Execution.HeirarchyMethodInfo(
                string.Empty,
                typeof(IsGreaterThanApi).GetMethod(nameof(IsGreaterThanApi.GetGreaterThan0))!,
                []
            )
        );

        // Assert
        validation.Errors.Should().HaveCount(0);
    }
}
