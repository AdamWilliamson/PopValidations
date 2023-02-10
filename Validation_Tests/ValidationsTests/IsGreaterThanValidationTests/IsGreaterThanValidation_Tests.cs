using FluentAssertions;
using FluentAssertions.Execution;
using PopValidations.Validations;
using PopValidations.Validations.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PopValidations_Tests.ValidationsTests.IsGreaterThanValidationTests;

public class IsGreaterThanValidation_Tests
{
    [Theory]
    [InlineData(int.MinValue, int.MaxValue)]
    [InlineData(double.MinValue, double.MaxValue)]
    [InlineData(char.MinValue, char.MaxValue)]
    [InlineData(float.MinValue, float.MaxValue)]
    [InlineData(long.MinValue, long.MaxValue)]
    [InlineData(short.MinValue, short.MaxValue)]
    [InlineData(null, null)]
    public void WhenValidatingWithHigherValues_TheyAllPass(IComparable testValue, IComparable incomingValue)
    {
        // Arrange
        var validator = new IsGreaterThanValidation(new ScopedData<object, IComparable>(testValue));

        // Act
        var result = validator.Validate(incomingValue);

        // Assert
        result.Success.Should().BeTrue();
    }

    [Theory]
    [InlineData(int.MinValue, double.MaxValue)]
    [InlineData(double.MinValue, char.MaxValue)]
    [InlineData(char.MinValue, float.MaxValue)]
    [InlineData(float.MinValue, long.MaxValue)]
    [InlineData(long.MinValue, short.MaxValue)]
    [InlineData(short.MinValue, int.MaxValue)]
    [InlineData(short.MinValue, null)]
    public void WhenValidatingAginastDifferentTypes_TheyAllFail(IComparable testValue, IComparable incomingValue)
    {
        // Arrange
        var validator = new IsGreaterThanValidation(new ScopedData<object, IComparable>(testValue));

        // Act
        var result = validator.Validate(incomingValue);

        // Assert
        result.Success.Should().BeFalse();
    }

    [Fact]
    public void TheValidationAndDescriptionValues_AreCorrect()
    {
        // Arrange
        var validator = new IsGreaterThanValidation(new ScopedData<object, IComparable>(2.33));

        // Act
        var validationResult = validator.Validate(0);
        var descriptionResult = validator.Describe();

        // Assert
        using (new AssertionScope())
        {
            validationResult.Message.Should().Be("Is not greater than '{{value}}'");
            validationResult.KeyValues.Should()
                .BeEquivalentTo(
                    new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("value", 2.33.ToString())
                    });
            descriptionResult.Message.Should().Be("Must be greater than '{{value}}'");
        }
    }

    [Fact]
    public async Task WhenUsingScopedData_TheMessagesAreCorrect()
    {
        // Arrange
        var validator = new IsGreaterThanValidation(new ScopedData<int, int>("Original", (x) => Task.FromResult(x)));

        // Act
        await validator.InitScopes(3);
        var validationResult = validator.Validate(2);
        var descriptionResult = validator.Describe();

        // Assert
        using (new AssertionScope())
        {
            validationResult.Message.Should().Be("Is not greater than '{{value}}'");
            descriptionResult.Message.Should().Be("Must be greater than '{{value}}'");
        }
    }
}
