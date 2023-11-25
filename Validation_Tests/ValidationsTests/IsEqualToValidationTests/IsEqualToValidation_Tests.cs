using FluentAssertions;
using FluentAssertions.Execution;
using PopValidations.Validations;
using PopValidations.Validations.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PopValidations_Tests.ValidationsTests.IsEqualToValidationTests;

internal class TestObject : IComparable
{
    public TestObject(int test)
    {
        Test = test;
    }

    public int Test { get; set; }

    public int CompareTo(object? obj)
    {
        return Equals(obj)? 0 : 1;
    }

    public bool Equals(TestObject? other)
    {
        return Test.Equals(other?.Test);
    }

    public override bool Equals(object? obj)
    {
        if (obj is TestObject result)
        {
            return Equals(result);
        }

        return false;
    }
}

public class IsEqualToValidation_Tests
{
    [Theory]
    [InlineData(int.MaxValue, int.MaxValue)]
    [InlineData(double.MaxValue, double.MaxValue)]
    [InlineData(char.MaxValue, char.MaxValue)]
    [InlineData(float.MaxValue, float.MaxValue)]
    [InlineData(long.MaxValue, long.MaxValue)]
    [InlineData(short.MaxValue, short.MaxValue)]
    [InlineData("MatchingValue", "MatchingValue")]
    [InlineData(null, null)]
    public void WhenValidatingWithEqualValues_TheyAllPass(IComparable testValue, IComparable incomingValue)
    {
        // Arrange
        var validator = new IsEqualToValidation(new ScopedData<object, IComparable>(testValue));

        // Act
        var result = validator.Validate(incomingValue);

        // Assert
        result.Success.Should().BeTrue();
    }

    [Fact]
    public void GivenAObject_WhenValidatingWithEqualValues_ItPasses()
    {
        // Arrange
        var validator = new IsEqualToValidation(new ScopedData<object, TestObject>(new TestObject(1)));

        // Act
        var result = validator.Validate(new TestObject(1));

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
    [InlineData(short.MaxValue, null)]
    [InlineData("MatchingValue", "NotAMatchingValue")]
    public void WhenValidatingAginastDifferentTypes_TheyAllFail(IComparable testValue, IComparable incomingValue)
    {
        // Arrange
        var validator = new IsEqualToValidation(new ScopedData<object, IComparable>(testValue));

        // Act
        var result = validator.Validate(incomingValue);

        // Assert
        result.Success.Should().BeFalse();
    }

    [Fact]
    public void GivenAObject_WhenValidatingWithNonEqualValues_ItFails()
    {
        // Arrange
        var validator = new IsEqualToValidation(new ScopedData<object, TestObject>(new TestObject(1)));

        // Act
        var result = validator.Validate(new TestObject(2));

        // Assert
        result.Success.Should().BeFalse();
    }

    [Fact]
    public void TheValidationAndDescriptionValues_AreCorrect()
    {
        // Arrange
        var validator = new IsEqualToValidation(new ScopedData<object, IComparable>(2.33));

        // Act
        var validationResult = validator.Validate(0);
        var descriptionResult = validator.Describe();

        // Assert
        using (new AssertionScope())
        {
            validationResult.Message.Should().Be("Is not equal to '{{value}}'.");
            validationResult.KeyValues.Should()
                .BeEquivalentTo(
                    new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("value", 2.33.ToString())
                    });
            descriptionResult.Message.Should().Be("Must equal to '{{value}}'.");
        }
    }

    [Fact]
    public async Task WhenUsingScopedData_TheMessagesAreCorrect()
    {
        // Arrange
        var validator = new IsEqualToValidation(new ScopedData<int, int>("Original", (x) => Task.FromResult(x)));

        // Act
        await validator.InitScopes(2);
        var validationResult = validator.Validate(3);
        var descriptionResult = validator.Describe();

        // Assert
        using (new AssertionScope())
        {
            validationResult.Message.Should().Be("Is not equal to '{{value}}'.");
            descriptionResult.Message.Should().Be("Must equal to '{{value}}'.");
        }
    }
}
