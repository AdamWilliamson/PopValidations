
using ApiValidations.Execution;

namespace PopApiValidations.Swashbuckle.Internal;

public interface IApiValidationRunnerFactory
{
    IApiValidationDescriber? GetRunner(Type type);
}
