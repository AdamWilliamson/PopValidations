using ApprovalTests;
using FluentAssertions;
using ApiValidations;
using ApiValidations_Tests.TestHelpers;
using System.Collections;

namespace ApiValidations_Tests.ReturnTests.IsLengthExclusivelyBetweenValidationTests;

public class IsLengthExclusivelyBetweenApi
{
    public IEnumerable Get() { return new List<int>(); }
}

public class IsLengthExclusivelyBetween_TestingValidator : ApiValidator<IsLengthExclusivelyBetweenApi>
{
    public IsLengthExclusivelyBetween_TestingValidator()
    {
        DescribeFunc(x => x.Get()).Return.IsLengthExclusivelyBetween(-1, 1);
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
        description.Results.Should().HaveCount(1);
        description.Results.Should().HaveCount(ValidatableHelper.GetValidatableCount<IsLengthExclusivelyBetweenApi>(ValidatableType.NoExceptions));
        Approvals.VerifyJson(JsonConverter.ToJson(description));
    }
}
