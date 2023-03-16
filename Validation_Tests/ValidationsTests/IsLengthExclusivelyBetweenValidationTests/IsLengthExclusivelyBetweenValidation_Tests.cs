using FluentAssertions;
using FluentAssertions.Execution;
using PopValidations.Validations;
using PopValidations.Validations.Base;
using System.Collections.Generic;
using Xunit;

namespace PopValidations_Tests.ValidationsTests.IsLengthExclusivelyBetweenValidationTests;

public class IsLengthExclusivelyBetweenValidation_Tests
{
    [Theory]
    [MemberData(nameof(GetPassingData))]
    public void WhenSupplyingAPassingValue_ItValidatesAsSuccessful(int value)
    {
        // Arrange
        var validator = new IsLengthExclusivelyBetweenValidation<int>(
            new ScopedData<int?>(0),
            new ScopedData<int?>(10)
        );

        // Act
        var result = validator.Validate(value);

        // Assert
        result.Success.Should().BeTrue();
    }

    public static IEnumerable<object[]> GetPassingData()
    {
        for (int i = 1; i < 10; i++)
        {
            yield return new object[] { i };
        }
    }

    [Theory]
    [MemberData(nameof(GetFailingData))]
    public void WhenSupplyingANonNullValue_ItValidatesAsUnsuccessful(int value)
    {
        // Arrange
        var validator = new IsLengthExclusivelyBetweenValidation<int>(
            new ScopedData<int?>(0),
            new ScopedData<int?>(10)
        );

        // Act
        var result = validator.Validate(value);

        // Assert
        result.Success.Should().BeFalse();
    }

    public static IEnumerable<object[]> GetFailingData()
    {
        for (int i = -10; i < 1; i++)
        {
            yield return new object[] { i };
        }

        for (int x = 10; x < 20; x++)
        {
            yield return new object[] { x };
        }
    }

    [Fact]
    public void TheValidationAndDescriptionValues_AreCorrect()
    {
        // Arrange
        var validator = new IsLengthExclusivelyBetweenValidation<int>(
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
                .Be("Is not between {{startValue}} and {{endValue}} exclusive.");
            validationResult.KeyValues
                .Should()
                .BeEquivalentTo(new List<KeyValuePair<string, string>>() {
                    new KeyValuePair<string, string>("startValue", "0"),
                    new KeyValuePair<string, string>("endValue", "10")
                });
            descriptionResult.Message
                .Should()
                .Be("Must be between {{startValue}} and {{endValue}} exclusive.");
        }
    }
}
