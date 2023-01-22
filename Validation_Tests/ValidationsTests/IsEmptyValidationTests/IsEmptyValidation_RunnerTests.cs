﻿using ApprovalTests;
using FluentAssertions;
using PopValidations;
using PopValidations_Tests.TestHelpers;
using PopValidations_Tests.ValidationsTests.GenericTestableObjects;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PopValidations_Tests.ValidationsTests;

public class IsEmpty_NoError_TestingValidator : AbstractValidator<NullAllFieldTypesDto>
{
    public IsEmpty_NoError_TestingValidator()
    {
        Describe(x => x.String).IsEmpty();
        Describe(x => x.AllFieldTypesList).IsEmpty();
        Describe(x => x.AllFieldTypesLinkedList).IsEmpty();
        Describe(x => x.AllFieldTypesIEnumerable).IsEmpty();
        Describe(x => x.AllFieldTypesDictionary).IsEmpty();
    }
}

public class IsEmptyValidation_RunnerTests
{
    [Fact]
    public async Task GivenAValidator_WithNoErrors_ThenNoErrorsAreProduced()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsEmpty_NoError_TestingValidator());

        // Act
        var validationResult = await runner.Validate(new NullAllFieldTypesDto()
        {
            String = null,
        });

        // Assert
        validationResult.Results.Should().BeEmpty();
    }

    [Fact]
    public async Task GivenAValidator_WithErrors_ThenEveryFieldHasErrors()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsEmpty_NoError_TestingValidator());

        List<NonNullAllFieldTypesDto> list = new() { new NonNullAllFieldTypesDto() };
        LinkedList<NonNullAllFieldTypesDto> ll = new(list);
        Dictionary<string, NonNullAllFieldTypesDto> dictionary = new() { { "item", new NonNullAllFieldTypesDto() } };

        NullAllFieldTypesDto dto = new()
        {
            AllFieldTypesList = list,
            AllFieldTypesLinkedList = ll,
            AllFieldTypesIEnumerable = list,
            AllFieldTypesDictionary = dictionary
        };

        // Act
        var validationResult = await runner.Validate(dto);
        var json = JsonConverter.ToJson(validationResult);

        // Assert
        validationResult.Results.Should().HaveCount(5);
        Approvals.VerifyJson(json);
    }
}