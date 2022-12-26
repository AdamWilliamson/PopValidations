using ApprovalTests;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using Validations;
using Validations_Tests.TestHelpers;
using Validations_Tests.ValidationsTests.GenericTestableObjects;
using Xunit;

namespace Validations_Tests.ValidationsTests;

public class EqualTo_NoError_TestingValidator : AbstractValidator<NonNullAllFieldTypesDto>
{
    public EqualTo_NoError_TestingValidator()
    {
        Describe(x => x.Integer).IsEqualTo(int.MaxValue);
        Describe(x => x.String).IsEqualTo(new string(char.MaxValue, 100));
        Describe(x => x.Decimal).IsEqualTo(decimal.MaxValue);
        Describe(x => x.Double).IsEqualTo(double.MaxValue);
        Describe(x => x.Short).IsEqualTo(short.MaxValue);
        Describe(x => x.Long).IsEqualTo(long.MaxValue);
        Describe(x => x.TwoComponentTuple).IsEqualTo(Tuple.Create(int.MinValue, int.MaxValue));
    }
}

public class EqualTo_AllErrored_TestingValidator : AbstractValidator<NonNullAllFieldTypesDto>
{
    public EqualTo_AllErrored_TestingValidator()
    {
        Describe(x => x.Integer).IsEqualTo(int.MinValue);
        Describe(x => x.String).IsEqualTo(new string(char.MinValue, 100));
        Describe(x => x.Decimal).IsEqualTo(decimal.MinValue);
        Describe(x => x.Double).IsEqualTo(double.MinValue);
        Describe(x => x.Short).IsEqualTo(short.MinValue);
        Describe(x => x.Long).IsEqualTo(long.MinValue);
        Describe(x => x.TwoComponentTuple).IsEqualTo(Tuple.Create(int.MaxValue, int.MinValue));
    }
}

public class IsEqualToValidation_RunnerTests
{
    [Fact]
    public async Task GivenAValidator_WithNoErrors_ThenNoErrorsAreProduced()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new EqualTo_NoError_TestingValidator());

        // Act
        var validationResult = await runner.Validate(new NonNullAllFieldTypesDto());

        // Assert
        validationResult.Results.Should().BeEmpty();
    }

    [Fact]
    public async Task GivenAValidator_WithErrors_ThenEveryFieldHasErrors()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new EqualTo_AllErrored_TestingValidator());

        // Act
        var validationResult = await runner.Validate(new NonNullAllFieldTypesDto());
        var json = JsonConverter.ToJson(validationResult);

        // Assert
        validationResult.Results.Should().HaveCount(7);
        Approvals.VerifyJson(json);
    }

    [Fact]
    public void GivenAValidator_WhenDescribing_ThenEveryFieldHasDescriptions()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new EqualTo_NoError_TestingValidator());

        // Act
        var description = runner.Describe();
        var json = JsonConverter.ToJson(description);

        // Assert
        description.Results.Should().HaveCount(7);
        Approvals.VerifyJson(json);
    }
}
