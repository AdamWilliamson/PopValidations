using ApprovalTests;
using ApprovalTests.Namers;
using PopValidations.Swashbuckle_Tests.DisabledTests;
using PopValidations.Swashbuckle_Tests.Helpers;
using Xunit;

namespace PopValidations.Swashbuckle_Tests.ConfigTests;

public class FilteringOpenApiConfigTests
{
    //TODO: These dont work???

    [Theory]
    [ClassData(typeof(ConfigSetups))]
    public async Task FilteringParentTest(ITestSetup setup)
    {
        // Arrange
        var config = new SimpleFilterConfig(setup.RequestType);

        // Act
        var helper = await setup.GetHelper(config);

        // Assert
        using (ApprovalResults.ForScenario($"{setup.Scenario}"))
        {
            Approvals.VerifyJson(helper.Content);
        }
    }

    [Theory]
    [ClassData(typeof(ConfigSetups))]
    public async Task FilteringChildTest(ITestSetup setup)
    {
        // Arrange
        var config = new SimpleFilterConfig(setup.RequestType.GetProperty("Child").PropertyType);

        // Act
        var helper = await setup.GetHelper(config);

        // Assert
        using (ApprovalResults.ForScenario($"{setup.Scenario}"))
        {
            Approvals.VerifyJson(helper.Content);
        }
    }
}
