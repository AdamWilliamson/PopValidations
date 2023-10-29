using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using PopValidations.Swashbuckle;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using PopValidations.ValidatorInternals;

namespace PopValidations.Swashbuckle_Tests
{
    internal class ApiWebApplicationFactory : WebApplicationFactory<Program>
    {
        List<Type> AdditionalControllers = new();
        OpenApiConfig? Config;
        List<(Type, Type)> validators = new();
        List<(Type, object)> registeredValues = new();

        public ApiWebApplicationFactory Register(Type t, object o) 
        {
            registeredValues.Add((t, o));
            return this;
        }

        public ApiWebApplicationFactory WithConfig(OpenApiConfig config)
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
            where TValidator : IMainValidator<TRequest>
        {
            AddValidator(typeof(IMainValidator<TRequest>), typeof(TValidator));
            return this;
        }

        public ApiWebApplicationFactory AddValidator(Type validator, Type request)
        {
            validators.Add((validator, request));
            return this;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            //builder.WithAdditionalControllers(typeof(MvcOptionsTestController));
            if (AdditionalControllers?.Any() == true)
            {
                builder.WithAdditionalControllers(AdditionalControllers.ToArray());
            }
            builder.ConfigureAppConfiguration(config => {});

            builder.ConfigureTestServices(services => 
            {
                services.AddSwaggerGenNewtonsoftSupport();

                foreach (var item in registeredValues)
                {
                    services.AddSingleton(item.Item1, item.Item2);
                }
                // Override to specify custom configs for testing settings.
                services.RegisterPopValidationsOpenApiDefaults(Config);
                //services.AddTransient(
                //    typeof(IMainValidator<NotNullTests.NotNullBaseTests.Request>),
                //    typeof(RequestValidator));

                foreach (var val in validators) 
                {
                    services.AddTransient(val.Item1, val.Item2);
                }
            });
        }
    }
}
