using PopValidations;
using PopValidations.ExampleWebApi.Controllers;
using PopValidations.ExampleWebApi.Handlers;
using PopValidations.MediatR;
using PopValidations.Swashbuckle;

var builder = WebApplication.CreateBuilder(args);

// When Adding Controllers, You can convert exceptions caused by Validations, into WebApi Validation Errors.
builder.Services.AddControllers(
    // This Filter provided by PopValidations converts exceptions into 422 Validation Errors for WebApi.
    options => options.Filters.Add<PopValidationExceptionFilter>()
);
builder.Services.AddEndpointsApiExplorer();

//== Swagger Setup Services
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

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
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
