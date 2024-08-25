using ApprovalTests;
using FluentAssertions;
using ApiValidations;
using ApiValidations_Tests.TestHelpers;
using System.Collections;

namespace ApiValidations_Tests.ReturnTests.IsLengthExclusivelyBetweenValidationTests;

public class IsLengthExclusivelyBetweenApi
{
    public IEnumerable Get() { return new List<int>(); }
    public IEnumerable Get_Invalid() { return new List<int>(); }
}

public class IsLengthExclusivelyBetween_TestingValidator : ApiValidator<IsLengthExclusivelyBetweenApi>
{
    public IsLengthExclusivelyBetween_TestingValidator()
    {
        DescribeFunc(x => x.Get()).Return.IsLengthExclusivelyBetween(-1, 1);
        DescribeFunc(x => x.Get_Invalid()).Return.IsLengthExclusivelyBetween(10, 12);
    }
}

public class IsLengthExclusivelyBetweenValidation_RunnerTests2
{
    [Fact]
    public void WhenDescribing_ItReturnsTheValidation()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(
            new IsLengthExclusivelyBetween_TestingValidator()
        );

        // Act
        var description = runner.Describe();

        // Assert
        description.Results.Should().HaveCount(2);
        description.Results.Should().HaveCount(ValidatableHelper.GetValidatableCount<IsLengthExclusivelyBetweenApi>(ValidatableType.NoExceptions));
        Approvals.VerifyJson(JsonConverter.ToJson(description));
    }

    [Fact]
    public async Task WhenValidating_ItReturnsTheValidation()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsLengthExclusivelyBetween_TestingValidator());

        // Act
        var validation = await runner.Validate(
            new IsLengthExclusivelyBetweenApi(),
            new ApiValidations.Execution.HeirarchyMethodInfo(
                string.Empty,
                typeof(IsLengthExclusivelyBetweenApi).GetMethod(nameof(IsLengthExclusivelyBetweenApi.Get_Invalid))!,
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
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsLengthExclusivelyBetween_TestingValidator());

        // Act
        var validation = await runner.Validate(
            new IsLengthExclusivelyBetweenApi(),
            new ApiValidations.Execution.HeirarchyMethodInfo(
                string.Empty,
                typeof(IsLengthExclusivelyBetweenApi).GetMethod(nameof(IsLengthExclusivelyBetweenApi.Get))!,
                []
            )
        );

        // Assert
        validation.Errors.Should().HaveCount(0);
    }
}
