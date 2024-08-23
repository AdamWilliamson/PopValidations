using ApprovalTests.Namers;

namespace ApiValidations_Tests.TestHelpers;

public static class ApprovalTestsHelpers
{
    public static IDisposable SilentForScenario(string data)
    {
        var name = ApprovalResults.Scrub(data);
        return NamerFactory.AsEnvironmentSpecificTest(name);
    }
}
