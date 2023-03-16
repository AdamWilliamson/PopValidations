using ApprovalTests;
using FluentAssertions;
using PopValidations;
using PopValidations_Tests.TestHelpers;
using PopValidations_Tests.ValidationsTests.GenericTestableObjects;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PopValidations_Tests.ValidationsTests.IsLengthExclusivelyBetweenValidationTests;

public class IsLengthInclusivelyBetween_NoError_TestingValidator
    : AbstractValidator<NullAllFieldTypesDto>
{
    public IsLengthInclusivelyBetween_NoError_TestingValidator()
    {
        Describe(x => x.String).IsLengthInclusivelyBetween(0, 10);
        Describe(x => x.AllFieldTypesList).IsLengthInclusivelyBetween(0, 1);
        Describe(x => x.AllFieldTypesLinkedList).IsLengthInclusivelyBetween(0, 1);
        Describe(x => x.AllFieldTypesIEnumerable).IsLengthInclusivelyBetween(0, 1);
        Describe(x => x.AllFieldTypesDictionary).IsLengthInclusivelyBetween(0, 1);
    }
}

public class IsLengthInclusivelyBetweenValidation_RunnerTests
{
    [Fact]
    public async Task GivenAValidator_WithNoErrors_ThenNoErrorsAreProduced()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(
            new IsLengthInclusivelyBetween_NoError_TestingValidator()
        );

        // Act
        var validationResult = await runner.Validate(new NullAllFieldTypesDto() { String = "bbb", });

        // Assert
        validationResult.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task GivenAValidator_WithErrors_ThenEveryFieldHasErrors()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(
            new IsLengthInclusivelyBetween_NoError_TestingValidator()
        );

        NullAllFieldTypesDto dto =
            new()
            {
                String = "zzzzzzzzzzzzzzzzzzzz",
                AllFieldTypesList = new() { new NonNullAllFieldTypesDto(), new NonNullAllFieldTypesDto() },
                AllFieldTypesLinkedList = new(new[] { new NonNullAllFieldTypesDto(), new NonNullAllFieldTypesDto() }),
                AllFieldTypesIEnumerable = new List<NonNullAllFieldTypesDto>()
                {
                    new NonNullAllFieldTypesDto(),
                    new NonNullAllFieldTypesDto()
                },
                AllFieldTypesDictionary = new() { { "item", new NonNullAllFieldTypesDto() }, { "item2", new NonNullAllFieldTypesDto() } }
            };

        // Act
        var validationResult = await runner.Validate(dto);
        var json = JsonConverter.ToJson(validationResult);

        // Assert
        validationResult.Errors.Should().HaveCount(5);
        Approvals.VerifyJson(json);
    }
}
