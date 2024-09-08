using ApiValidations;
using ApiValidations.Execution;
using PopValidations.Execution;
using PopValidations.ValidatorInternals;

namespace ApiValidations_Tests.TestHelpers
{
    public static class ValidationRunnerHelper
    {
        public static IApiValidationRunner<TValidationType> BasicRunnerSetup<TValidationType>(IApiMainValidator<TValidationType> validator)
        {
            return new ApiValidationRunner<TValidationType>(
                new List<IApiMainValidator<TValidationType>>() { validator },
                new MessageProcessor()
                //new ValidationRunner<TValidationType>(
                //     new List<IMainValidator<TValidationType>>() { validator as IMainValidator<TValidationType> ?? throw new Exception("Api Validator isnt a IMainValidator") },
                     
                // ),
                
            );
        }
    }
}
