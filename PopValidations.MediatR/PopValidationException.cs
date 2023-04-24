using System.Web.Http.Filters;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PopValidations.MediatR;
using System.Net;
using System.Web.Http;
using IActionFilter = Microsoft.AspNetCore.Mvc.Filters.IActionFilter;

namespace PopValidations.MediatR;

public class PopValidationException : HttpResponseException
{
    public PopValidationException(Dictionary<string, List<string>> errors)
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
        if (context.Exception is PopValidationException httpResponseException)
        {
            context.Result = new ObjectResult(httpResponseException)
            {
                StatusCode = (int)HttpStatusCode.UnprocessableEntity
            };

            context.ExceptionHandled = true;
        }
    }
}