using PopValidations.Execution;

namespace PopValidations.Swashbuckle.Internal;

public class ValidationRunnerFactory : IValidationRunnerFactory
{
    private readonly IServiceProvider serviceProvider;

    public ValidationRunnerFactory(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public IValidationDescriber? GetRunner(Type type)
    {
        var validationRunnerType = typeof(IValidationRunner<>).MakeGenericType(type);
        return serviceProvider.GetService(validationRunnerType) as IValidationDescriber;
    }
}
