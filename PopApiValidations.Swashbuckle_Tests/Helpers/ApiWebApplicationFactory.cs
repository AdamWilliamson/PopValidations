using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using PopApiValidations.Swashbuckle;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ApiValidations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using PopApiValidations.ExampleWebApi.Controllers;

namespace PopApiValidations.Swashbuckle_Tests.Helpers;

internal class ApiWebApplicationFactory : WebApplicationFactory<Program>
{
    List<Type> AdditionalControllers = new();
    PopApiOpenApiConfig? Config;
    List<(Type, Type)> validators = new();
    List<(Type, Func<IServiceProvider, object>)> realizedValidators = new();
    List<(Type, object)> registeredValues = new();
    List<(Type Interface, Type Old, object Instance)> replaceRegisteredValues = new();

    public ApiWebApplicationFactory Register(Type t, object o) 
    {
        registeredValues.Add((t, o));
        return this;
    }

    public ApiWebApplicationFactory WithConfig(PopApiOpenApiConfig config)
    {
        Config = config;
        return this;
    }

    public ApiWebApplicationFactory AddController<T>()
    {
        AddController(typeof(T));
        return this;
    }
    public ApiWebApplicationFactory AddController(Type type)
    {
        AdditionalControllers.Add(type);
        return this;
    }

    public ApiWebApplicationFactory AddValidator<TValidator, TRequest>()
        where TValidator : IApiMainValidator<TRequest>
    {
        AddValidator(typeof(IApiMainValidator<TRequest>), typeof(TValidator));
        return this;
    }

    public ApiWebApplicationFactory AddValidator(Type validator, Type request)
    {
        validators.Add((validator, request));
        return this;
    }

    public ApiWebApplicationFactory AddRealizedValidator(Type validator, Func<IServiceProvider, object> implementationFactory)
    {
        if (implementationFactory != null) realizedValidators.Add((validator, implementationFactory));

        return this;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        if (AdditionalControllers?.Any() == true)
        {
            builder.WithAdditionalControllers(AdditionalControllers.ToArray());
        }
        
        builder.ConfigureAppConfiguration(config => {});

        builder.ConfigureTestServices(services => 
        {
            if (Config != null)
            {
                services.AddSingleton(typeof(PopApiOpenApiConfig), Config);
            }
            else
            {
                services.AddSingleton(typeof(PopApiOpenApiConfig), new PopApiOpenApiConfig()
                {
                    ValidateEndpoint = (x) => false
                });
            }

            services.AddSwaggerGenNewtonsoftSupport();

            foreach (var item in registeredValues)
            {
                services.AddSingleton(item.Item1, item.Item2);
            }

            // Override to specify custom configs for testing settings.
            //services.RegisterPopApiValidationsOpenApiDefaults(Config);

            foreach (var val in validators) 
            {
                services.AddTransient(val.Item1, val.Item2);
            }

            foreach(var val in realizedValidators)
            {
                services.AddTransient(val.Item1, val.Item2);
            }

            foreach( var replacement in replaceRegisteredValues)
            {
                services.Remove(new ServiceDescriptor(replacement.Interface, replacement.Old, ServiceLifetime.Transient));
                services.Add(new ServiceDescriptor(replacement.Interface, replacement.Instance, ServiceLifetime.Singleton));
            }
        });
    }

    internal void ReplaceRegister<TInterface, TOld, TNew>(TNew instance)
    {
        replaceRegisteredValues.Add((typeof(TInterface), typeof(TOld), instance));
    }
}
