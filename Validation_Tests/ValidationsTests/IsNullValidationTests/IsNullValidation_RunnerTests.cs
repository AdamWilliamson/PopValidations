
using ApprovalTests;
using FluentAssertions;
using PopValidations;
using PopValidations_Tests.TestHelpers;
using PopValidations_Tests.ValidationsTests.GenericTestableObjects;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PopValidations_Tests.ValidationsTests.IsNullValidationTests;

public class IsNull_NoError_TestingValidator : AbstractValidator<NullAllFieldTypesDto>
{
    public IsNull_NoError_TestingValidator()
    {
        Describe(x => x.Integer).IsNull();
        Describe(x => x.String).IsNull();
        Describe(x => x.Decimal).IsNull();
        Describe(x => x.Double).IsNull();
        Describe(x => x.Short).IsNull();
        Describe(x => x.Long).IsNull();
        Describe(x => x.TwoComponentTuple).IsNull();
        Describe(x => x.TwoComponentNewTupple).IsNull();
        Describe(x => x.AllFieldTypesList).IsNull();
        Describe(x => x.AllFieldTypesLinkedList).IsNull();
        Describe(x => x.AllFieldTypesIEnumerable).IsNull();
        Describe(x => x.AllFieldTypesDictionary).IsNull();
        Describe(x => x.Struct).IsNull();
    }
}

public class IsNullValidation_RunnerTests
{
    [Fact]
    public async Task GivenAValidator_WithNoErrors_ThenNoErrorsAreProduced()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsNull_NoError_TestingValidator());

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

        // Assert
        validationResult.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task GivenAValidator_WithErrors_ThenEveryFieldHasErrors()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsNull_NoError_TestingValidator());

        // Act
        var validationResult = await runner.Validate(new NullAllFieldTypesDto()
        {
            Integer = 1,
            String = "",
            Decimal = 1.0m,
            Double = 1.0d,
            Short = 2,
            Long = 3,
            TwoComponentTuple = new(1,1),
            TwoComponentNewTupple = new(1,1),
            AllFieldTypesList = new(),
            AllFieldTypesLinkedList = new(),
            AllFieldTypesIEnumerable = new List<NonNullAllFieldTypesDto>(),
            AllFieldTypesDictionary = new(),
            Struct = new (),
        });
        var json = JsonConverter.ToJson(validationResult);

        // Assert
        validationResult.Errors.Should().HaveCount(13);
        Approvals.VerifyJson(json);
    }
}
