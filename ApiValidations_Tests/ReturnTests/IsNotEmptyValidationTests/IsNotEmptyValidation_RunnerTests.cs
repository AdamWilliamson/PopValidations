using ApprovalTests;
using FluentAssertions;
using ApiValidations;
using ApiValidations_Tests.TestHelpers;

namespace ApiValidations_Tests.ReturnTests.IsNotEmptyValidationTests;

public class NotEmptyApi 
{
    public decimal? Get1() { return null; }
    public string? Get2() { return null; }
}

public class NotEmpty_TestingValidator : ApiValidator<NotEmptyApi>
{
    public NotEmpty_TestingValidator()
    {
        DescribeFunc(x => x.Get1()).Return.IsNotEmpty();

        DescribeFunc(x => x.Get2()).Return.IsNotEmpty();
    }
}

public class IsNotEmptyValidation_RunnerTests
{
    [Fact]
    public void WhenDescribing_ItReturnsTheValidation()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new NotEmpty_TestingValidator());

        // Act
        var description = runner.Describe();

        // Assert
        description.Results.Should().HaveCount(2);
        description.Results.Should().HaveCount(ValidatableHelper.GetValidatableCount<NotEmptyApi>(ValidatableType.NoExceptions));
        Approvals.VerifyJson(JsonConverter.ToJson(description));
    }
}
