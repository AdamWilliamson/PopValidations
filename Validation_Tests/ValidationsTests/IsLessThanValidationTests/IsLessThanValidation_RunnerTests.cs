using ApprovalTests;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using PopValidations;
using PopValidations_Tests.TestHelpers;
using PopValidations_Tests.ValidationsTests.GenericTestableObjects;
using Xunit;

namespace PopValidations_Tests.ValidationsTests.IsLessThanValidationTests;

public class IsLessThan_NoError_TestingValidator : AbstractValidator<NonNullAllFieldTypesDto>
{
    public IsLessThan_NoError_TestingValidator()
    {
        Describe(x => x.Integer).IsLessThan(int.MaxValue);
        Describe(x => x.String).IsLessThan(new string(char.MaxValue, 100));
        Describe(x => x.Decimal).IsLessThan(decimal.MaxValue);
        Describe(x => x.Double).IsLessThan(double.MaxValue);
        Describe(x => x.Short).IsLessThan(short.MaxValue);
        Describe(x => x.Long).IsLessThan(long.MaxValue);
    }
}

public class IsLessThan_AllErrored_TestingValidator : AbstractValidator<NonNullAllFieldTypesDto>
{
    public IsLessThan_AllErrored_TestingValidator()
    {
        Describe(x => x.Integer).IsLessThan(int.MaxValue, options => options.WithErrorMessage("This Integer is not less then MaxValue"));
        Describe(x => x.String).IsLessThan(new string(char.MaxValue, 100));
        Describe(x => x.Decimal).IsLessThan(decimal.MaxValue);
        Describe(x => x.Double).IsLessThan(double.MaxValue);
        Describe(x => x.Short).IsLessThan(short.MaxValue);
        Describe(x => x.Long).IsLessThan(long.MaxValue);
    }
}

public class IsLessThanValidation_RunnerTests
{
    [Fact]
    public async Task GivenAValidator_WithNoErrors_ThenNoErrorsAreProduced()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsLessThan_NoError_TestingValidator());

        // Act
        var validationResult = await runner.Validate(new NonNullAllFieldTypesDto()
        {
            Integer = int.MaxValue/2,
            String = new string('g', 100),
            Decimal = decimal.MaxValue / 2,
            Double = double.MaxValue / 2,
            Short = short.MaxValue/2,
            Long = long.MaxValue / 2
        });

        // Assert
        validationResult.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task GivenAValidator_WithErrors_ThenEveryFieldHasErrors()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsLessThan_AllErrored_TestingValidator());

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
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsLessThan_NoError_TestingValidator());

        // Act
        var description = runner.Describe();
        var json = JsonConverter.ToJson(description);

        // Assert
        description.Results.Should().HaveCount(6);
        Approvals.VerifyJson(json);
    }
}
