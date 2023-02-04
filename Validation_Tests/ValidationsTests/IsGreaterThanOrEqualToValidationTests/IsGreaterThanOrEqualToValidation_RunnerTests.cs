using ApprovalTests;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using PopValidations;
using PopValidations_Tests.TestHelpers;
using PopValidations_Tests.ValidationsTests.GenericTestableObjects;
using Xunit;

namespace PopValidations_Tests.ValidationsTests.IsGreaterThanOrEqualToValidationTests;

public class IsGreaterThanOrEqualTo_NoError_TestingValidator : AbstractValidator<NonNullAllFieldTypesDto>
{
    public IsGreaterThanOrEqualTo_NoError_TestingValidator()
    {
        Describe(x => x.Integer).IsGreaterThanOrEqualTo(int.MinValue/2).IsGreaterThanOrEqualTo(int.MinValue);
        Describe(x => x.String).IsGreaterThanOrEqualTo(new string(char.MinValue, 100)).IsGreaterThanOrEqualTo(new string(char.MaxValue, 100));
        Describe(x => x.Decimal).IsGreaterThanOrEqualTo(decimal.MinValue).IsGreaterThanOrEqualTo(decimal.MaxValue);
        Describe(x => x.Double).IsGreaterThanOrEqualTo(double.MinValue).IsGreaterThanOrEqualTo(double.MaxValue);
        Describe(x => x.Short).IsGreaterThanOrEqualTo(short.MinValue).IsGreaterThanOrEqualTo(short.MaxValue);
        Describe(x => x.Long).IsGreaterThanOrEqualTo(long.MinValue).IsGreaterThanOrEqualTo(long.MaxValue);
    }
}

public class IsGreaterThanOrEqualTo_AllErrored_TestingValidator : AbstractValidator<NonNullAllFieldTypesDto>
{
    public IsGreaterThanOrEqualTo_AllErrored_TestingValidator()
    {
        Describe(x => x.Integer).IsGreaterThanOrEqualTo(int.MaxValue);
        Describe(x => x.String).IsGreaterThanOrEqualTo(new string(char.MaxValue, 100));
        Describe(x => x.Decimal).IsGreaterThanOrEqualTo(decimal.MaxValue);
        Describe(x => x.Double).IsGreaterThanOrEqualTo(double.MaxValue);
        Describe(x => x.Short).IsGreaterThanOrEqualTo(short.MaxValue);
        Describe(x => x.Long).IsGreaterThanOrEqualTo(long.MaxValue);
    }
}

public class IsGreaterThanOrEqualToValidation_RunnerTests
{
    [Fact]
    public async Task GivenAValidator_WithNoErrors_ThenNoErrorsAreProduced()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsGreaterThanOrEqualTo_NoError_TestingValidator());

        // Act
        var validationResult = await runner.Validate(new NonNullAllFieldTypesDto());

        // Assert
        validationResult.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task GivenAValidator_WithErrors_ThenEveryFieldHasErrors()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsGreaterThanOrEqualTo_AllErrored_TestingValidator());

        // Act
        var validationResult = await runner.Validate(new NonNullAllFieldTypesDto()
        {
            Integer = int.MaxValue / 2,
            String = new string('g', 100),
            Decimal = decimal.MaxValue / 2,
            Double = double.MaxValue / 2,
            Short = short.MaxValue / 2,
            Long = long.MaxValue / 2
        });
        var json = JsonConverter.ToJson(validationResult);

        // Assert
        validationResult.Errors.Should().HaveCount(6);
        Approvals.VerifyJson(json);
    }

    [Fact]
    public void GivenAValidator_WhenDescribing_ThenEveryFieldHasDescriptions()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsGreaterThanOrEqualTo_NoError_TestingValidator());

        // Act
        var description = runner.Describe();
        var json = JsonConverter.ToJson(description);

        // Assert
        description.Results.Should().HaveCount(6);
        Approvals.VerifyJson(json);
    }
}
