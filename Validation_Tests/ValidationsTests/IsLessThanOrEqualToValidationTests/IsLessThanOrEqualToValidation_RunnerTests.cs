using ApprovalTests;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using PopValidations;
using PopValidations_Tests.TestHelpers;
using PopValidations_Tests.ValidationsTests.GenericTestableObjects;
using Xunit;

namespace PopValidations_Tests.ValidationsTests.IsLessThanOrEqualToValidationTests;

public class IsLessThanOrEqualTo_NoError_TestingValidator : AbstractValidator<NonNullAllFieldTypesDto>
{
    public IsLessThanOrEqualTo_NoError_TestingValidator()
    {
        Describe(x => x.Integer).IsLessThanOrEqualTo(int.MaxValue);
        Describe(x => x.String).IsLessThanOrEqualTo(new string(char.MaxValue, 100));
        Describe(x => x.Decimal).IsLessThanOrEqualTo(decimal.MaxValue);
        Describe(x => x.Double).IsLessThanOrEqualTo(double.MaxValue);
        Describe(x => x.Short).IsLessThanOrEqualTo(short.MaxValue);
        Describe(x => x.Long).IsLessThanOrEqualTo(long.MaxValue);
    }
}

public class IsLessThanOrEqualTo_AllErrored_TestingValidator : AbstractValidator<NonNullAllFieldTypesDto>
{
    public IsLessThanOrEqualTo_AllErrored_TestingValidator()
    {
        Describe(x => x.Integer).IsLessThanOrEqualTo(int.MinValue);
        Describe(x => x.String).IsLessThanOrEqualTo(new string(char.MinValue, 100));
        Describe(x => x.Decimal).IsLessThanOrEqualTo(decimal.MinValue);
        Describe(x => x.Double).IsLessThanOrEqualTo(double.MinValue);
        Describe(x => x.Short).IsLessThanOrEqualTo(short.MinValue);
        Describe(x => x.Long).IsLessThanOrEqualTo(long.MinValue);
    }
}

public class IsLessThanOrEqualToValidation_RunnerTests
{
    [Fact]
    public async Task GivenAValidator_WithNoErrors_ThenNoErrorsAreProduced()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsLessThanOrEqualTo_NoError_TestingValidator());

        // Act
        var validationResult = await runner.Validate(new NonNullAllFieldTypesDto());

        // Assert
        validationResult.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task GivenAValidator_WithErrors_ThenEveryFieldHasErrors()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsLessThanOrEqualTo_AllErrored_TestingValidator());

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
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsLessThanOrEqualTo_NoError_TestingValidator());

        // Act
        var description = runner.Describe();
        var json = JsonConverter.ToJson(description);

        // Assert
        description.Results.Should().HaveCount(6);
        Approvals.VerifyJson(json);
    }
}
