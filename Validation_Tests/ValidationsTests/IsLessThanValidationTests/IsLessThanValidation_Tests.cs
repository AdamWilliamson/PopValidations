using FluentAssertions;
using FluentAssertions.Execution;
using PopValidations.Validations;
using PopValidations.Validations.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PopValidations_Tests.ValidationsTests.IsLessThanValidationTests;

public class IsLessThanValidation_Tests
{
    [Theory]
    [InlineData(int.MaxValue, int.MinValue)]
    [InlineData(double.MaxValue, double.MinValue)]
    [InlineData(char.MaxValue, char.MinValue)]
    [InlineData(float.MaxValue, float.MinValue)]
    [InlineData(long.MaxValue, long.MinValue)]
    [InlineData(short.MaxValue, short.MinValue)]
    [InlineData(null, null)]
    [InlineData(1, 0)]
    public void WhenValidatingWithHigherValues_TheyAllPass(IComparable testValue, IComparable incomingValue)
    {
        // Arrange
        var validator = new IsLessThanValidation(new ScopedData<object, IComparable>(testValue));

        // Act
        var result = validator.Validate(incomingValue);

        // Assert
        result.Success.Should().BeTrue();
    }

    [Theory]
    [InlineData(int.MaxValue, double.MinValue)]
    [InlineData(double.MaxValue, char.MinValue)]
    [InlineData(char.MaxValue, float.MinValue)]
    [InlineData(float.MaxValue, long.MinValue)]
    [InlineData(long.MaxValue, short.MinValue)]
    [InlineData(short.MaxValue, int.MinValue)]
    [InlineData(short.MaxValue, null)]
    [InlineData(-2, -1)]
    [InlineData(-1, -1)]
    [InlineData(1, 1)]
    public void WhenValidatingAginastDifferentTypes_TheyAllFail(IComparable testValue, IComparable incomingValue)
    {
        // Arrange
        var validator = new IsLessThanValidation(new ScopedData<object, IComparable>(testValue));

        // Act
        var result = validator.Validate(incomingValue);

        // Assert
        result.Success.Should().BeFalse();
    }

    [Fact]
    public void TheValidationAndDescriptionValues_AreCorrect()
    {
        // Arrange
        var validator = new IsLessThanValidation(new ScopedData<object, IComparable>(0));

        // Act
        var validationResult = validator.Validate(2.33);
        var descriptionResult = validator.Describe();

        // Assert
        using (new AssertionScope())
        {
            validationResult.Message.Should().Be("Is not less than '{{value}}'.");
            validationResult.KeyValues.Should()
                .BeEquivalentTo(
                    new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("value", 0.ToString())
                    });
            descriptionResult.Message.Should().Be("Must be less than '{{value}}'.");
        }
    }

    [Fact]
    public async Task WhenUsingScopedData_TheMessagesAreCorrect()
    {
        // Arrange
        var validator = new IsLessThanValidation(new ScopedData<int, int>("Original", (x) => Task.FromResult(x)));

        // Act
        await validator.InitScopes(2);
        var validationResult = validator.Validate(3);
        var descriptionResult = validator.Describe();

        // Assert
        using (new AssertionScope())
        {
            validationResult.Message.Should().Be("Is not less than '{{value}}'.");
            descriptionResult.Message.Should().Be("Must be less than '{{value}}'.");
        }
    }
}
