using PopValidations.Validations.Base;
using PopValidations_Tests.TestHelpers;

namespace FluentAssertions;

public static class ValidationActionResultExtensions
{
    public static ValidationActionResultAssertions Should(this ValidationActionResult instance)
    {
        return new ValidationActionResultAssertions(instance);
    }
}

