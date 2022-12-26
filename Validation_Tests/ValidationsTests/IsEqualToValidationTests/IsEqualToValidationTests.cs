using FluentAssertions;
using FluentAssertions.Execution;
using PopValidations.Validations;
using PopValidations.Validations.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Validations_Tests.ValidationsTests;

public class IsEqualToValidationTests
{
    [Theory]
    [InlineData(int.MaxValue, int.MaxValue)]
    [InlineData(double.MaxValue, double.MaxValue)]
    [InlineData(char.MaxValue, char.MaxValue)]
    [InlineData(float.MaxValue, float.MaxValue)]
    [InlineData(long.MaxValue, long.MaxValue)]
    [InlineData(short.MaxValue, short.MaxValue)]
    public void WhenValidatingWithEqualValues_TheyAllPass(IComparable testValue, IComparable incomingValue)
    {
        // Arrange
        var validator = new IsEqualToValidation(testValue);

        // Act
        var result = validator.Validate(incomingValue);

        // Assert
        result.Success.Should().BeTrue();
    }

    [Theory]
    [InlineData(int.MaxValue, double.MaxValue)]
    [InlineData(double.MaxValue, char.MaxValue)]
    [InlineData(char.MaxValue, float.MaxValue)]
    [InlineData(float.MaxValue, long.MaxValue)]
    [InlineData(long.MaxValue, short.MaxValue)]
    [InlineData(short.MaxValue, int.MaxValue)]
    public void WhenValidatingAginastDifferentTypes_TheyAllFail(IComparable testValue, IComparable incomingValue)
    {
        // Arrange
        var validator = new IsEqualToValidation(testValue);

        // Act
        var result = validator.Validate(incomingValue);

        // Assert
        result.Success.Should().BeFalse();
    }

    [Fact]
    public void TheValidationAndDescriptionValues_AreCorrect()
    {
        // Arrange
        var validator = new IsEqualToValidation(2.33);

        // Act
        var validationResult = validator.Validate(0);
        var descriptionResult = validator.Describe();

        // Assert
        using(new AssertionScope())
        { 
            validationResult.Message.Should().Be("Is not equal to '{{value}}'");
            validationResult.KeyValues.Should()
                .BeEquivalentTo(
                    new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("value", 2.33.ToString())
                    });
            descriptionResult.Message.Should().Be("Must equal to '{{value}}'");
        }
    }

    [Fact]
    public void WhenUsingScopedData_TheMessagesAreCorrect()
    {
        // Arrange
        var validator = new IsEqualToValidation(new ScopedData <int, int>((int x) => Task.FromResult(x)));

        // Act
        validator.InitScopes(2.33);
        var validationResult = validator.Validate(2.33);
        var descriptionResult = validator.Describe();

        // Assert
        using (new AssertionScope())
        {
            validationResult.Message.Should().Be("Is not equal to '{{value}}'");
            descriptionResult.Message.Should().Be("Must equal to '{{value}}'");
        }
    }
}
