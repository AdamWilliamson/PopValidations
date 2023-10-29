using ApprovalTests;
using ApprovalTests.Namers;
using PopValidations.Swashbuckle_Tests.DisabledTests;
using PopValidations.Swashbuckle_Tests.Helpers;
using Xunit;

namespace PopValidations.Swashbuckle_Tests.ConfigTests;

public class DisabledOpenApiConfigTests
{
    [Theory]
    [ClassData(typeof(ConfigSetups))]
    public async Task DisabledTest(ITestSetup setup)
    {
        // Arrange
        var config = new DisabledConfig();

        // Act
        var helper = await setup.GetHelper(config);

        // Assert
        using (ApprovalResults.ForScenario($"{setup.Scenario}"))
        {
            Approvals.VerifyJson(helper.Content);
        }
    }
}
