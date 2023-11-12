
using ApprovalTests;
using FluentAssertions;
using PopValidations;
using PopValidations_Tests.TestHelpers;
using PopValidations_Tests.ValidationsTests.GenericTestableObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PopValidations_Tests.ValidationsTests.SwitchValidationTests;

public record Level1(int? NIntegerLevel1, Level2 Level2, List<Level2> ArrayOfLevel2);
public record Level2(int? NIntegerLevel2, Level3 Level3, List<Level3> ArrayOfLevel3);
public record Level3(int? NIntegerLevel3, Level4 Level4, List<Level4> ArrayOfLevel4);
public record Level4(int? NIntegerLevel4);

public class Validator
{
    private readonly int? nlevelInt;

    public Validator(int? NlevelInt)
    {
        nlevelInt = NlevelInt;
    }

    public bool IsNull => nlevelInt is null;
    public bool IsTooBig => nlevelInt > 20;
    public bool IsBelowZero => nlevelInt < 0;

}

public static class ValidatorCreationService
{
    public static Validator Create(int? NLevelInt) { return new Validator(NLevelInt); }
    public static Task<Validator> CreateTasked(int? NLevelInt) { return Task.FromResult(new Validator(NLevelInt)); }
}

public class Level1_TestingValidator : AbstractValidator<Level1>
{
    public Level1_TestingValidator()
    {
        Switch("Returns 6", x => ValidatorCreationService.Create(x.NIntegerLevel1))
            .Case(x => x.NIntegerLevel1, "Must not be Null", (x, data) => data is { IsNull: true } && x is null, "Is null.")
            .Case(x => x.NIntegerLevel1, "Must be above 0 and not null", (x, data) => data is { IsBelowZero: true } && x is not null, "Is below 0.")
            .Case(x => x.NIntegerLevel1, "Must be below  20", (x, data) => data is { IsTooBig: true } && x is not null, "Is too big.")
        ;

        When("Tst", x => Task.FromResult(x.Level2 is not null), () =>
        {
            Describe(x => x.Level2).SetValidator(new Level2_TestingValidator());
        });

        DescribeEnumerable(x => x.ArrayOfLevel2).ForEach(x => x.SetValidator(new Level2_TestingValidator()));
    }
}

public class Level2_TestingValidator : AbstractSubValidator<Level2>
{
    public Level2_TestingValidator()
    {
        Switch("Returns 6", x => ValidatorCreationService.CreateTasked(x.NIntegerLevel2))
            .Case(x => x.NIntegerLevel2, "Must not be Null", (x, data) => data is { IsNull: true } && x is null, "Is null.")
            .Case(x => x.NIntegerLevel2, "Must be above 0 and not null", (x, data) => data is { IsBelowZero: true } && x is not null, "Is below 0.")
            .Case(x => x.NIntegerLevel2, "Must be below  20", (x, data) => data is { IsTooBig: true } && x is not null, "Is too big.")
        ;

        When("Tst", x => Task.FromResult(x.Level3 is not null), () =>
        {
            Describe(x => x.Level3).SetValidator(new Level3_TestingValidator());
        });

        DescribeEnumerable(x => x.ArrayOfLevel3).ForEach(x => x.SetValidator(new Level3_TestingValidator()));
    }
}

public class Level3_TestingValidator : AbstractSubValidator<Level3>
{
    public Level3_TestingValidator()
    {
        Switch("Returns 6", x => ValidatorCreationService.Create(x.NIntegerLevel3))
            .Case(x => x.NIntegerLevel3, "Must not be Null", (x, data) => data is { IsNull: true } && x is null, "Is null.")
            .Case(x => x.NIntegerLevel3, "Must be above 0 and not null", (x, data) => data is { IsBelowZero: true } && x is not null, "Is below 0.")
            .Case(x => x.NIntegerLevel3, "Must be below  20", (x, data) => data is { IsTooBig: true } && x is not null, "Is too big.")
        ;

        When("Tst", x => Task.FromResult(x.Level4 is not null), () =>
        {
            Describe(x => x.Level4).SetValidator(new Level4_TestingValidator());
        });

        DescribeEnumerable(x => x.ArrayOfLevel4).ForEach(x => x.SetValidator(new Level4_TestingValidator()));
    }
}

public class Level4_TestingValidator : AbstractSubValidator<Level4>
{
    public Level4_TestingValidator()
    {
        Switch("Returns 6", x => ValidatorCreationService.CreateTasked(x.NIntegerLevel4))
            .Case(x => x.NIntegerLevel4, "Must not be Null", (x, data) => data is { IsNull: true } && x is null, "Is null.")
            .Case(x => x.NIntegerLevel4, "Must be above 0 and not null", (x, data) => data is { IsBelowZero: true } && x is not null, "Is below 0.")
            .Case(x => x.NIntegerLevel4, "Must be below  20", (x, data) => data is { IsTooBig: true } && x is not null, "Is too big.")
        ;
    }
}

public class SwitchValidation_RunnerTests
{
    [Fact]
    public async Task GivenAValidator_WithNoErrors_ThenNoErrorsAreProduced()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new Level1_TestingValidator());
        var rand = new Random();

        int? ValidRandomValues()
        {
            return new int?[]{ 1, 10, 15 }[rand.Next(2)];
        }

        // Act
        var validationResult = await runner.Validate(
            new Level1(
                NIntegerLevel1: ValidRandomValues(),
                Level2: new Level2(
                    NIntegerLevel2: ValidRandomValues(),
                    Level3: new Level3(
                        NIntegerLevel3: ValidRandomValues(),
                        Level4: new Level4(NIntegerLevel4: ValidRandomValues()),
                        ArrayOfLevel4: new() { new Level4(NIntegerLevel4: ValidRandomValues()), new Level4(NIntegerLevel4: ValidRandomValues()) }
                    ),
                    ArrayOfLevel3: new() { new Level3(ValidRandomValues(), null,null), new Level3(ValidRandomValues(), null,null) }
                ),
                ArrayOfLevel2: new() { 
                    new Level2(ValidRandomValues(), null, 
                        ArrayOfLevel3: new() { 
                            new Level3(ValidRandomValues(), null, ArrayOfLevel4: new() { new Level4(NIntegerLevel4: ValidRandomValues()), new Level4(NIntegerLevel4: ValidRandomValues()) }), 
                            new Level3(ValidRandomValues(), null, null) 
                        }), 
                    new Level2(ValidRandomValues(), null, null) 
                }
            )
        );

        // Assert
        validationResult.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task GivenAValidator_WithErrors_ThenEveryFieldHasErrors()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new Level1_TestingValidator());
        var rand = new Random();

        int? InValidRandomValues()
        {
            return new int?[] { null, 30, -1 }[rand.Next(2)];
        }

        // Act
        var validationResult = await runner.Validate(
            new Level1(
                NIntegerLevel1: InValidRandomValues(),
                Level2: new Level2(
                    NIntegerLevel2: InValidRandomValues(),
                    Level3: new Level3(
                        NIntegerLevel3: InValidRandomValues(),
                        Level4: new Level4(NIntegerLevel4: InValidRandomValues()),
                        ArrayOfLevel4: new() { new Level4(NIntegerLevel4: InValidRandomValues()), new Level4(NIntegerLevel4: InValidRandomValues()) }
                    ),
                    ArrayOfLevel3: new() { new Level3(InValidRandomValues(), null, null), new Level3(InValidRandomValues(), null, null) }
                ),
                ArrayOfLevel2: new()
                {
                    new Level2(InValidRandomValues(), null,
                        ArrayOfLevel3: new()
                        {
                            new Level3(InValidRandomValues(), null, ArrayOfLevel4: new() { new Level4(InValidRandomValues()), new Level4(InValidRandomValues()) }),
                            new Level3(InValidRandomValues(), null, null)
                        }),
                    new Level2(InValidRandomValues(), null, null)
                }
            )
        );
        var json = JsonConverter.ToJson(validationResult);

        // Assert
        validationResult.Errors.Should().HaveCount(14);
    }
}
