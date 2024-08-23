using ApiValidations;
using ApiValidations_Tests.TestHelpers;
using ApprovalTests;
using FluentAssertions;

namespace ApiValidations_Tests.ReturnTests.IsLessThanOrEqualToValidationTests;

public class IsLessThanOrEqualtoApi
{
    public int GetValue() { return 1; }
}

public class IsLessThanOrEqualTo_TestingValidator : ApiValidator<IsLessThanOrEqualtoApi>
{
    public IsLessThanOrEqualTo_TestingValidator()
    {
        DescribeFunc(x => x.GetValue()).Return.IsLessThanOrEqualTo(long.MaxValue);
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
        description.Results.Should().HaveCount(1);
        description.Results.Should().HaveCount(ValidatableHelper.GetValidatableCount<IsLessThanOrEqualtoApi>(ValidatableType.NoExceptions));
        Approvals.VerifyJson(JsonConverter.ToJson(description));
    }
}
