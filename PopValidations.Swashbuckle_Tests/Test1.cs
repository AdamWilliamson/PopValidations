using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Xunit;
using FluentAssertions;
using PopValidations.Swashbuckle;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using PopValidations.ValidatorInternals;

namespace PopValidations.Swashbuckle_Tests
{
    internal static class WebHostBuilderExtensions
    {
        public static IWebHostBuilder WithAdditionalControllers(this IWebHostBuilder builder, params Type[] controllers)
        {
            return builder.ConfigureTestServices(
                services =>
                {
                    var partManager = GetApplicationPartManager(services);

                    partManager.FeatureProviders.Add(new ExternalControllersFeatureProvider(controllers));
                });
        }

        private static ApplicationPartManager GetApplicationPartManager(IServiceCollection services)
        {
            var partManager = (ApplicationPartManager)services
                .Last(descriptor => descriptor.ServiceType == typeof(ApplicationPartManager))
                .ImplementationInstance;
            return partManager;
        }

        private class ExternalControllersFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
        {
            private readonly Type[] _controllers;

            public ExternalControllersFeatureProvider(params Type[] controllers)
            {
                _controllers = controllers ?? Array.Empty<Type>();
            }

            public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
            {
                feature.Controllers.Clear();
                foreach (var controller in _controllers)
                {
                    feature.Controllers.Add(controller.GetTypeInfo());
                }
            }
        }
    }

    public class ExternalControllersFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        private readonly Type[] _controllers;

        public ExternalControllersFeatureProvider(params Type[] controllers)
        {
            _controllers = controllers ?? Array.Empty<Type>();
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            foreach (var controller in _controllers)
            {
                feature.Controllers.Add(controller.GetTypeInfo());
            }
        }
    }

    public class TestApiConfig : OpenApiConfig
    {
        public TestApiConfig()
        {
            TypeValidationLevel = (Type t) =>
                (
                    ValidationLevel.None
                );
        }
    }

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
                foreach(var item in registeredValues)
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

    public class Test1
    {
        [Fact]
        public async Task Test()
        {
            var factory = new ApiWebApplicationFactory()
                .AddController<MvcOptionsTestController>();
            var client = factory.CreateClient();
            
            var json = await client.GetAsync("/swagger/v1/swagger.json");

            var content = (await json.Content.ReadAsStringAsync());
            content.Should().NotBeNull();
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class MvcOptionsTestController : Controller
    {
        [HttpGet("test")]
        public object Get([FromQuery] Request request)
        {
            return request;
        }
    }

    public class Request
    {
        public string Id { get; set; }

        public string OtherId { get; set; }
    }
}
