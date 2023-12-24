using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

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
            var partManager = services
                .Last(descriptor => descriptor.ServiceType == typeof(ApplicationPartManager))
                ?.ImplementationInstance as ApplicationPartManager;

            if (partManager == null) throw new Exception("Part manager is not correct..  How?");
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
}
