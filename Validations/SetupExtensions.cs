using Microsoft.Extensions.DependencyInjection;
using PopValidations.Execution;
using PopValidations.ValidatorInternals;
using System.Linq;
using System.Reflection;

namespace PopValidations;
public static class PopValidation
{
    public static IServiceCollection RegisterRunner(this IServiceCollection services)
    {
        services.AddSingleton(typeof(MessageProcessor));
        services.AddTransient(typeof(IValidationRunner<>), typeof(ValidationRunner<>));
        return services;
    }

    public static IServiceCollection RegisterAllMainValidators(this IServiceCollection services, Assembly assembly)
    {
        assembly
            .GetTypes()
            .Where(a => a.GetInterface(typeof(IMainValidator<>).Name) != null && !a.IsAbstract && !a.IsInterface)
            .Select(a => new { assignedType = a, serviceTypes = a.GetInterfaces().ToList() })
            .ToList()
            .ForEach(typesToRegister =>
            {
                typesToRegister.serviceTypes.ForEach(typeToRegister => services.AddTransient(typeToRegister, typesToRegister.assignedType));
            });

        return services;
    }
}
