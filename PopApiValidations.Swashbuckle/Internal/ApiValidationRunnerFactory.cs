using PopValidations.Execution;

namespace PopApiValidations.Swashbuckle.Internal;

public class ApiValidationRunnerFactory : IApiValidationRunnerFactory
{
    private readonly IServiceProvider serviceProvider;

    public ApiValidationRunnerFactory(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public IValidationDescriber? GetRunner(Type type)
    {
        var validationRunnerType = typeof(IValidationRunner<>).MakeGenericType(type);
        return serviceProvider.GetService(validationRunnerType) as IValidationDescriber;
    }
}
