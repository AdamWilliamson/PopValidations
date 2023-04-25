using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Web.Http;
using IActionFilter = Microsoft.AspNetCore.Mvc.Filters.IActionFilter;

namespace PopValidations.MediatR;

public class PopValidationHttpException : HttpResponseException
{
    public PopValidationHttpException(Dictionary<string, List<string>> errors)
        :base(HttpStatusCode.UnprocessableEntity)
    {
        Errors = errors;
    }

    public Dictionary<string, List<string>> Errors { get; }
}

public class PopValidationExceptionFilter : IActionFilter, IOrderedFilter
{
    public int Order => int.MaxValue - 10;

    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is PopValidationHttpException exception)
        {
            context.Result = new ObjectResult(exception.Errors)
            {
                StatusCode = (int)HttpStatusCode.UnprocessableEntity
            };

            context.ExceptionHandled = true;
        }
    }
}