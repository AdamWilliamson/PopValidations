using FluentAssertions;
using FluentAssertions.Execution;
using PopValidations.Validations;
using PopValidations.Validations.Base;
using System;
using System.Collections.Generic;
using Xunit;

namespace PopValidations_Tests.ValidationsTests.IsLengthExclusivelyBetweenValidationTests;

public class IsLengthInclusivelyBetweenValidation_Tests
{
    [Theory]
    [InlineData(0, 0, 10)]
    [InlineData(1, 0, 10)]
    [InlineData(70, 50, 100)]
    [InlineData(9, 0, 10)]
    [InlineData(10, 0, 10)]
    [InlineData(-1, -10, -1)]
    [InlineData(-1, -10, 0)]
    [InlineData(-9, -10, 0)]
    [InlineData(-10, -10, 0)]
    public void WhenSupplyingAPassingValue_ItValidatesAsSuccessful(int value, int min, int max)
    {
        // Arrange
        var validator = new IsLengthInclusivelyBetweenValidation<int>(
            new ScopedData<int?>(min),
            new ScopedData<int?>(max)
        );

        // Act
        var result = validator.Validate(value);

        // Assert
        result.Success.Should().BeTrue();
    }

    [Theory]
    [InlineData(-1, 0, 10)]
    [InlineData(101, 50, 100)]
    [InlineData(11, 0, 10)]
    [InlineData(100, 0, 10)]
    [InlineData(-100, 0, 10)]
    [InlineData(-11, -10, 0)]
    [InlineData(1, -10, 0)]
    public void WhenSupplyingANonNullValue_ItValidatesAsUnsuccessful(int value, int min, int max)
    {
        // Arrange
        var validator = new IsLengthInclusivelyBetweenValidation<int>(
            new ScopedData<int?>(min),
            new ScopedData<int?>(max)
        );

        // Act
        var result = validator.Validate(value);

        // Assert
        result.Success.Should().BeFalse();
    }

    [Fact]
    public void TheValidationAndDescriptionValues_AreCorrect()
    {
        // Arrange
        var validator = new IsLengthInclusivelyBetweenValidation<int>(
            new ScopedData<int?>(0),
            new ScopedData<int?>(10)
        );

        // Act
        var validationResult = validator.Validate(-1);
        var descriptionResult = validator.Describe();

        // Assert
        using (new AssertionScope())
        {
            validationResult.Success.Should().BeFalse();
            validationResult.Message
                .Should()
                .Be("Is not between {{startValue}} and {{endValue}} inclusive.");
            validationResult.KeyValues
                .Should()
                .BeEquivalentTo(new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("startValue", "0"),
                    new KeyValuePair<string, string>("endValue", "10")
                });
            descriptionResult.Message
                .Should()
                .Be("Must be between {{startValue}} and {{endValue}} inclusive.");
        }
    }
}
