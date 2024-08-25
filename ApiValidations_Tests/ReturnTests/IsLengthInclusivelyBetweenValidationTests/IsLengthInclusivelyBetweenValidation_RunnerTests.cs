using ApprovalTests;
using FluentAssertions;
using ApiValidations;
using ApiValidations_Tests.TestHelpers;
using System.Collections;

namespace ApiValidations_Tests.ReturnTests.IsLengthInclusivelyBetweenValidationTests;

public class IsLengthInclusivelyBetweenApi
{
    public string Get() { return string.Empty; }
    public IEnumerable Get_Invalid() { return new List<int>(); }
}

public class IsLengthExclusivelyBetween_TestingValidator : ApiValidator<IsLengthInclusivelyBetweenApi>
{
    public IsLengthExclusivelyBetween_TestingValidator()
    {
        DescribeFunc(x => x.Get()).Return.IsLengthInclusivelyBetween(-1, 1);
        DescribeFunc(x => x.Get_Invalid()).Return.IsLengthExclusivelyBetween(10, 12);
    }
}

public class IsLengthInclusivelyBetweenValidation_RunnerTests
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
        description.Results.Should().HaveCount(ValidatableHelper.GetValidatableCount<IsLengthInclusivelyBetweenApi>(ValidatableType.NoExceptions));
        Approvals.VerifyJson(JsonConverter.ToJson(description));
    }

    [Fact]
    public async Task WhenValidating_ItReturnsTheValidation()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsLengthExclusivelyBetween_TestingValidator());

        // Act
        var validation = await runner.Validate(
            new IsLengthInclusivelyBetweenApi(),
            new ApiValidations.Execution.HeirarchyMethodInfo(
                string.Empty,
                typeof(IsLengthInclusivelyBetweenApi).GetMethod(nameof(IsLengthInclusivelyBetweenApi.Get_Invalid))!,
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
            new IsLengthInclusivelyBetweenApi(),
            new ApiValidations.Execution.HeirarchyMethodInfo(
                string.Empty,
                typeof(IsLengthInclusivelyBetweenApi).GetMethod(nameof(IsLengthInclusivelyBetweenApi.Get))!,
                []
            )
        );

        // Assert
        validation.Errors.Should().HaveCount(0);
    }
}
