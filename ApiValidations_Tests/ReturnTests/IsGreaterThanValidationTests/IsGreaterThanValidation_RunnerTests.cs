using ApprovalTests;
using FluentAssertions;
using ApiValidations;
using ApiValidations_Tests.TestHelpers;

namespace ApiValidations_Tests.ReturnTests.IsGreaterThanValidationTests;

public class IsGreaterThanApi
{
    public int Get() { return 0; }
}

public class IsGreaterThan_TestingValidator : ApiValidator<IsGreaterThanApi>
{
    public IsGreaterThan_TestingValidator()
    {
        DescribeFunc(x => x.Get()).Return.IsGreaterThan(long.MaxValue);
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
        description.Results.Should().HaveCount(1);
        description.Results.Should().HaveCount(ValidatableHelper.GetValidatableCount<IsGreaterThanApi>(ValidatableType.NoExceptions));
        Approvals.VerifyJson(JsonConverter.ToJson(description));
    }
}
