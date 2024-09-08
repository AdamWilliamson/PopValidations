using Microsoft.Extensions.DependencyInjection;
using ApiValidations.Execution;
using System.Reflection;
using PopValidations;
using PopValidations.ValidatorInternals;
using PopValidations.Execution;

namespace ApiValidations;

public static class PopApiValidationExtensions
{
    public static IServiceCollection RegisterApiValidationRunner(this IServiceCollection services)
    {
        //PopValidation.RegisterRunner(services);
        services.AddSingleton(typeof(MessageProcessor));
        //services.AddScoped(typeof(IValidationRunner<>), typeof(ValidationRunner<>));
        services.AddTransient(typeof(IApiValidationRunner<>), typeof(ApiValidationRunner<>));
        return services;
    }

    public static IServiceCollection RegisterAllMainApiValidators(this IServiceCollection services, Assembly assembly)
    {
        //PopValidation.RegisterAllMainValidators(services, assembly);

        //assembly
        //    .GetTypes()
        //    .Where(a => a.GetInterface(typeof(IApiMainValidator<>).Name) != null && !a.IsAbstract && !a.IsInterface)
        //    .Select(a => new { assignedType = a, apiValidatorType = a.GetInterface(typeof(IApiMainValidator<>).Name), objValidatorType = a.GetInterface(typeof(IMainValidator<>).Name) })
        //    .ToList()
        //    .ForEach(typesToRegister =>
        //    {
        //        services.AddScoped(typesToRegister.apiValidatorType, typesToRegister.assignedType);
        //        services.AddScoped(typesToRegister.objValidatorType, (sp) => sp.GetService(typesToRegister.apiValidatorType));
        //        //typesToRegister.serviceTypes.ForEach(typeToRegister => );
        //    });

        assembly
            .GetTypes()
            .Where(a => a.GetInterface(typeof(IApiMainValidator<>).Name) != null && !a.IsAbstract && !a.IsInterface)
            .Select(a => new { assignedType = a, serviceTypes = a.GetInterfaces().ToList() })
            .ToList()
            .ForEach(typesToRegister =>
            {
                typesToRegister.serviceTypes.ForEach(typeToRegister => services.AddTransient(typeToRegister, typesToRegister.assignedType));
            });

        return services;
    }


    //public static IMvcBuilder AddApiValidationsFilter(this IMvcBuilder builder)
    //{
    //    builder.Services.Configure<MvcOptions>(options =>
    //    {
    //        options.Filters.Add(typeof(ApiValidationsAttribute));
    //    });
    //    return builder;
    //}

    //public class ApiValidationsAttribute : ActionFilterAttribute
    //{
    //    public override async void OnActionExecuting(ActionExecutingContext filterContext)
    //    {
    //        var controllerType = filterContext.Controller.GetType();
    //        var runnerForControllerType = typeof(IApiValidationRunner<>).MakeGenericType(controllerType);

    //        var runner = filterContext.HttpContext.RequestServices.GetService(runnerForControllerType);
    //        if (runner == null)
    //        {
    //            return;
    //        }

    //        var method = runnerForControllerType.GetMethods().FirstOrDefault(x => x.Name == "Validate" && x.GetParameters().Count() == 2);

    //        if (method == null) return;

    //        var methodInfo = ((ControllerActionDescriptor)filterContext.ActionDescriptor).MethodInfo;
    //        var orderedParams = new List<object?>();

    //        foreach (var p in methodInfo.GetParameters())
    //        {
    //            if (p.Name is null || !filterContext.ActionArguments.ContainsKey(p.Name))
    //            {
    //                return;
    //            }
    //            else
    //            {
    //                orderedParams.Add(filterContext.ActionArguments[p.Name!]);
    //            }
    //        }

    //        var result = method.Invoke(filterContext.Controller, [new HeirarchyMethodInfo(string.Empty, methodInfo, orderedParams)]) as Task<ApiValidationResult>;
    //        if (result != null)
    //        {
    //            var validations = await result;
    //            if (validations.Errors.Any())
    //            {
    //                foreach (var validation in validations.Errors)
    //                {
    //                    filterContext.ModelState.AddModelError(validation.Key, validation.Value.FirstOrDefault() ?? "Error");
    //                }
    //                filterContext.Result = new EmptyResult();
    //                return;
    //            }
    //        }

    //        base.OnActionExecuting(filterContext);
    //    }
    //}
}
