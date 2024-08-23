using ApprovalTests;
using FluentAssertions;
using ApiValidations;
using ApiValidations_Tests.TestHelpers;

namespace PopValidations_Tests.ReturnTests.IsCustomValidationTests;

public class IsCustomApi
{
    public int IntReturnNoParams() { return int.MaxValue; }
    public decimal DecimalReturnNoParams() { return decimal.MaxValue; }
    public string StringReturnNoParams() { return "zzzzzz"; }
}

public class IsCustom_TestingValidator : ApiValidator<IsCustomApi>
{
    public IsCustom_TestingValidator()
    {
        DescribeFunc(x => x.IntReturnNoParams()).Return.Is("Must be min value.", "Is not min value.", x => x == int.MinValue);
        DescribeFunc(x => x.DecimalReturnNoParams()).Return.Is("Must be min value.", "Is not min value.", x => x == decimal.MinValue);
        DescribeFunc(x => x.StringReturnNoParams()).Return.Is("Must be aaaaa.", "Is not aaaaa.", x => x == "aaaaa");
    }
}

public class IsCustomValidation_RunnerTests
{
    [Fact]
    public void WhenDescribing_ItReturnsTheValidation()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsCustom_TestingValidator());

        // Act
        var description = runner.Describe();

        // Assert
        description.Results.Should().HaveCount(3);
        description.Results.Should().HaveCount(ValidatableHelper.GetValidatableCount<IsCustomApi>(ValidatableType.NoExceptions));
        Approvals.VerifyJson(JsonConverter.ToJson(description));
    }
}
