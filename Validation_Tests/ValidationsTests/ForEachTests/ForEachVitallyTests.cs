using ApprovalTests;
using FluentAssertions;
using FluentAssertions.Execution;
using Newtonsoft.Json;
using PopValidations;
using PopValidations_Tests.TestHelpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PopValidations_Tests.ValidationsTests.ForEachTests;

public record VitallyModel(List<string> ArrayOfStrings);

public class Level1VitallyForEachValidator : AbstractValidator<VitallyModel>
{
    public Level1VitallyForEachValidator()
    {
        DescribeEnumerable(x => x.ArrayOfStrings)
            .Vitally().IsNotNull()
            .Vitally().ForEach(x => x
                .Vitally().IsNotNull()
                .IsEqualTo("Test")
            );
    }
}

public class Level1ForEachValidator : AbstractValidator<VitallyModel>
{
    public Level1ForEachValidator()
    {
        DescribeEnumerable(x => x.ArrayOfStrings)
            .Vitally().IsNotNull()
            .ForEach(x => x
                .Vitally().IsNotNull()
                .IsEqualTo("Test")
            );
    }
}


public class Level1ForEachMultipleErrorsValidator : AbstractValidator<VitallyModel>
{
    public Level1ForEachMultipleErrorsValidator()
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
    [Fact]
    public async Task GivenAVitallyFailingFirstItem_ThenItDoesntReportAnyAdditionalErrors()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new Level1VitallyForEachValidator());
        var testSubject = new VitallyModel(
            ArrayOfStrings: new()
            {
                null,
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
        }
    }


    [Fact]
    public async Task GivenAVitallyFailingFirstItem_ThatDoesntVitallyFailItsOwnValidation_ReportsEqualToError()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new Level1VitallyForEachValidator());
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
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new Level1ForEachValidator());
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
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new Level1ForEachMultipleErrorsValidator());
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
}
