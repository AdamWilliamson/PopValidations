using FluentAssertions;
using FluentAssertions.Execution;
using PopValidations.Validations;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace PopValidations_Tests.ValidationsTests.IsEmptyValidationTests;

public class IsEmptyValidation_Tests
{
    [Theory]
    [MemberData(nameof(GetPassingData))]
    public void WhenSupplyingANullOrEmptyValue_ItValidatesAsSuccessful(object? value)
    {
        // Arrange
        var validator = new IsEmptyValidation();

        // Act
        var result = validator.Validate(value);

        // Assert
        result.Success.Should().BeTrue();
    }

    public static IEnumerable<object?[]> GetPassingData()
    {
        var allData = new List<object?[]>
        {
            new object?[] { System.Array.Empty<object>() },
            new object?[] { new List<object>() },
            new object?[] { new LinkedList<object>() },
            new object?[] { "" },
            new object?[] { " " },
            new object?[] { new Dictionary<object, object>() },
            new object?[] { null },
        };

        return allData;
    }

    [Theory]
    [MemberData(nameof(GetFailingData))]
    public void WhenSupplyingANonNullValue_ItValidatesAsUnsuccessful(IEnumerable value)
    {
        // Arrange
        var validator = new IsEmptyValidation();

        // Act
        var result = validator.Validate(value);

        // Assert
        result.Success.Should().BeFalse();
    }

    public static IEnumerable<object[]> GetFailingData()
    {
        LinkedList<object> ll = new();
        ll.AddFirst(new object());

        var allData = new List<object[]>
        {
            new object[] { new int[] { 1, 2, 3 } },
            new object[] { new List<object>() { new object() } },
            new object[] { ll },
            new object[] { "Hello" },
            new object[] { new Dictionary<object, object>() { { new object(), new object() } } },
        };

        return allData;
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
            validationResult.Message.Should().Be("Is not empty.");
            validationResult.KeyValues.Should()
                .BeEquivalentTo(new List<KeyValuePair<string, string>>());
            descriptionResult.Message.Should().Be("Must be empty.");
        }
    }
}
