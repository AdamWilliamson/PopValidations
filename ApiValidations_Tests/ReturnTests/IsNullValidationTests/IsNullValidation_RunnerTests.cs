
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
}
