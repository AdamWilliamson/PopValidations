using ApiValidations.Execution;

namespace PopApiValidations.Swashbuckle.Internal;

public class ApiValidationRunnerFactory : IApiValidationRunnerFactory
{
    private readonly IServiceProvider serviceProvider;

    public ApiValidationRunnerFactory(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public IApiValidationDescriber? GetRunner(Type type)
    {
        var validationRunnerType = typeof(IApiValidationRunner<>).MakeGenericType(type);
        return serviceProvider.GetService(validationRunnerType) as IApiValidationDescriber;
    }
}
