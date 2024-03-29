﻿using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Reflection;

namespace PopValidations.Swashbuckle_Tests
{
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
}
