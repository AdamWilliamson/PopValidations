using FluentAssertions;
using FluentAssertions.Execution;
using PopValidations.Validations;
using System.Collections.Generic;
using Xunit;

namespace PopValidations_Tests.ValidationsTests;

public class IsEmptyValidationTests
{
    [Fact]
    public void WhenSupplyingANullValue_ItValidatesAsSuccessful()
    {
        // Arrange
        var validator = new IsEmptyValidation();

        // Act
        var result = validator.Validate(null);

        // Assert
        result.Success.Should().BeTrue();
    }

    [Fact]
    public void TheValidationAndDescriptionValues_AreCorrect()
    {
        // Arrange
        var validator = new IsEmptyValidation();

        // Act
        var validationResult = validator.Validate(0);
        var descriptionResult = validator.Describe();

        // Assert
        using (new AssertionScope())
        {
            validationResult.Message.Should().Be("Is not empty");
            validationResult.KeyValues.Should()
                .BeEquivalentTo(new List<KeyValuePair<string, string>>());
            descriptionResult.Message.Should().Be("Must be empty");
        }
    }
}
