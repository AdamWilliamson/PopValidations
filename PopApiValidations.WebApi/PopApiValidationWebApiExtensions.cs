using Microsoft.Extensions.DependencyInjection;
using ApiValidations.Execution;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PopApiValidations;

public static class PopApiValidationWebApiExtensions
{
    //public static IServiceCollection RegisterApiValidationRunner(this IServiceCollection services)
    //{
    //    PopValidation.RegisterRunner(services);
    //    services.AddTransient(typeof(IApiValidationRunner<>), typeof(ApiValidationRunner<>));
    //    return services;
    //}

    //public static IServiceCollection RegisterAllMainApiValidators(this IServiceCollection services, Assembly assembly)
    //{
    //    PopValidation.RegisterAllMainValidators(services, assembly);

    //    assembly
    //        .GetTypes()
    //        .Where(a => a.GetInterface(typeof(IApiMainValidator<>).Name) != null && !a.IsAbstract && !a.IsInterface)
    //        .Select(a => new { assignedType = a, serviceTypes = a.GetInterfaces().ToList() })
    //        .ToList()
    //        .ForEach(typesToRegister =>
    //        {
    //            typesToRegister.serviceTypes.ForEach(typeToRegister => services.AddTransient(typeToRegister, typesToRegister.assignedType));
    //        });

    //    return services;
    //}


    public static IMvcBuilder AddApiValidationsFilter(this IMvcBuilder builder)
    {
        builder.Services.Configure<MvcOptions>(options =>
        {
            options.Filters.Add(typeof(ApiValidationsAttribute));
        });
        return builder;
    }

    public class ApiValidationsAttribute : ActionFilterAttribute
    {
        public override async void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.ActionArguments.Any() && filterContext.ActionDescriptor == null)
            {
                base.OnActionExecuting(filterContext);
                return;
            }

            var controllerType = filterContext.Controller.GetType();
            var runnerForControllerType = typeof(IApiValidationRunner<>).MakeGenericType(controllerType);

            var runner = filterContext.HttpContext.RequestServices.GetService(runnerForControllerType);
            if (runner == null)
            {
                return;
            }

            var method = runnerForControllerType.GetMethods().FirstOrDefault(x => x.Name == "Validate" && x.GetParameters().Count() == 2);

            if (method == null) return;

            var methodInfo = ((ControllerActionDescriptor)filterContext.ActionDescriptor).MethodInfo;
            var orderedParams = new List<object?>();

            foreach (var p in methodInfo.GetParameters())
            {
                if (p.Name is null || !filterContext.ActionArguments.ContainsKey(p.Name))
                {
                    return;
                }
                else
                {
                    orderedParams.Add(filterContext.ActionArguments[p.Name!]);
                }
            }

            var result = method.Invoke(runner, [filterContext.Controller, new HeirarchyMethodInfo(string.Empty, methodInfo, orderedParams)]) as Task<ApiValidationResult>;
            if (result != null)
            {
                var validations = await result;
                if (validations.Errors.Any())
                {
                    foreach (var validation in validations.Errors)
                    {
                        filterContext.ModelState.AddModelError(validation.Key, validation.Value.FirstOrDefault() ?? "Error");
                    }
                    filterContext.Result = new ObjectResult
                        (
                            new ValidationProblemDetails(filterContext.ModelState)
                            {
                                Detail = null,
                                Instance = null,
                                Status = null,
                                Title = null,
                                Type = null,
                            }
                        )
                        {
                            StatusCode = StatusCodes.Status422UnprocessableEntity
                        };
                    

                    return;
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}

