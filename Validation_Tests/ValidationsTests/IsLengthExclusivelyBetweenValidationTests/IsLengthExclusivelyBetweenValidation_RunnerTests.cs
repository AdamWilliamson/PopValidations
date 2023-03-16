using ApprovalTests;
using FluentAssertions;
using PopValidations;
using PopValidations_Tests.TestHelpers;
using PopValidations_Tests.ValidationsTests.GenericTestableObjects;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PopValidations_Tests.ValidationsTests.IsLengthExclusivelyBetweenValidationTests;

public class IsLengthExclusivelyBetween_NoError_TestingValidator
    : AbstractValidator<NullAllFieldTypesDto>
{
    public IsLengthExclusivelyBetween_NoError_TestingValidator()
    {
        Describe(x => x.String).IsLengthExclusivelyBetween(0, 10);
        Describe(x => x.AllFieldTypesList).IsLengthExclusivelyBetween(-1, 1);
        Describe(x => x.AllFieldTypesLinkedList).IsLengthExclusivelyBetween(-1, 1);
        Describe(x => x.AllFieldTypesIEnumerable).IsLengthExclusivelyBetween(-1, 1);
        Describe(x => x.AllFieldTypesDictionary).IsLengthExclusivelyBetween(-1, 1);
    }
}

public class IsLengthExclusivelyBetweenValidation_RunnerTests2
{
    [Fact]
    public async Task GivenAValidator_WithNoErrors_ThenNoErrorsAreProduced2()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(
            new IsLengthExclusivelyBetween_NoError_TestingValidator()
        );

        // Act
        var validationResult = await runner.Validate(new NullAllFieldTypesDto() { String = "bbb", });

        // Assert
        validationResult.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task GivenAValidator_WithErrors_ThenEveryFieldHasErrors()
    {
       //Arrange
       var runner = ValidationRunnerHelper.BasicRunnerSetup(
           new IsLengthExclusivelyBetween_NoError_TestingValidator()
       );

        NullAllFieldTypesDto dto =
            new()
            {
                String = "zzzzzzzzzzzzzzzzzzzzzzzzzzz",
                AllFieldTypesList = new() { new NonNullAllFieldTypesDto() },
                AllFieldTypesLinkedList = new(new[] { new NonNullAllFieldTypesDto() }),
                AllFieldTypesIEnumerable = new List<NonNullAllFieldTypesDto>()
                {
                    new NonNullAllFieldTypesDto()
                },
                AllFieldTypesDictionary = new() { { "item", new NonNullAllFieldTypesDto() } }
            };

        // Act
        var validationResult = await runner.Validate(dto);
        var json = JsonConverter.ToJson(validationResult);

        validationResult.Errors.Should().HaveCount(5);
        Approvals.VerifyJson(json);
    }
}
