using ApprovalTests;
using PopValidations;
using PopValidations_Tests.TestHelpers;
using System.Threading.Tasks;
using Xunit;

namespace PopValidations_Tests.ValidationsTests.WhenValidationTests.When_IncludeInsideForEachWhenValidationTests;

public static class DataRetriever
{
    public static Task<string> GetValue(Base v)
    {
        return Task.FromResult(v?.DependantField + " GetValue");
    }

    public static Task<string> GetMoreValue(Base v)
    {
        return Task.FromResult(v?.DependantField + " GetMoreValue");
    }
}

public record Base(string? DependantField);
public record Level1(bool Check, string? DependantField, Level2? Child) : Base(DependantField);
public record Level2(bool Check, string? DependantField, Level3? Child) : Base(DependantField);
public record Level3(bool Check, string? DependantField, Level4? Child) : Base(DependantField);
public record Level4(bool Check, string? DependantField, Level5? Child) : Base(DependantField);
public record Level5(bool Check, string? DependantField);

public class Validator : AbstractValidator<Level1>
{
    public Validator()
    {
        When(
           "When Check is True",
           x => Task.FromResult(x.Child != null),
           () =>
           {
               Describe(x => x.DependantField).IsEqualTo(1);

               Include(new SubValidator());
           }
        );
    }
}

public class SubValidator : AbstractSubValidator<Level1>
{
    public SubValidator()
    {
        Describe(x => x.DependantField).IsEqualTo("0");

        When(
            "When 10 == 10",
            x => Task.FromResult(x.Check == true),
            () =>
            {
                Describe(x => x.DependantField).IsEqualTo(2);
            }
        );
    }
}

public class When_IncludeForEachScopeTest
{
    [Fact]
    public async Task Validation()
    {
        // Arrange
        var validationRunner = ValidationRunnerHelper.BasicRunnerSetup(new Validator());

        // Act
        var validationResult = await validationRunner.Validate(
            new Level1(
                Check: true,
                DependantField: "1",
                Child: new Level2(
                    Check: true,
                    DependantField: "2",
                    Child: null
                )
            )
        );

        // Assert
        Approvals.VerifyJson(JsonConverter.ToJson(validationResult));
    }

    [Fact]
    public void Description()
    {
        // Arrange
        var descriptionRunner = ValidationRunnerHelper.BasicRunnerSetup(new Validator());

        // Act
        var descriptionResult = descriptionRunner.Describe();

        // Assert
        Approvals.VerifyJson(JsonConverter.ToJson(descriptionResult));
    }
}
