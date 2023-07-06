using Microsoft.AspNetCore.Hosting;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.OpenApi.Writers;
//using Swashbuckle.AspNetCore.Swagger;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Data.Common;
//using Microsoft.VisualStudio.TestPlatform.TestHost;
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
//using Microsoft.AspNetCore.Mvc.Testing;
//using Microsoft.Data.Sqlite;
//using Microsoft.EntityFrameworkCore;
//using RazorPagesProject.Data;

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

        public ApiWebApplicationFactory WithConfig(OpenApiConfig config)
        {
            Config = config;
            return this;
        }

        public ApiWebApplicationFactory AddController<T>()
        {
            AdditionalControllers.Add(typeof(T));
            return this;
        }

        public ApiWebApplicationFactory AddValidator<T, X>()
            where T: IMainValidator<X>
        {
            validators.Add((typeof(IMainValidator<X>), typeof(T)));
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

                // Override to specify custom configs for testing settings.
                services.RegisterPopValidationsOpenApiDefaults(Config);
                
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
