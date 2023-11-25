﻿using FluentAssertions;
using FluentAssertions.Execution;
using PopValidations.Validations;
using System.Collections.Generic;
using Xunit;

namespace PopValidations_Tests.ValidationsTests.NotNullValidationTests;

public class NotNullValidation_Tests
{
    [Fact]
    public void WhenSupplyingANonNullValue_ItValidatesAsSuccessful()
    {
        // Arrange
        var validator = new IsNotNullValidation();

        // Act
        var result = validator.Validate(0);

        // Assert
        result.Success.Should().BeTrue();
    }

    [Fact]
    public void WhenSupplyingANullValue_ItValidatesAsFailure()
    {
        // Arrange
        var validator = new IsNotNullValidation();

        // Act
        var result = validator.Validate(null);

        // Assert
        result.Success.Should().BeFalse();
    }

    [Fact]
    public void TheValidationAndDescriptionValues_AreCorrect()
    {
        // Arrange
        var validator = new IsNotNullValidation();

        // Act
        var validationResult = validator.Validate(null);
        var descriptionResult = validator.Describe();

        // Assert
        using (new AssertionScope())
        {
            validationResult.Message.Should().Be("Is null.");
            validationResult.KeyValues.Should()
                .BeEquivalentTo(new List<KeyValuePair<string, string>>());
            descriptionResult.Message.Should().Be("Must not be null.");
        }
    }
}
