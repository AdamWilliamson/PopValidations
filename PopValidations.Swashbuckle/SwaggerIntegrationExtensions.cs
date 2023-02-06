using Microsoft.Extensions.DependencyInjection;
using PopValidations.Swashbuckle.Internal;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PopValidations.Swashbuckle;

public static class SwaggerIntegrationExtensions
{
    public static void RegisterOpenApiModificationFilter(this SwaggerGenOptions options)
    {
        options.SchemaFilter<PopValidationSchemaFilter>();
    }

    public static IServiceCollection RegisterPopValidationsOpenApiDefaults(
        this IServiceCollection services, 
        OpenApiConfig? config = null,
        Type? validationRunnerFactoryType = null
    )
    {
        services.AddSingleton(config ?? new OpenApiConfig());
        services.AddSingleton(typeof(IValidationRunnerFactory), validationRunnerFactoryType ?? typeof(ValidationRunnerFactory));
        return services;
    }
}
