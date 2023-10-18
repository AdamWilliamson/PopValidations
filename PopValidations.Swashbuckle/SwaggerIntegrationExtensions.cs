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
        services.AddSwaggerGenNewtonsoftSupport();
        services.AddSingleton(config ?? new OpenApiConfig());
        services.AddTransient(typeof(IValidationRunnerFactory), validationRunnerFactoryType ?? typeof(ValidationRunnerFactory));
        return services;
    }
}
