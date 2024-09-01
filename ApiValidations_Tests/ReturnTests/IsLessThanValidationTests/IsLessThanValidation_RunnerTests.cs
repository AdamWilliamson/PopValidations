using ApprovalTests;
using FluentAssertions;
using ApiValidations;
using ApiValidations_Tests.TestHelpers;

namespace PopValidations_Tests.returnTests.IsLessThanValidationTests;

public class IsLessThanApi
{
    public int Get() { return 1; }
    public int GetValueLessThan10() { return 11; }
}

public class IsLessThan_TestingValidator : ApiValidator<IsLessThanApi>
{
    public IsLessThan_TestingValidator()
    {
        DescribeFunc(x => x.Get()).Return.IsLessThan(int.MaxValue);
        DescribeFunc(x => x.GetValueLessThan10()).Return.IsLessThan(10);
    }
}

public class IsLessThanValidation_RunnerTests
{
    [Fact]
    public void WhenDescribing_ItReturnsTheValidation()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsLessThan_TestingValidator());

        // Act
        var description = runner.Describe();

        // Assert
        description.Results.Should().HaveCount(2);
        description.Results.Should().HaveCount(ValidatableHelper.GetValidatableCount<IsLessThanApi>(ValidatableType.NoExceptions));
        Approvals.VerifyJson(JsonConverter.ToJson(description));
    }

    [Fact]
    public async Task WhenValidating_ItReturnsTheValidation()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsLessThan_TestingValidator());

        // Act
        var validation = await runner.ValidateAndExecute(
            new IsLessThanApi(),
            new ApiValidations.Execution.HeirarchyMethodInfo(
                string.Empty,
                typeof(IsLessThanApi).GetMethod(nameof(IsLessThanApi.GetValueLessThan10))!,
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
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsLessThan_TestingValidator());

        // Act
        var validation = await runner.ValidateAndExecute(
            new IsLessThanApi(),
            new ApiValidations.Execution.HeirarchyMethodInfo(
                string.Empty,
                typeof(IsLessThanApi).GetMethod(nameof(IsLessThanApi.Get))!,
                []
            )
        );

        // Assert
        validation.Errors.Should().HaveCount(0);
    }
}
