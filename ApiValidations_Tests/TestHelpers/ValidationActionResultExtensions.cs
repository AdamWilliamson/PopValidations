using PopValidations.Validations.Base;

namespace PopValidations_Tests.TestHelpers;

public static class ValidationActionResultExtensions
{
    public static ValidationActionResultAssertions Should(this ValidationActionResult instance)
    {
        return new ValidationActionResultAssertions(instance);
    }
}

