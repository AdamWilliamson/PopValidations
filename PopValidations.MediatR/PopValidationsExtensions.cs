using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace PopValidations.MediatR;

public static class PopValidationsExtensions
{
    //public static IServiceCollection AddPopValidation(
    //    this IServiceCollection services,
    //    ServiceLifetime lifetime = ServiceLifetime.Transient
    //)
    //{
    //    services.Add(new ServiceDescriptor(typeof(IPipelineBehavior<,>), typeof(PopValidationBehavior<,>), lifetime));

    //    return services;
    //}

    public static MediatRServiceConfiguration AddPopValidations(this MediatRServiceConfiguration config)
    {
        config
            .AddOpenBehavior(typeof(PopValidationBehavior<,>));

        return config;
    }
}