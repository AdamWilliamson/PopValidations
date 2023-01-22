using ApprovalTests;
using FluentAssertions;
using System.Threading.Tasks;
using PopValidations;
using PopValidations_Tests.TestHelpers;
using PopValidations_Tests.ValidationsTests.GenericTestableObjects;
using Xunit;

namespace PopValidations_Tests.ValidationsTests.IsCustomValidationTests;

public class IsCustom_AllErrored_TestingValidator : AbstractValidator<NullAllFieldTypesDto>
{
    public IsCustom_AllErrored_TestingValidator()
    {
        Describe(x => x.Integer).Is("Description", "Error", IntTest);
        Describe(x => x.String).Is("Description", "Error", StringTest);
        Describe(x => x.Decimal).Is("Description", "Error", DecimalTest);
    }

    public static bool IntTest(int? value)
    {
        return value == 1;
    }

    public static bool StringTest(string? value)
    {
        return value == "1";
    }

    public static bool DecimalTest(decimal? value)
    {
        return value == 1;
    }
}

public class IsCustomValidation_RunnerTests
{
    [Fact]
    public async Task GivenAValidator_WithNoErrors_ThenNoErrorsAreProduced()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(
            new IsCustom_AllErrored_TestingValidator()
        );

        // Act
        var validationResult = await runner.Validate(
            new NullAllFieldTypesDto()
            {
                Integer = 1,
                String = "1",
                Decimal = 1
            }
        );

        // Assert
        validationResult.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task GivenAValidator_WithErrors_ThenEveryFieldHasErrors()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(
            new IsCustom_AllErrored_TestingValidator()
        );

        // Act
        var validationResult = await runner.Validate(
            new NullAllFieldTypesDto()
            {
                Integer = 2,
                String = "2",
                Decimal = 2
            }
        );
        var json = JsonConverter.ToJson(validationResult);

        // Assert
        validationResult.Errors.Should().HaveCount(3);
        Approvals.VerifyJson(json);
    }

    [Fact]
    public void GivenAValidator_WhenDescribing_ThenEveryFieldHasDescriptions()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(
            new IsCustom_AllErrored_TestingValidator()
        );

        // Act
        var description = runner.Describe();
        var json = JsonConverter.ToJson(description);

        // Assert
        description.Results.Should().HaveCount(3);
        Approvals.VerifyJson(json);
    }
}
