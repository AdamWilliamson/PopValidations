using PopValidations.Execution.Description;

namespace ApiValidations_Tests.TestHelpers;

public static class DescriptionResultExtensions
{
    public static DescriptionResultAssertions Should(this DescriptionResult instance)
    {
        return new DescriptionResultAssertions(instance);
    }
}

