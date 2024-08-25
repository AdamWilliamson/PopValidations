
using ApprovalTests;
using FluentAssertions;
using ApiValidations;
using ApiValidations_Tests.TestHelpers;

namespace ApiValidations_Tests.ReturnTests.IsNullValidationTests;

public class IsNullApi
{
    public int? Get1() { return null; }
    public object Get2() { return new object(); }
}

public class IsNull_TestingValidator : ApiValidator<IsNullApi>
{
    public IsNull_TestingValidator()
    {
        DescribeFunc(x => x.Get1()).Return.IsNull();

        DescribeFunc(x => x.Get2()).Return.IsNull();
    }
}

public class IsNullValidation_RunnerTests
{
    [Fact]
    public void WhenDescribing_ItReturnsTheValidation()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsNull_TestingValidator());

        // Act
        var description = runner.Describe();

        // Assert
        description.Results.Should().HaveCount(2);
        description.Results.Should().HaveCount(ValidatableHelper.GetValidatableCount<IsNullApi>(ValidatableType.NoExceptions));
        Approvals.VerifyJson(JsonConverter.ToJson(description));
    }

    [Fact]
    public async Task WhenValidating_ItReturnsTheValidation()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsNull_TestingValidator());

        // Act
        var validation = await runner.Validate(
            new IsNullApi(),
            new ApiValidations.Execution.HeirarchyMethodInfo(
                string.Empty,
                typeof(IsNullApi).GetMethod(nameof(IsNullApi.Get2))!,
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
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsNull_TestingValidator());

        // Act
        var validation = await runner.Validate(
            new IsNullApi(),
            new ApiValidations.Execution.HeirarchyMethodInfo(
                string.Empty,
                typeof(IsNullApi).GetMethod(nameof(IsNullApi.Get1))!,
                []
            )
        );

        // Assert
        validation.Errors.Should().HaveCount(0);
    }
}
