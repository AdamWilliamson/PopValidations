using ApprovalTests;
using PopValidations_Tests.TestHelpers;
using PopValidations_Tests.ValidationsTests.ScopeWhenValidationTests;
using System.Threading.Tasks;
using Xunit;

namespace PopValidations_Tests.ValidationsTests.ScopeValidationTests;

public record Base(string? DependantField);
public record Level1(bool Check, string? DependantField, Level2? Child) : Base(DependantField);
public record Level2(bool Check, string? DependantField, Level3? Child) : Base(DependantField);
public record Level3(bool Check, string? DependantField, Level4? Child) : Base(DependantField);
public record Level4(bool Check, string? DependantField, Level5? Child) : Base(DependantField);
public record Level5(bool Check, string? DependantField) : Base(DependantField);

public class Scope_DemoTest
{
    [Fact]
    public async Task Validation()
    {
        // Arrange
        var validationRunner = ValidationRunnerHelper.BasicRunnerSetup(new Level1Validator());

        // Act
        var validationResult = await validationRunner.Validate(
            new Level1(
                Check: true,
                DependantField: "1",
                Child: new Level2(
                    Check: true,
                    DependantField: "2",
                    //Child: null
                    Child: new Level3(
                        Check: true,
                        DependantField: "3",
                        Child: new Level4(
                            Check: true,
                            DependantField: "4",
                            Child: new Level5(
                                Check: true,
                                DependantField: "5"
                            )
                        )
                    )
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
        var descriptionRunner = ValidationRunnerHelper.BasicRunnerSetup(new Level1Validator());

        // Act
        var descriptionResult = descriptionRunner.Describe();

        // Assert
        Approvals.VerifyJson(JsonConverter.ToJson(descriptionResult));
    }
}
