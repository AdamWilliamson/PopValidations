using FluentAssertions;
using FluentAssertions.Execution;
using PopValidations.Validations;
using System.Collections.Generic;
using Xunit;

namespace PopValidations_Tests.ValidationsTests.IsEmailValidationTests;

public class IsEmailValidation_Tests
{
    [Theory]
    [InlineData("a@a")]
    [InlineData("a@a.s")]
    [InlineData("a.a@a")]
    [InlineData("a@a.g.3.4")]
    public void WhenValidatingWithGoodValues_TheyAllPass(object value)
    {
        // Arrange
        var validator = new IsEmailValidation();

        // Act
        var result = validator.Validate(value);

        // Assert
        result.Success.Should().BeTrue();
    }

    [Theory]
    [InlineData("@a")]
    [InlineData("a@")]
    [InlineData("@")]
    [InlineData("a.a")]
    [InlineData(true)]
    [InlineData(1)]
    [InlineData(null)]
    public void WhenValidatingWithBadValues_TheyAllFail(object? value)
    {
        // Arrange
        var validator = new IsEmailValidation();

        // Act
        var result = validator.Validate(value);

        // Assert
        result.Success.Should().BeFalse();
    }

    [Fact]
    public void TheValidationAndDescriptionValues_AreCorrect()
    {
        // Arrange
        var validator = new IsEmailValidation();

        // Act
        var validationResult = validator.Validate("@123");
        var descriptionResult = validator.Describe();

        // Assert
        using (new AssertionScope())
        {
            validationResult.Message.Should().Be("Is not a valid email.");
            validationResult.KeyValues.Should()
                .BeEquivalentTo(
                    new List<KeyValuePair<string, string>>());
            descriptionResult.Message.Should().Be("Must be a valid email.");
        }
    }
}