using ApprovalTests;
using FluentAssertions;
using PopValidations;
using PopValidations_Tests.TestHelpers;
using PopValidations_Tests.ValidationsTests.GenericTestableObjects;
using System.Threading.Tasks;
using Xunit;

namespace PopValidations_Tests.ValidationsTests.IsEmailValidationTests;

public class IsEmail_NoError_TestingValidator : AbstractValidator<NonNullAllFieldTypesDto>
{
    public IsEmail_NoError_TestingValidator()
    {
        Describe(x => x.String).IsEmail();
    }
}

public class IsEmail_AllErrored_TestingValidator : AbstractValidator<NonNullAllFieldTypesDto>
{
    public IsEmail_AllErrored_TestingValidator()
    {
        Describe(x => x.Integer).IsEmail();
        Describe(x => x.String).IsEmail();
        Describe(x => x.Decimal).IsEmail();
        Describe(x => x.Double).IsEmail();
        Describe(x => x.Short).IsEmail();
        Describe(x => x.Long).IsEmail();
    }
}

public class IsEmailValidation_RunnerTests
{
    [Fact]
    public async Task GivenAValidator_WithNoErrors_ThenNoErrorsAreProduced()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsEmail_NoError_TestingValidator());

        // Act
        var validationResult = await runner.Validate(new NonNullAllFieldTypesDto()
        {
            String = "IsEmail@UnitTest.com",
        });

        // Assert
        validationResult.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task GivenAValidator_WithErrors_ThenEveryFieldHasErrors()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsEmail_AllErrored_TestingValidator());

        // Act
        var validationResult = await runner.Validate(new NonNullAllFieldTypesDto());
        var json = JsonConverter.ToJson(validationResult);

        // Assert
        validationResult.Errors.Should().HaveCount(6);
        Approvals.VerifyJson(json);
    }
}