using ApprovalTests;
using ApiValidations_Tests.TestHelpers;

namespace ApiValidations_Tests.ParamTests.ScopeValidationTests.Scope_ForEachValidationTests;

public class Scope_ForEachTest
{
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
