using ApprovalTests;
using FluentAssertions;
using FluentAssertions.Execution;
using Newtonsoft.Json;
using PopValidations;
using PopValidations_Tests.TestHelpers;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PopValidations_Tests.ValidationsTests.ForEachTests;

public class Level1DepthValidator : AbstractValidator<Level1>
{
    public Level1DepthValidator()
    {
        Describe(x => x.Name).IsEqualTo(nameof(Level1DepthValidator));

        DescribeEnumerable(x => x.Level2Array)
            .Vitally()
            .IsNotNull()
            .ForEach(x => x.SetValidator(new Level2DepthValidator()))
            ;
    }
}

public class Level2DepthValidator : AbstractSubValidator<Level2>
{
    public Level2DepthValidator()
    {
        Describe(x => x.Name).IsEqualTo(nameof(Level2DepthValidator));

        DescribeEnumerable(x => x.Level3Array)
            .Vitally()
            .IsNotNull()
            .ForEach(x => x.SetValidator(new Level3DepthValidator()))
            ;
    }
}

public class Level3DepthValidator : AbstractSubValidator<Level3>
{
    public Level3DepthValidator()
    {
        Describe(x => x.Name).IsEqualTo(nameof(Level3DepthValidator));

        DescribeEnumerable(x => x.Level4Array)
            .Vitally()
            .IsNotNull()
            .ForEach(x => x.SetValidator(new Level4DepthValidator()))
            ;
    }
}

public class Level4DepthValidator : AbstractSubValidator<Level4>
{
    public Level4DepthValidator()
    {
        Describe(x => x.Name).IsEqualTo(nameof(Level4DepthValidator));

        DescribeEnumerable(x => x.Level5Array)
            .Vitally()
            .IsNotNull()
            .ForEach(x => x.SetValidator(new Level5DepthValidator()));
    }
}

public class Level5DepthValidator : AbstractSubValidator<Level5>
{
    public Level5DepthValidator()
    {
        Describe(x => x.Name).IsEqualTo(nameof(Level5DepthValidator));
    }
}

public class ForEachDepthTests
{
    [Fact]
    public async Task Given5LevelsDeepForeachValidations_ItShowsNoErrors()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new Level1DepthValidator());

        var testSubject = new Level1(
            Name: nameof(Level1DepthValidator),
            Level2Array: new()
            {
                new Level2(
                    Name: nameof(Level2DepthValidator),
                    Level3Array: new()
                    {
                        new Level3(
                            Name: nameof(Level3DepthValidator),
                            Level4Array: new()
                            {
                                new Level4(
                                    Name: nameof(Level4DepthValidator),
                                    Level5Array: new()
                                    {
                                        new Level5(nameof(Level5DepthValidator))
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
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new Level1DepthValidator());
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
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new Level1DepthValidator());
        var testSubject = new Level1(
            Name: nameof(Level1DepthValidator),
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
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new Level1DepthValidator());
        var testSubject = new Level1(
            Name: nameof(Level1DepthValidator),
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
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new Level1DepthValidator());
        var testSubject = new Level1(
            Name: nameof(Level1DepthValidator),
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
            , x).ToList()
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
