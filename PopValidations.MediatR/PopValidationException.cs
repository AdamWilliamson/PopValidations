//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;
using System.Web.Http.ExceptionHandling;
using System.Net;
using System.Web.Http;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;
//using IActionFilter = Microsoft.AspNetCore.Mvc.Filters.IActionFilter;

namespace PopValidations.MediatR;

public class PopValidationHttpException : HttpResponseException
{
    public PopValidationHttpException(Dictionary<string, List<string>> errors)
        : base(HttpStatusCode.UnprocessableEntity)
    {
        Errors = errors;
    }

    public Dictionary<string, List<string>> Errors { get; }
}

//public class PopValidationExceptionFilter : IActionFilter, IOrderedFilter
//{
//    public int Order => int.MaxValue - 10;

//    public void OnActionExecuting(ActionExecutingContext context) { }

//    public void OnActionExecuted(ActionExecutedContext context)
//    {
//        if (context.Exception is PopValidationHttpException exception)
//        {
//            context.Result = new ObjectResult(exception.Errors)
//            {
//                StatusCode = (int)HttpStatusCode.UnprocessableEntity
//            };

//            context.ExceptionHandled = true;
//        }
//    }
//}


//app.Run(async (context, next) => {
//    try
//    {
//        await next(context);
//    }
//    catch (PopValidationHttpException e)
//    {
//        context.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
//    }
//});




//public class PopValidationEceptionHandlingMiddleware : IExceptionHandler
//{
//    public Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
//    {
//        if (context.Exception is PopValidationHttpException exception)
//        {
//            context.Result = new PopValidationEceptionHandlingResult(exception, context.Request);
//            //new HttpResponseMessage(HttpStatusCode.UnprocessableEntity)
//            //{
//            //    Content = new StringContent(JsonConvert.SerializeObject(exception.Errors), Encoding.Unicode)
//            //};
//            //context.Result = new ObjectResult(exception.Errors)
//            //{
//            //    StatusCode = (int)HttpStatusCode.UnprocessableEntity
//            //};
//            //context.Exce
//            //context.ExceptionHandled = true;
//        }

//        return Task.CompletedTask;
//    }
//}

//public class PopValidationEceptionHandlingResult : IHttpActionResult
//{
//    PopValidationHttpException exception;
//    HttpRequestMessage _request;

//    public PopValidationEceptionHandlingResult(PopValidationHttpException exception, HttpRequestMessage request)
//    {
//        this.exception = exception;
//        _request = request;
//    }

//    public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
//    {
//        var response = new HttpResponseMessage()
//        {
//            Content = new StringContent(JsonConvert.SerializeObject(exception.Errors, Formatting.Indented)),
//            RequestMessage = _request
//        };
//        return Task.FromResult(response);
//    }
//}

//public static class PopValidationEceptionHandlingExtensions
//{
//    public static IServiceCollection RegisterPopValidationsEceptionHandling(this WebApplicationBuilder Services)
//    {
//        Services.AddExceptionHandler<PopValidationEceptionHandlingMiddleware>((o, t) => { });
//        // services.AddExceptionHandler<PopValidationEceptionHandlingMiddleware>((o, t) => { });
//        return Services;
//    }
//}