using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PopApiValidations.Swashbuckle.Internal;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PopApiValidations.Swashbuckle;

public static class SwaggerIntegrationExtensions
{
    public static void RegisterApiValidationOpenApiFilter(this SwaggerGenOptions options)
    {
        options.OperationFilter<PopApiValidationSchemaFilter>();
    }

    public static void RegisterApiValidationPerEndpointDefinitionsFilter(this IServiceCollection services)
    {
        var descriptor =new ServiceDescriptor(
            typeof(ISchemaGenerator),
            typeof(NonRefSchemaGenerator),
            ServiceLifetime.Transient
        );
        services.Replace(descriptor);
    }

    public static IServiceCollection RegisterPopApiValidationsOpenApiDefaults(
        this IServiceCollection services,
        PopApiOpenApiConfig? config = null,
        Type? validationRunnerFactoryType = null
    )
    {
        services.AddSingleton(config ?? new PopApiOpenApiConfig());
        services.AddTransient(typeof(IApiValidationRunnerFactory), validationRunnerFactoryType ?? typeof(ApiValidationRunnerFactory));
        return services;
    }
}
