using FluentAssertions;
using FluentAssertions.Execution;
using PopValidations.Validations;
using System;
using System.Collections.Generic;
using Xunit;

namespace PopValidations_Tests.ValidationsTests.IsNullValidationTests;

public class IsNullValidation_Tests
{
    [Fact]
    public void WhenSupplyingANullValue_ItValidatesAsSuccessful()
    {
        // Arrange
        var validator = new IsNullValidation();

        // Act
        var result = validator.Validate(null);

        // Assert
        result.Success.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(NonNullValues))]
    public void WhenSupplyingANonNullValue_ItValidatesAsFailure(object? value)
    {
        // Arrange
        var validator = new IsNullValidation();

        // Act
        var result = validator.Validate(value);

        // Assert
        result.Success.Should().BeFalse();
    }

    public static IEnumerable<object[]> NonNullValues()
    {
        yield return new object[] { "" };
        yield return new object[] { "   " };
        yield return new object[] { Array.Empty<int>() };
        yield return new object[] { new object() };
        yield return new object[] { new List<string>() };
    }


    [Fact]
    public void TheValidationAndDescriptionValues_AreCorrect()
    {
        // Arrange
        var validator = new IsNullValidation();

        // Act
        var validationResult = validator.Validate(0);
        var descriptionResult = validator.Describe();

        // Assert
        using (new AssertionScope())
        {
            validationResult.Message.Should().Be("Is not null.");
            validationResult.KeyValues.Should()
                .BeEquivalentTo(new List<KeyValuePair<string, string>>());
            descriptionResult.Message.Should().Be("Must be null.");
        }
    }
}
