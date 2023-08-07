using FluentAssertions;
using FluentAssertions.Execution;
using PopValidations.Validations;
using PopValidations.Validations.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PopValidations_Tests.ValidationsTests.IsLessThanOrEqualToValidationTests;

public class IsLessThanOrEqualToValidation_Tests
{
    [Theory]
    [InlineData(int.MaxValue, int.MinValue)]
    [InlineData(double.MaxValue, double.MinValue)]
    [InlineData(char.MaxValue, char.MinValue)]
    [InlineData(float.MaxValue, float.MinValue)]
    [InlineData(long.MaxValue, long.MinValue)]
    [InlineData(short.MaxValue, short.MinValue)]
    [InlineData(int.MaxValue, int.MaxValue)]
    [InlineData(double.MaxValue, double.MaxValue)]
    [InlineData(char.MaxValue, char.MaxValue)]
    [InlineData(float.MaxValue, float.MaxValue)]
    [InlineData(long.MaxValue, long.MaxValue)]
    [InlineData(short.MaxValue, short.MaxValue)]
    [InlineData(null, null)]
    public void WhenValidatingWithHigherValues_TheyAllPass(IComparable testValue, IComparable incomingValue)
    {
        // Arrange
        var validator = new IsLessThanOrEqualToValidation(new ScopedData<object, IComparable>(testValue));

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
    public void WhenValidatingAginastDifferentTypes_TheyAllFail(IComparable testValue, IComparable incomingValue)
    {
        // Arrange
        var validator = new IsLessThanOrEqualToValidation(new ScopedData<object, IComparable>(testValue));

        // Act
        var result = validator.Validate(incomingValue);

        // Assert
        result.Success.Should().BeFalse();
    }

    [Fact]
    public void TheValidationAndDescriptionValues_AreCorrect()
    {
        // Arrange
        var validator = new IsLessThanOrEqualToValidation(new ScopedData<object, IComparable>(0));

        // Act
        var validationResult = validator.Validate(2);
        var descriptionResult = validator.Describe();

        // Assert
        using (new AssertionScope())
        {
            validationResult.Message.Should().Be("Is not less than or equal to '{{value}}'.");
            validationResult.KeyValues.Should()
                .BeEquivalentTo(
                    new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("value",0.ToString())
                    });
            descriptionResult.Message.Should().Be("Must be less than or equal to '{{value}}'.");
        }
    }

    [Fact]
    public async Task WhenUsingScopedData_TheMessagesAreCorrect()
    {
        // Arrange
        var validator = new IsLessThanOrEqualToValidation(new ScopedData<int, int>("Original", (x) => Task.FromResult(x)));

        // Act
        await validator.InitScopes(2);
        var validationResult = validator.Validate(3);
        var descriptionResult = validator.Describe();

        // Assert
        using (new AssertionScope())
        {
            validationResult.Message.Should().Be("Is not less than or equal to '{{value}}'.");
            descriptionResult.Message.Should().Be("Must be less than or equal to '{{value}}'.");
        }
    }
}
