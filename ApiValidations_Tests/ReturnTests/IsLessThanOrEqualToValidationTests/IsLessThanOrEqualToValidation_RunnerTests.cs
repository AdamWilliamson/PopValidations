using ApiValidations;
using ApiValidations_Tests.TestHelpers;
using ApprovalTests;
using FluentAssertions;

namespace ApiValidations_Tests.ReturnTests.IsLessThanOrEqualToValidationTests;

public class IsLessThanOrEqualtoApi
{
    public int GetValue() { return 1; }
    public int GetValueLessThan10() { return 11; }
}

public class IsLessThanOrEqualTo_TestingValidator : ApiValidator<IsLessThanOrEqualtoApi>
{
    public IsLessThanOrEqualTo_TestingValidator()
    {
        DescribeFunc(x => x.GetValue()).Return.IsLessThanOrEqualTo(int.MaxValue);
        DescribeFunc(x => x.GetValueLessThan10()).Return.IsLessThanOrEqualTo(10);
    }
}

public class IsLessThanOrEqualToValidation_RunnerTests
{
    [Fact]
    public void WhenDescribing_ItReturnsTheValidation()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsLessThanOrEqualTo_TestingValidator());

        // Act
        var description = runner.Describe();

        // Assert
        description.Results.Should().HaveCount(2);
        description.Results.Should().HaveCount(ValidatableHelper.GetValidatableCount<IsLessThanOrEqualtoApi>(ValidatableType.NoExceptions));
        Approvals.VerifyJson(JsonConverter.ToJson(description));
    }

    [Fact]
    public async Task WhenValidating_ItReturnsTheValidation()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsLessThanOrEqualTo_TestingValidator());

        // Act
        var validation = await runner.Validate(
            new IsLessThanOrEqualtoApi(),
            new ApiValidations.Execution.HeirarchyMethodInfo(
                string.Empty,
                typeof(IsLessThanOrEqualtoApi).GetMethod(nameof(IsLessThanOrEqualtoApi.GetValueLessThan10))!,
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
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsLessThanOrEqualTo_TestingValidator());

        // Act
        var validation = await runner.Validate(
            new IsLessThanOrEqualtoApi(),
            new ApiValidations.Execution.HeirarchyMethodInfo(
                string.Empty,
                typeof(IsLessThanOrEqualtoApi).GetMethod(nameof(IsLessThanOrEqualtoApi.GetValue))!,
                []
            )
        );

        // Assert
        validation.Errors.Should().HaveCount(0);
    }
}
