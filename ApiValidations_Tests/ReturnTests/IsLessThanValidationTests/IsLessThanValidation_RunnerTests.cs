using ApprovalTests;
using FluentAssertions;
using ApiValidations;
using ApiValidations_Tests.TestHelpers;

namespace PopValidations_Tests.returnTests.IsLessThanValidationTests;

public class IsLessThanApi
{
    public int Set() { return 1; }
}

public class IsLessThan_TestingValidator : ApiValidator<IsLessThanApi>
{
    public IsLessThan_TestingValidator()
    {
        DescribeFunc(x => x.Set()).Return.IsLessThan(int.MaxValue);
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
        description.Results.Should().HaveCount(1);
        description.Results.Should().HaveCount(ValidatableHelper.GetValidatableCount<IsLessThanApi>(ValidatableType.NoExceptions));
        Approvals.VerifyJson(JsonConverter.ToJson(description));
    }
}
