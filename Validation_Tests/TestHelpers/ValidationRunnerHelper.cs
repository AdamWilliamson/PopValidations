using PopValidations.Execution;
using PopValidations.ValidatorInternals;
using System.Collections.Generic;

namespace PopValidations_Tests.TestHelpers
{
    internal static class ValidationRunnerHelper
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
