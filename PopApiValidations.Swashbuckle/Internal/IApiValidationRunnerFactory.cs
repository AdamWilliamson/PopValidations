
using PopValidations.Execution;

namespace PopApiValidations.Swashbuckle.Internal;

public interface IApiValidationRunnerFactory
{
    IValidationDescriber? GetRunner(Type type);
}
