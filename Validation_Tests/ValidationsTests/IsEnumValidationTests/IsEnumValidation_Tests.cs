using FluentAssertions;
using FluentAssertions.Execution;
using PopValidations.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PopValidations_Tests.ValidationsTests.IsEnumToValidationTests;

public enum TestEnum
{
    First = 1,
    Second = 2
}

public class IsEnumValidation_Tests
{
    [Theory]
    [InlineData(1)]
    [InlineData(1.0d)]
    [InlineData('1')]
    [InlineData("1")]
    [InlineData(1.0f)]
    public void WhenValidatingWithConvertableValues_TheyAllPass(object value)
    {
        // Arrange
        var validator = new IsEnumValidation<object>(typeof(TestEnum));

        // Act
        var result = validator.Validate(value);

        // Assert
        result.Success.Should().BeTrue();
        result.KeyValues.Select(x => x.Value).Should().NotContain("unknown");
    }

    [Theory]
    [InlineData(int.MaxValue)]
    [InlineData(double.MaxValue)]
    [InlineData(char.MaxValue)]
    [InlineData(float.MaxValue)]
    [InlineData(long.MaxValue)]
    [InlineData(short.MaxValue)]
    [InlineData(null)]
    public void WhenValidatingWithNonConvertableValues_TheyAllFail(object value)
    {
        // Arrange
        var validator = new IsEnumValidation<object>(typeof(TestEnum));

        // Act
        var result = validator.Validate(value);

        // Assert
        result.Success.Should().BeFalse();
    }

    [Theory]
    [InlineData(typeof(Object))]
    [InlineData(null)]
    public void WhenValidatingWithOddTypes_TheyAllFail(object value)
    {
        // Arrange
        var validator = new IsEnumValidation<object>(typeof(TestEnum));

        // Act
        var result = validator.Validate(value);

        // Assert
        result.Success.Should().BeFalse();
        result.KeyValues.Select(x => x.Value).Should().Contain("unknown");
    }
    
    [Fact]
    public void TheValidationAndDescriptionValues_AreCorrect()
    {
        // Arrange
        var validator = new IsEnumValidation<int>(typeof(TestEnum));

        // Act
        var validationResult = validator.Validate(0);
        var descriptionResult = validator.Describe();

        // Assert
        using (new AssertionScope())
        {
            validationResult.Message.Should().Be("'{{value}}' Is not a valid value.");
            validationResult.KeyValues.Should()
                .BeEquivalentTo(
                    new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("value", "0"),
                        new KeyValuePair<string, string>("enumNames", string.Join(',', Enum.GetNames<TestEnum>())),
                        new KeyValuePair<string, string>(
                            "enumValues",
                            string.Join(
                                ',',
                                Enum.GetValues<TestEnum>().Cast<Enum>().Select(x => x.ToString("d")).ToArray()
                            )
                        ),
                        new KeyValuePair<string, string>("fieldType", "numeric"),
                    });
            descriptionResult.Message.Should().Be("Must be one of '{{enumNames}}' or '{{enumValues}}'.");
        }
    }
}
