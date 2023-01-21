using PopValidations.Execution;

namespace PopValidations.Swashbuckle.Internal;

public interface IValidationRunnerFactory
{
    IValidationDescriber? GetRunner(Type type);
}
