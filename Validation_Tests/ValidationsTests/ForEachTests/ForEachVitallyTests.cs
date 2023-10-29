using FluentAssertions;
using FluentAssertions.Execution;
using PopValidations;
using PopValidations_Tests.TestHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PopValidations_Tests.ValidationsTests.ForEachTests;

public record VitallyModel(List<string> ArrayOfStrings);



public class ForEachValidator : AbstractValidator<VitallyModel>
{
    public ForEachValidator()
    {
        DescribeEnumerable(x => x.ArrayOfStrings)
            .Vitally().IsNotNull()
            .ForEach(x => x
                .Vitally().IsNotNull()
                .IsEqualTo("Test")
            );
    }
}


public class ForEachMultipleErrorsValidator : AbstractValidator<VitallyModel>
{
    public ForEachMultipleErrorsValidator()
    {
        DescribeEnumerable(x => x.ArrayOfStrings)
            .Vitally().IsNotNull()
            .ForEach(x => x
                .IsNull()
                .IsEqualTo("Test")
                .IsLengthInclusivelyBetween(1, 4)
            );
    }
}

public class ForEachVitallyTests
{
    public class VitallyForEachValidator : AbstractValidator<VitallyModel>
    {
        public VitallyForEachValidator()
        {
            DescribeEnumerable(x => x.ArrayOfStrings)
                .Vitally().IsNotNull()
                .Vitally().ForEach(x => x
                    .Vitally().IsNotNull()
                    .IsEqualTo("Test")
                    .IsEqualTo("Test")
                );
        }
    }

    [Theory]
    [InlineData(null, "Failure", null, 1, 1, 0)]
    [InlineData("Test", "Test_Fail", null, 1, 2, 1)]
    [InlineData("Test_Fail", "Test_Fail", null, 1, 2, 0)]
    public async Task GivenAVitallyFailingFirstItem_ThenItDoesntReportAnyAdditionalErrors(
        string? item1, string? item2, string? item3, int fields, int fieldErrorCount, int errorfieldIndex
        )
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new VitallyForEachValidator());
        var testSubject = new VitallyModel(
            ArrayOfStrings: new()
            {
                item1,
                item2,
                item3
            }
        );

        // Act
        var validationResult = await runner.Validate(testSubject);

        // Assert
        using (new AssertionScope())
        {
            validationResult.Errors.Should().HaveCount(fields);
            validationResult.Errors[$"ArrayOfStrings[{errorfieldIndex}]"].Should().HaveCount(fieldErrorCount);
        }
    }

    [Fact]
    public async Task GivenAVitallyFailingFirstItem_ThatDoesntVitallyFailItsOwnValidation_ReportsEqualToError()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new VitallyForEachValidator());
        var testSubject = new VitallyModel(
            ArrayOfStrings: new()
            {
                "FailingValue",
                "Failure",
                null
            }
        );

        // Act
        var validationResult = await runner.Validate(testSubject);

        // Assert
        using (new AssertionScope())
        {
            validationResult.Errors.Should().HaveCount(1);
            validationResult.GetErrorsForField("ArrayOfStrings[0]")
                .First().Should().Be("Is not equal to 'Test'.");
        }
    }

    [Fact]
    public async Task GivenAFailingFirstItem_WithoutVitalForEach_MultipleItemsError()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new ForEachValidator());
        var testSubject = new VitallyModel(
            ArrayOfStrings: new()
            {
                "FailingValue",
                "Failure",
                null
            }
        );

        // Act
        var validationResult = await runner.Validate(testSubject);

        // Assert
        using (new AssertionScope())
        {
            validationResult.Errors.Should().HaveCount(testSubject.ArrayOfStrings.Count);
            validationResult.GetErrorsForField("ArrayOfStrings[0]")
                .First().Should().Be("Is not equal to 'Test'.");
            validationResult.GetErrorsForField("ArrayOfStrings[1]")
                .First().Should().Be("Is not equal to 'Test'.");
            validationResult.GetErrorsForField("ArrayOfStrings[2]")
                .First().Should().Be("Is null.");
        }
    }

    [Fact]
    public async Task GivenEveryItemFailing_ItShouldReturnMultipleErrorsOnEachItem()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new ForEachMultipleErrorsValidator());
        var testSubject = new VitallyModel(
            ArrayOfStrings: new()
            {
                "FailingValue",
                "Failure",
                "Another Failure"
            }
        );

        // Act
        var validationResult = await runner.Validate(testSubject);

        // Assert
        using (new AssertionScope())
        {
            validationResult.Errors.Should().HaveCount(testSubject.ArrayOfStrings.Count);
            validationResult.GetErrorsForField("ArrayOfStrings[0]").Should().HaveCount(3);
            validationResult.GetErrorsForField("ArrayOfStrings[1]").Should().HaveCount(3);
            validationResult.GetErrorsForField("ArrayOfStrings[2]").Should().HaveCount(3);
        }
    }

    public class MultipleForEachValidator : AbstractValidator<VitallyModel>
    {
        public MultipleForEachValidator()
        {
            DescribeEnumerable(x => x.ArrayOfStrings)
                .Vitally().IsNotNull()
                .ForEach(x => x
                    .IsNull()
                );

            DescribeEnumerable(x => x.ArrayOfStrings)
                .Vitally().ForEach(x => x
                    .IsEqualTo("Test")
                );
        }
    }

    [Fact]
    public async Task GivenMultipleDescriptionsOfTheSameArray_VitallyForEach_Merges()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new MultipleForEachValidator());
        var testSubject = new VitallyModel(
            ArrayOfStrings: new()
            {
                "FailingValue",
                "Failure",
                "Another Failure"
            }
        );

        // Act
        var validationResult = await runner.Validate(testSubject);

        // Assert
        using (new AssertionScope())
        {
            validationResult.Errors.Should().HaveCount(1);
            validationResult.GetErrorsForField("ArrayOfStrings[0]").Should().HaveCount(2);
        }
    }
}
