using ApiValidations.Execution;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Http;
using System.Reflection;

namespace PopApiValidations;

public class ApiValidationsAttribute : ActionFilterAttribute
{
    public override async void OnActionExecuting(ActionExecutingContext filterContext)
    {
        var methodInfo = ((ControllerActionDescriptor)filterContext.ActionDescriptor)?.MethodInfo;
        if (!filterContext.ActionArguments.Any() || methodInfo == null || methodInfo.GetCustomAttribute(typeof(ApiValidationsIgnoreAttribute)) != null)
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

        var result = method.Invoke(
            runner,
            [filterContext.Controller, new HeirarchyMethodInfo(string.Empty, methodInfo, orderedParams)]
        ) as Task<ApiValidationResult>;

        if (result != null)
        {
            var validations = await result;
            if (validations.Errors.Any())
            {
                foreach (var validation in validations.Errors)
                {
                    filterContext.ModelState.AddModelError(
                        ProcessParam(validation.Key),
                        validation.Value.FirstOrDefault() ?? "Error"
                    );
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

    private string ProcessParam(string errorKey)
    {
        return ApiValidations.Execution.PopApiValidations.Configuation.GetParamNameFromErrorKey?.Invoke(errorKey) ?? errorKey;
    }
}