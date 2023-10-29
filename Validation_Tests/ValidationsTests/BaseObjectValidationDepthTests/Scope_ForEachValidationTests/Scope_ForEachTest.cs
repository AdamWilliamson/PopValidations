using ApprovalTests;
using PopValidations_Tests.TestHelpers;
using System.Threading.Tasks;
using Xunit;

namespace PopValidations_Tests.ValidationsTests.BaseObjectValidationDepthTests.Scope_ForEachValidationTests;

public class Scope_ForEachTest
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
                Child: new()
                {
                    new Level2(
                        Check: true,
                        DependantField: "2",
                        Child: new()
                        {
                            new Level3(
                                Check: true,
                                DependantField: "3",
                                Child: new()
                                {
                                    new Level4(
                                        Check: true,
                                        DependantField: "4",
                                        Child: new()
                                        {
                                            new Level5(
                                                Check: false,
                                                DependantField: "5"
                                            ),
                                            new Level5(
                                                Check: true,
                                                DependantField: "5.1"
                                            )
                                        }
                                    )
                                }
                            ),
                            new Level3(
                                Check: true,
                                DependantField: "3.1",
                                Child: new()
                                {
                                    new Level4(
                                        Check: true,
                                        DependantField: "4",
                                        Child: null
                                    )
                                }
                            )
                        }
                    )
                }
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
