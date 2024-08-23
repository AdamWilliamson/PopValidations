using ApprovalTests;
using FluentAssertions;
using ApiValidations_Tests.TestHelpers;
using ApiValidations;

namespace ApiValidations_Tests.ReturnTests.IsEmptyValidationTests.IsEmptyValidationTests;

public class EmptyApi
{
    public string ReturnMustBeEmpty() { return string.Empty; }
}

public class IsEmpty_TestingValidator : ApiValidator<EmptyApi>
{
    public IsEmpty_TestingValidator()
    {
        DescribeFunc(x => x.ReturnMustBeEmpty()).Return.IsEmpty();
    }
}

public class IsEmptyValidation_RunnerTests
{
    [Fact]
    public void WhenDescribing_ItReturnsTheValidation()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsEmpty_TestingValidator());

        // Act
        var description = runner.Describe();

        // Assert
        description.Results.Should().HaveCount(1);
        description.Results.Should().HaveCount(ValidatableHelper.GetValidatableCount<EmptyApi>(ValidatableType.NoExceptions));
        Approvals.VerifyJson(JsonConverter.ToJson(description));
    }
}
