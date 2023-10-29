using FluentAssertions;
using FluentAssertions.Execution;
using PopValidations.Validations;
using System;
using System.Collections.Generic;
using Xunit;

namespace PopValidations_Tests.ValidationsTests.IsCustomValidationTests;

public class IsCustomValidation_Tests
{
    public static bool TestValue(int value)
    {
        return value == 0;
    }

    public static bool ThrowFunc(int value)
    {
        throw new System.Exception("Fake Error");
    }

    [Fact]
    public void GivenAPassingValue_ThenItPasses()
    {
        // Arrange
        var validator = new IsCustomValidation<int>(
            "Decription",
            "Error",
            TestValue
        );

        // Act
        var result = validator.Validate(0);

        // Assert
        result.Success.Should().BeTrue();
    }

    [Fact]
    public void GivenAFailingValue_ThenItFails()
    {
        // Arrange
        var validator = new IsCustomValidation<int>(
            "Description",
            "Error",
            TestValue
        );

        // Act
        var result = validator.Validate(1);

        // Assert
        result.Success.Should().BeFalse();
    }

    [Fact]
    public void WhenValidating_WithAFuncThatExceptions_ItExceptions()
    {
        // Arrange
        var validator = new IsCustomValidation<int>(
            "Description",
            "Error",
            ThrowFunc
        );

        // Act + Assert
        var result =  validator.Invoking(a => a.Validate(0)).Should().Throw<Exception>();
    }

    [Fact]
    public void TheValidationAndDescriptionValues_AreCorrect()
    {
        // Arrange
        var validator = new IsCustomValidation<int>(
            "Description",
            "Error",
            TestValue
        );

        // Act
        var validationResult = validator.Validate(1);
        var descriptionResult = validator.Describe();

        // Assert
        using (new AssertionScope())
        {
            validationResult.Message.Should().Be("Error");
            validationResult.KeyValues.Should()
                .BeEquivalentTo(
                    new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("value", 1.ToString()),
                        new KeyValuePair<string, string>("is_value", "")
                    });
            descriptionResult.Message.Should().Be("Description");
        }
    }
}
