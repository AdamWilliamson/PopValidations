using ApprovalTests;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using PopValidations;
using PopValidations_Tests.TestHelpers;
using PopValidations_Tests.ValidationsTests.GenericTestableObjects;
using Xunit;

namespace PopValidations_Tests.ValidationsTests.IsGreaterThanValidationTests;

public class IsGreaterThan_NoError_TestingValidator : AbstractValidator<NonNullAllFieldTypesDto>
{
    public IsGreaterThan_NoError_TestingValidator()
    {
        Describe(x => x.Integer).IsGreaterThan(int.MinValue);
        Describe(x => x.String).IsGreaterThan(new string(char.MinValue, 100));
        Describe(x => x.Decimal).IsGreaterThan(decimal.MinValue);
        Describe(x => x.Double).IsGreaterThan(double.MinValue);
        Describe(x => x.Short).IsGreaterThan(short.MinValue);
        Describe(x => x.Long).IsGreaterThan(long.MinValue);
    }
}

public class IsGreaterThan_AllErrored_TestingValidator : AbstractValidator<NonNullAllFieldTypesDto>
{
    public IsGreaterThan_AllErrored_TestingValidator()
    {
        Describe(x => x.Integer).IsGreaterThan(int.MaxValue);
        Describe(x => x.String).IsGreaterThan(new string(char.MaxValue, 100));
        Describe(x => x.Decimal).IsGreaterThan(decimal.MaxValue);
        Describe(x => x.Double).IsGreaterThan(double.MaxValue);
        Describe(x => x.Short).IsGreaterThan(short.MaxValue);
        Describe(x => x.Long).IsGreaterThan(long.MaxValue);
    }
}

public class IsGreaterThanValidation_RunnerTests
{
    [Fact]
    public async Task GivenAValidator_WithNoErrors_ThenNoErrorsAreProduced()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsGreaterThan_NoError_TestingValidator());

        // Act
        var validationResult = await runner.Validate(new NonNullAllFieldTypesDto());

        // Assert
        validationResult.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task GivenAValidator_WithErrors_ThenEveryFieldHasErrors()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsGreaterThan_AllErrored_TestingValidator());

        // Act
        var validationResult = await runner.Validate(new NonNullAllFieldTypesDto());
        var json = JsonConverter.ToJson(validationResult);

        // Assert
        validationResult.Errors.Should().HaveCount(6);
        Approvals.VerifyJson(json);
    }

    [Fact]
    public void GivenAValidator_WhenDescribing_ThenEveryFieldHasDescriptions()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsGreaterThan_NoError_TestingValidator());

        // Act
        var description = runner.Describe();
        var json = JsonConverter.ToJson(description);

        // Assert
        description.Results.Should().HaveCount(6);
        Approvals.VerifyJson(json);
    }
}
