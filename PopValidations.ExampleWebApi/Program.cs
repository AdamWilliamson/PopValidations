using PopValidations;
using PopValidations.ExampleWebApi.Features.AdvancedExample;
using PopValidations.ExampleWebApi.Features.BasicExample;
using PopValidations.MediatR;
using PopValidations.Swashbuckle;
using PopValidations.WebApi;

var builder = WebApplication.CreateBuilder(args);

// When Adding Controllers, You can convert exceptions caused by Validations, into WebApi Validation Errors.
// This Filter provided by PopValidations converts exceptions into 422 Validation Errors for WebApi.
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

//== Swagger Setup Services
builder.Services.AddMediatR(
    cfg => cfg
        .RegisterServicesFromAssemblyContaining<BasicObjectController>()
        // Pop Validation Extension, that adds a MediatR Behaviour to validate all objects before executing the handlers.
        .AddPopValidations()
);

// PopValidations Extensions Function for Registering The Validation Runner
builder.Services.RegisterRunner()
    // And this extension and all the Validators in the same assembly as "SongValidator"
    .RegisterAllMainValidators(typeof(BasicObjectController).Assembly);

// Register a Pop Validation Config that describes the configuration for describing the validations within OpenApi
builder.Services.RegisterPopValidationsOpenApiDefaults(new WebApiConfig());

// Inside the Swagger generator, you now just need to Add the PopValidation Filter, that will modify the OpenApi Schema
builder.Services.AddSwaggerGen(
    options =>
    {
        // Register PopValidation's Custom API decorations
        options.RegisterOpenApiModificationFilter();
    });
//== End Swagger Setup

//== Include Services for Validators and Handlers
builder.Services.AddSingleton<AlbumVerificationService, AlbumVerificationService>();
//== End Include

var app = builder.Build();

app.UseMediatRToHttpErrorMiddleware();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    //== Setup Swagger and UI
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        // Shows the Validation Messages within the UI.
        options.ShowExtensions();
    });
}

app.Run();
