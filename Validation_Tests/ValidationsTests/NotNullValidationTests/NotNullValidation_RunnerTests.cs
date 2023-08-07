using ApprovalTests;
using FluentAssertions;
using PopValidations;
using PopValidations_Tests.TestHelpers;
using PopValidations_Tests.ValidationsTests.GenericTestableObjects;
using System.Threading.Tasks;
using Xunit;

namespace PopValidations_Tests.ValidationsTests.NotNullValidationTests.NotNullValidationTests;

public class NotNull_NoError_TestingValidator : AbstractValidator<NullAllFieldTypesDto>
{
    public NotNull_NoError_TestingValidator()
    {
        Describe(x => x.Integer).IsNotNull();
        Describe(x => x.String).IsNotNull();
        Describe(x => x.Decimal).IsNotNull();
        Describe(x => x.Double).IsNotNull();
        Describe(x => x.Short).IsNotNull();
        Describe(x => x.Long).IsNotNull();
        Describe(x => x.TwoComponentTuple).IsNotNull();
        Describe(x => x.TwoComponentNewTupple).IsNotNull();
        Describe(x => x.AllFieldTypesList).IsNotNull();
        Describe(x => x.AllFieldTypesLinkedList).IsNotNull();
        Describe(x => x.AllFieldTypesIEnumerable).IsNotNull();
        Describe(x => x.AllFieldTypesDictionary).IsNotNull();
        Describe(x => x.Struct).IsNotNull();
    }
}

public class NotNullValidation_RunnerTests
{
    [Fact]
    public async Task GivenAValidator_WithNoErrors_ThenNoErrorsAreProduced()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new NotNull_NoError_TestingValidator());

        // Act
        var validationResult = await runner.Validate(new NullAllFieldTypesDto());

        // Assert
        validationResult.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task GivenAValidator_WithErrors_ThenEveryFieldHasErrors()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new NotNull_NoError_TestingValidator());

        // Act
        var validationResult = await runner.Validate(new NullAllFieldTypesDto()
        {
            Integer = null,
            String = null,
            Decimal = null,
            Double = null,
            Short = null,
            Long = null,
            TwoComponentTuple = null,
            TwoComponentNewTupple = null,
            AllFieldTypesList = null,
            AllFieldTypesLinkedList = null,
            AllFieldTypesIEnumerable = null,
            AllFieldTypesDictionary = null,
            Struct = null,
        });
        var json = JsonConverter.ToJson(validationResult);

        // Assert
        validationResult.Errors.Should().HaveCount(13);
        Approvals.VerifyJson(json);
    }
}
