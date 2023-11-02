<template>
  <v-container fluid bg-color="surface">
    <v-row>
      <v-col>
        <v-card>
          <v-card-title><h3>MediatR</h3></v-card-title>
          <v-card-text>MediatR is a great quick way to connect peices of code and execute them. One of it's benefits, is it allows you to run modifier code before the
          main code.  These things are called Behaviours, and PopValidations allows you to take advantage of that. <br /> 
          With the extensions provided, a behaviour is added that will run the validations against a MediatR's inputs, and cancel execution if any of the validations fail.<br />
          This way you can incorporate Validation restrictions to execution in your code, with little effort.
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <v-row>
      <v-col>
        <v-card>
          <v-card-text>
            <a href="https://www.nuget.org/packages/PopValidations.MediatR">https://www.nuget.org/packages/PopValidations.MediatR</a><br />
            Or via CLI<br />
            dotnet add package PopValidations.MediatR --version 0.9.0
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <v-row>
      <v-col>
        <CodeWindow
          language="csharp"
          source='using PopValidations;
using PopValidations.MediatR;
          
builder.Services.AddMediatR(
    cfg => cfg
        .RegisterServicesFromAssemblyContaining<HomeController>()
        // Pop Validation Extension, that adds a MediatR Behaviour to validate all objects before executing the handlers.
        .AddPopValidations()
);

// PopValidations Extensions Function for Registering The Validation Runner
builder.Services.RegisterRunner()
    // And this extension and all the Validators in the same assembly as "AlbumValidator"
    .RegisterAllMainValidators(typeof(AlbumValidator).Assembly);

// If you want to auto-convert MediatR PopValidation errors into http status code errors,
// you can include the code below, and use it my building the app
// and call on the WebApplication in the Program.cs file
// app.UseMediatRToHttpErrorMiddleware();

'
        ></CodeWindow>
      </v-col>
    </v-row>

    <v-row>
      <v-col>
      <h3>MediatR results to WebApi error response helper</h3>
      </v-col>
    </v-row>

    <v-row>
      <v-col>
        <CodeWindow
          language="csharp"
          source='namespace PopValidations.WebApi;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PopValidations.MediatR;
using System.Net;
using static System.Net.Mime.MediaTypeNames;

public static class PopValidationsWebApiExtenstions
{
    public static void UseMediatRToHttpErrorMiddleware(this WebApplication? app)
    {
        app.UseExceptionHandler(exceptionHandlerApp =>
        {
            exceptionHandlerApp.Run(async context =>
            {
                var exceptionHandlerPathFeature =
                    context.Features.Get<IExceptionHandlerPathFeature>();

                if (exceptionHandlerPathFeature?.Error is PopValidationHttpException exception)
                {
                    context.Response.ContentType = Text.Plain;
                    context.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;

                    await context.Response.WriteAsync(JsonConvert.SerializeObject(exception.Errors));
                }
            });
        });
    }
}


'
        ></CodeWindow>
      </v-col>
    </v-row>
  </v-container>
</template>