using ApprovalTests;
using FluentAssertions;
using PopValidations;
using PopValidations.Validations;
using PopValidations_Tests.TestHelpers;
using PopValidations_Tests.ValidationsTests.GenericTestableObjects;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PopValidations_Tests.ValidationsTests;

public class NotEmpty_NoError_TestingValidator : AbstractValidator<NullAllFieldTypesDto>
{
    public NotEmpty_NoError_TestingValidator()
    {
        Describe(x => x.Integer).IsNotEmpty();
        Describe(x => x.String).IsNotEmpty();
        Describe(x => x.Decimal).IsNotEmpty();
        Describe(x => x.Double).IsNotEmpty();
        Describe(x => x.Short).IsNotEmpty();
        Describe(x => x.Long).IsNotEmpty();
        Describe(x => x.TwoComponentTuple).IsNotEmpty();
        Describe(x => x.TwoComponentNewTupple).IsNotEmpty();
        Describe(x => x.AllFieldTypesList).IsNotEmpty();
        Describe(x => x.AllFieldTypesLinkedList).IsNotEmpty();
        Describe(x => x.AllFieldTypesIEnumerable).IsNotEmpty();
        Describe(x => x.AllFieldTypesDictionary).IsNotEmpty();
        Describe(x => x.Struct).IsNotEmpty();
    }
}

public class IsNotEmptyValidation_RunnerTests
{
    [Fact]
    public async Task GivenAValidator_WithNoErrors_ThenNoErrorsAreProduced()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new NotEmpty_NoError_TestingValidator());

        NullAllFieldTypesDto dto = new()
        {
            String = "item",
            AllFieldTypesList = new() { new NonNullAllFieldTypesDto() },
            AllFieldTypesLinkedList = new(new[] { new NonNullAllFieldTypesDto() }),
            AllFieldTypesIEnumerable = new List<NonNullAllFieldTypesDto>() { new NonNullAllFieldTypesDto() },
            AllFieldTypesDictionary = new() { { "item", new NonNullAllFieldTypesDto() } }
        };

        // Act
        var validationResult = await runner.Validate(dto);

        // Assert
        validationResult.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task GivenAValidator_WithErrors_ThenEveryFieldHasErrors()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new NotEmpty_NoError_TestingValidator());

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

    [Fact]
    public void GivenAValidator_WhenDescribing_ThenEveryFieldHasDescriptions()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new NotEmpty_NoError_TestingValidator());

        // Act
        var description = runner.Describe();
        var json = JsonConverter.ToJson(description);

        // Assert
        description.Results.Should().HaveCount(13);
        Approvals.VerifyJson(json);
    }
}
