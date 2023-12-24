namespace PopValidations.WebApi;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PopValidations.MediatR;
using System.Net;
using static System.Net.Mime.MediaTypeNames;

public static class PopValidationsWebApiExtenstions
{
    public static void UseMediatRToHttpErrorMiddleware(this WebApplication app)
    {
        app.UseExceptionHandler(exceptionHandlerApp =>
        {
            exceptionHandlerApp.Run(async context =>
            {
                var exceptionHandlerPathFeature =
                    context.Features.Get<IExceptionHandlerPathFeature>();

                if (exceptionHandlerPathFeature?.Error is PopValidationMediatRException exception)
                {
                    context.Response.ContentType = Text.Plain;
                    context.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;

                    await context.Response.WriteAsync(JsonConvert.SerializeObject(exception.Errors));
                }
            });
        });
    }
}

