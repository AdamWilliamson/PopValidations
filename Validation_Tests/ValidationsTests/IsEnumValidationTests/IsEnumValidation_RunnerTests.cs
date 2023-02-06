using ApprovalTests;
using FluentAssertions;
using System.Threading.Tasks;
using PopValidations;
using PopValidations_Tests.TestHelpers;
using PopValidations_Tests.ValidationsTests.GenericTestableObjects;
using Xunit;

namespace PopValidations_Tests.ValidationsTests.IsEnumToValidationTests;

public enum IsEnumTestEnum
{
    First = 10,
    Second = 20
}

public class IsEnum_NoError_TestingValidator : AbstractValidator<NullAllFieldTypesDto>
{
    public IsEnum_NoError_TestingValidator()
    {
        Describe(x => x.Integer).IsEnum(typeof(IsEnumTestEnum));
        Describe(x => x.String).IsEnum(typeof(IsEnumTestEnum));
        Describe(x => x.Decimal).IsEnum(typeof(IsEnumTestEnum));
        Describe(x => x.Double).IsEnum(typeof(IsEnumTestEnum));
        Describe(x => x.Short).IsEnum(typeof(IsEnumTestEnum));
        Describe(x => x.Long).IsEnum(typeof(IsEnumTestEnum));
    }
}

public class IsEnumValidation_RunnerTests
{
    [Fact]
    public async Task GivenAValidator_WithNoErrors_ThenNoErrorsAreProduced()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsEnum_NoError_TestingValidator());

        // Act
        var validationResult = await runner.Validate(new NullAllFieldTypesDto()
        {
            Integer = (int)IsEnumTestEnum.First,
            String = "Second",
            Decimal = (decimal)IsEnumTestEnum.Second,
            Double= (double)IsEnumTestEnum.First,
            Short = (short)IsEnumTestEnum.Second,
            Long = (long)IsEnumTestEnum.First
        });

        // Assert
        validationResult.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task GivenAValidator_WithErrors_ThenEveryFieldHasErrors()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsEnum_NoError_TestingValidator());

        // Act
        var validationResult = await runner.Validate(new NullAllFieldTypesDto()
        {
            Integer = 999,
            String = "999",
            Decimal = 999.0m,
            Double = 99.09d,
            Short = 999,
            Long = 999
        });

        // Assert
        validationResult.Errors.Should().HaveCount(6);
    }

    [Fact]
    public void GivenAValidator_WhenDescribing_ThenEveryFieldHasDescriptions()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsEnum_NoError_TestingValidator());

        // Act
        var description = runner.Describe();
        var json = JsonConverter.ToJson(description);

        // Assert
        description.Results.Should().HaveCount(6);
        Approvals.VerifyJson(json);
    }
}
