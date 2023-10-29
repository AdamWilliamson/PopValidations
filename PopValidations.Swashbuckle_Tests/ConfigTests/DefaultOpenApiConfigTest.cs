using ApprovalTests;
using ApprovalTests.Namers;
using PopValidations.Swashbuckle_Tests.Helpers;
using Xunit;

namespace PopValidations.Swashbuckle_Tests.ConfigTests;

public class DefaultOpenApiConfigTests
{
    [Theory]
    [ClassData(typeof(ConfigSetups))]
    public async Task DefaultSettingsTest(ITestSetup setup)
    {
        // Arrange
        var config = new WebApiConfig();

        // Act
        var helper = await setup.GetHelper(config);

        // Assert
        using (ApprovalResults.ForScenario($"{setup.Scenario}"))
        {
            Approvals.VerifyJson(helper.Content);
        }
    }
}
