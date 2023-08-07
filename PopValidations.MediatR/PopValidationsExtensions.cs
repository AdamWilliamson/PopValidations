using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace PopValidations.MediatR;

public static class PopValidationsExtensions
{
    public static MediatRServiceConfiguration AddPopValidations(this MediatRServiceConfiguration config)
    {
        config
            .AddOpenBehavior(typeof(PopValidationBehavior<,>));

        return config;
    }
}