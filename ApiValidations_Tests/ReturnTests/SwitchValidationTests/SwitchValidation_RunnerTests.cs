using ApprovalTests;
using PopValidations;
using ApiValidations;
using ApiValidations_Tests.TestHelpers;

namespace ApiValidations_Tests.ReturnTests.SwitchValidationTests;

public record Level1(int? NIntegerLevel1, Level2? Level2, List<Level2?>? ArrayOfLevel2)
{ public bool Check() { return true; } }
public record Level2(int? NIntegerLevel2, Level3? Level3, List<Level3?>? ArrayOfLevel3)
{ public bool Check() { return true; } }
public record Level3(int? NIntegerLevel3, Level4? Level4, List<Level4?>? ArrayOfLevel4)
{ public bool Check() { return true; } }
public record Level4(int? NIntegerLevel4)
{ public bool Check() { return true; } }

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
    public static Validator Create() { return new Validator(1); }
    public static Task<Validator> CreateTasked(object value) { return Task.FromResult(new Validator((int)value )); }
}

public class Level1_TestingValidator : ApiValidator<Level1>
{
    public Level1_TestingValidator()
    {
        DescribeFunc(x => x.Check()).Return
            .Switch("Returns 6", () => Task.FromResult(ValidatorCreationService.Create()))
                .Case("Must not be Null", (x) => x == null, "Is null.")
                .Ignore("Must be Null", (x) => x != null)
                .End();

        When("Tst", x => Task.FromResult(x.Level2 is not null), () =>
        {
            Describe(x => x.Level2).SetValidator(new Level2_TestingValidator());
        });

        DescribeEnumerable(x => x.ArrayOfLevel2).ForEach(x => x.SetValidator(new Level2_TestingValidator()));
    }
}

public class Level2_TestingValidator : ApiSubValidator<Level2>
{
    public Level2_TestingValidator()
    {
        DescribeFunc(x => x.Check()).Return
            .Switch("Db Validation must match: ", (context) => ValidatorCreationService.CreateTasked(context[0]))
                .Case("Must not be Null", (x) => x == null, "Is null.")
                .Ignore("Must be Null", (x) => x != null)
                .End();

        When("Tst", x => Task.FromResult(x.Level3 is not null), () =>
        {
            Describe(x => x.Level3).SetValidator(new Level3_TestingValidator());
        });

        DescribeEnumerable(x => x.ArrayOfLevel3).ForEach(x => x.SetValidator(new Level3_TestingValidator()));
    }
}

public class Level3_TestingValidator : ApiSubValidator<Level3>
{
    public Level3_TestingValidator()
    {
        DescribeFunc(x => x.Check()).Return
            .Switch("Db Validation must match: ", (context) => ValidatorCreationService.CreateTasked(context[0]))
                .Case("Must not be Null", (x) => x == null, "Is null.")
                .Ignore("Must be Null", (x) => x != null)
                .End();

        When("Tst", x => Task.FromResult(x.Level4 is not null), () =>
        {
            Describe(x => x.Level4).SetValidator(new Level4_TestingValidator());
        });

        DescribeEnumerable(x => x.ArrayOfLevel4).ForEach(x => x.SetValidator(new Level4_TestingValidator()));
    }
}

public class Level4_TestingValidator : ApiSubValidator<Level4>
{
    public Level4_TestingValidator()
    {
        DescribeFunc(x => x.Check()).Return
            .Switch("Db Validation must match: ", (context) => ValidatorCreationService.CreateTasked(context[0]))
                .Case("Must not be Null", (x) => x == null, "Is null.")
                .Ignore("Must be Null", (x) => x != null)
                .End();
    }
}

public class SwitchValidation_RunnerTests
{
    [Fact]
    public void WhenDescribing_ItReturnsTheValidation()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new Level1_TestingValidator());

        // Act
        var description = runner.Describe();
        
        // Assert
        Approvals.VerifyJson(JsonConverter.ToJson(description));
    }
}
