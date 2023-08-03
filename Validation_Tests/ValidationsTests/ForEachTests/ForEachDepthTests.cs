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

namespace PopValidations_Tests.Bugs;

public record Level5(string Name);

public record Level4(string Name, List<Level5> Level5Array);

public record Level3(string Name, List<Level4> Level4Array);

public record Level2(string Name, List<Level3>? Level3Array);

public record Level1(string Name, List<Level2>? Level2Array);

public class Level1Validator : AbstractValidator<Level1>
{
    public Level1Validator()
    {
        Describe(x => x.Name).IsEqualTo(nameof(Level1Validator));

        DescribeEnumerable(x => x.Level2Array)
            .Vitally()
            .NotNull()
            .ForEach(x => x.SetValidator(new Level2Validator()));
    }
}

public class Level2Validator : AbstractSubValidator<Level2>
{
    public Level2Validator()
    {
        Describe(x => x.Name).IsEqualTo(nameof(Level2Validator));

        DescribeEnumerable(x => x.Level3Array)
            .Vitally()
            .NotNull()
            .ForEach(x => x.SetValidator(new Level3Validator()));
    }
}

public class Level3Validator : AbstractSubValidator<Level3>
{
    public Level3Validator()
    {
        Describe(x => x.Name).IsEqualTo(nameof(Level3Validator));

        DescribeEnumerable(x => x.Level4Array)
            .Vitally()
            .NotNull()
            .ForEach(x => x.SetValidator(new Level4Validator()));
    }
}

public class Level4Validator : AbstractSubValidator<Level4>
{
    public Level4Validator()
    {
        Describe(x => x.Name).IsEqualTo(nameof(Level4Validator));

        DescribeEnumerable(x => x.Level5Array)
            .Vitally()
            .NotNull()
            .ForEach(x => x.SetValidator(new Level5Validator()));
    }
}

public class Level5Validator : AbstractSubValidator<Level5>
{
    public Level5Validator()
    {
        Describe(x => x.Name).IsEqualTo(nameof(Level5Validator));
    }
}

public class ForEachDepthTests
{
    [Fact]
    public async Task Given5LevelsDeepForeachValidations_ItShowsNoErrors()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new Level1Validator());
        var testSubject = new Level1(
            Name: nameof(Level1Validator),
            Level2Array: new()
            {
                new Level2(
                    Name: nameof(Level2Validator),
                    Level3Array: new()
                    {
                        new Level3(
                            Name: nameof(Level3Validator),
                            Level4Array: new()
                            {
                                new Level4(
                                    Name: nameof(Level4Validator),
                                    Level5Array: new()
                                    {
                                        new Level5(nameof(Level5Validator))
                                    }
                                )
                            }
                        )
                    }
                )
            }
        );

        // Act
        var validationResult = await runner.Validate(testSubject);

        // Assert
        using (new AssertionScope())
        {
            validationResult.Errors.Should().BeEmpty();
        }
    }

    [Fact]
    public async Task Given5LevelsDeepForeachValidations_ItShowsErrorsAtEachDepth()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new Level1Validator());
        var testSubject = new Level1(
            Name: "Invalid",
            Level2Array: new()
            {
                new Level2(
                    Name: "Invalid",
                    Level3Array: new()
                    {
                        new Level3(
                            Name: "Invalid",
                            Level4Array: new()
                            {
                                new Level4(
                                    Name: "Invalid",
                                    Level5Array: new() 
                                    { 
                                        new Level5("Invalid") 
                                    }
                                )
                            }
                        )
                    }
                )
            }
        );

        // Act
        var validationResult = await runner.Validate(testSubject);

        // Assert
        using (new AssertionScope())
        {
            Approvals.VerifyJson(JsonConvert.SerializeObject(validationResult.Errors));
            validationResult.Errors.Should().NotBeEmpty();
            validationResult.ContainsAnyError("Name");
            validationResult.ContainsAnyError("Level2Array[0].Name");
            validationResult.ContainsAnyError("Level2Array[0].Level3Array[0].Name");
            validationResult.ContainsAnyError("Level2Array[0].Level3Array[0].Level4Array[0].Name");
            validationResult.ContainsAnyError("Level2Array[0].Level3Array[0].Level4Array[0].Level5Array[0].Name");
        }
    }

    [Theory]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(10)]
    public async Task SingleLevelForeachValidations_ItShowsXErrors(int x)
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new Level1Validator());
        var testSubject = new Level1(
            Name: nameof(Level1Validator),
            Level2Array: Enumerable.Repeat(
                new Level2(
                    Name: "Invalid",
                    null
                )
            , x).ToList()
        );

        // Act
        var validationResult = await runner.Validate(testSubject);

        // Assert
        using (new AssertionScope())
        {
            validationResult.Errors.Should().NotBeEmpty();
            validationResult.ContainsNoError("Name");
            for (int i = 0; i < x; i++)
            {
                validationResult.ContainsAnyError($"Level2Array[{i}].Name");
                validationResult.ContainsAnyError($"Level2Array[{i}].Level3Array");
            }
        }
    }

    [Theory]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(10)]
    public async Task MultiLayeredForeachValidations_ItShowsXErrors(int x)
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new Level1Validator());
        var testSubject = new Level1(
            Name: nameof(Level1Validator),
            Level2Array: new()
            {
                new Level2(
                    Name: "Invalid",
                    Level3Array: Enumerable.Repeat(
                        new Level3(
                            Name: "Invalid",
                            null
                        )
                    , x).ToList()
                )
            }
        );

        // Act
        var validationResult = await runner.Validate(testSubject);

        // Assert
        using (new AssertionScope())
        {
            validationResult.Errors.Should().NotBeEmpty();
            validationResult.ContainsNoError("Name");
            for (int i = 0; i < x; i++)
            {
                validationResult.ContainsAnyError($"Level2Array[0].Level3Array[{i}].Name");
                validationResult.ContainsAnyError($"Level2Array[0].Level3Array[{i}].Level4Array");
            }
        }
    }

    [Theory]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(10)]
    public async Task MultipleForeachs_WhenInternalToEachOther_ItShowsX2Errors(int x)
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new Level1Validator());
        var testSubject = new Level1(
            Name: nameof(Level1Validator),
            Level2Array: Enumerable.Repeat(
                new Level2(
                    Name: "Invalid",
                    Level3Array: Enumerable.Repeat(
                        new Level3(
                            Name: "Invalid",
                            null
                        )
                    , x).ToList()
                )
            ,x).ToList()
        );

        // Act
        var validationResult = await runner.Validate(testSubject);

        // Assert
        using (new AssertionScope())
        {
            validationResult.Errors.Should().NotBeEmpty();
            validationResult.ContainsNoError("Name");
            for (int i1 = 0; i1 < x; i1++)
            {
                for (int i2 = 0; i2 < x; i2++)
                {
                    validationResult.ContainsAnyError($"Level2Array[{i1}].Level3Array[{i2}].Name");
                    validationResult.ContainsAnyError($"Level2Array[{i1}].Level3Array[{i2}].Level4Array");
                }
            }
        }
    }
}
