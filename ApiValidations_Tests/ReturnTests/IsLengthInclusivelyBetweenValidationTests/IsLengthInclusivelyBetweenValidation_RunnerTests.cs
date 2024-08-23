using ApprovalTests;
using FluentAssertions;
using ApiValidations;
using ApiValidations_Tests.TestHelpers;

namespace ApiValidations_Tests.ReturnTests.IsLengthInclusivelyBetweenValidationTests;

public class IsLengthInclusivelyBetween
{
    public string Get() { return string.Empty; }
}

public class IsLengthExclusivelyBetween_TestingValidator : ApiValidator<IsLengthInclusivelyBetween>
{
    public IsLengthExclusivelyBetween_TestingValidator()
    {
        DescribeFunc(x => x.Get()).Return.IsLengthInclusivelyBetween(-1, 1);
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
        description.Results.Should().HaveCount(1);
        description.Results.Should().HaveCount(ValidatableHelper.GetValidatableCount<IsLengthInclusivelyBetween>(ValidatableType.NoExceptions));
        Approvals.VerifyJson(JsonConverter.ToJson(description));
    }
}
