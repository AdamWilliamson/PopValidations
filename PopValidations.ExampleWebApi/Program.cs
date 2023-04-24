using PopValidations;
using PopValidations.ExampleWebApi.Controllers;
using PopValidations.ExampleWebApi.Handlers;
using PopValidations.MediatR;
using PopValidations.Swashbuckle;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
    options.Filters.Add<PopValidationExceptionFilter>()
);
builder.Services.AddEndpointsApiExplorer();

//== Swagger Setup Services
builder.Services.AddMediatR(
    cfg => cfg
        .RegisterServicesFromAssemblyContaining<HomeController>()
        .AddPopValidations()
);
builder.Services.RegisterRunner().RegisterAllMainValidators(typeof(AlbumValidator).Assembly);
builder.Services.RegisterPopValidationsOpenApiDefaults();
builder.Services.AddSwaggerGen(
    options =>
    {
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
    app.UseSwaggerUI();
}

app.Run();
