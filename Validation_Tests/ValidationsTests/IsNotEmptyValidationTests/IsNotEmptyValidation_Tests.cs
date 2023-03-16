using FluentAssertions;
using FluentAssertions.Execution;
using PopValidations.Validations;
using PopValidations_Tests.ValidationsTests.GenericTestableObjects;
using System;
using System.Collections.Generic;
using Xunit;

namespace PopValidations_Tests.ValidationsTests.IsNotEmptyValidationTests;

public class IsNotEmptyValidation_Tests
{
    [Theory]
    [MemberData(nameof(NotNullOrEmptyData))]
    public void WhenSupplyingAValue_ItValidatesAsSuccessful(object? value)
    {
        // Arrange
        var validator = new IsNotEmptyValidation();

        // Act
        var result = validator.Validate(value);

        // Assert
        result.Success.Should().BeTrue();
    }

    public static IEnumerable<object[]> NotNullOrEmptyData()
    {
        yield return new object[] { "a" };
        yield return new object[] { "    b  " };
        yield return new object[] { new[] { 1 } };
        yield return new object[] { new NonNullAllFieldTypesDto() };
    }

    [Theory]
    [MemberData(nameof(NullAndEmptyData))]
    public void WhenSupplyingAEmptyValue_ItValidatesAsUnsuccessful(object? value)
    {
        // Arrange
        var validator = new IsNotEmptyValidation();

        // Act
        var result = validator.Validate(value);

        // Assert
        result.Success.Should().BeFalse();
    }

    public static IEnumerable<object[]> NullAndEmptyData()
    {
        yield return new object[] { "" };
        yield return new object[] { "   " };
        yield return new object[] { Array.Empty<int>() };
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        yield return new object[] { null };
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }

    [Fact]
    public void TheValidationAndDescriptionValues_AreCorrect()
    {
        // Arrange
        var validator = new IsNotEmptyValidation();

        // Act
        var validationResult = validator.Validate("");
        var descriptionResult = validator.Describe();

        // Assert
        using (new AssertionScope())
        {
            validationResult.Message.Should().Be("Is empty");
            validationResult.KeyValues.Should()
                .BeEquivalentTo(new List<KeyValuePair<string, string>>());
            descriptionResult.Message.Should().Be("Must not be empty");
        }
    }
}
