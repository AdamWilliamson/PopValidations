using PopValidations.Execution.Validation;
using PopValidations.Validations.Base;
using PopValidations_Tests.TestHelpers;

namespace FluentAssertions;

public static class ValidationResultExtensions
{
    public static ValidationResultAssertions Should(this ValidationResult instance)
    {
        return new ValidationResultAssertions(instance);
    }
}

