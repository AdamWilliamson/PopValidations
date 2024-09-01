using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PopApiValidations.Swashbuckle.Internal;
using PopValidations.Swashbuckle;
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

        //services.AddTransient<ISchemaGenerator, NonRefSchemaGenerator>();
        //options.<NonRefSchemaGenerator>();
    }

    public static IServiceCollection RegisterPopApiValidationsOpenApiDefaults(
        this IServiceCollection services,
        OpenApiConfig? config = null,
        Type? validationRunnerFactoryType = null
    )
    {
        services.AddSingleton(config ?? new OpenApiConfig());
        services.AddTransient(typeof(IApiValidationRunnerFactory), validationRunnerFactoryType ?? typeof(ApiValidationRunnerFactory));
        return services;
    }
}
