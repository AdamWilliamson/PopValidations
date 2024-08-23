using ApiValidations_Tests.TestHelpers;
using PopValidations.Execution.Description;

namespace FluentAssertions;

public static class DescriptionResultExtensions
{
    public static DescriptionResultAssertions Should(this DescriptionResult instance)
    {
        return new DescriptionResultAssertions(instance);
    }
}

