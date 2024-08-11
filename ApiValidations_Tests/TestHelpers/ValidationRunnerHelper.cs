using PopValidations.Execution;
using PopValidations.ValidatorInternals;

namespace ApiValidations_Tests.TestHelpers
{
    public static class ValidationRunnerHelper
    {
        public static IValidationRunner<TValidationType> BasicRunnerSetup<TValidationType>(IMainValidator<TValidationType> validator)
        {
            return new ValidationRunner<TValidationType>(
                 new List<IMainValidator<TValidationType>>() { validator },
                 new MessageProcessor()
             );
        }
    }
}
