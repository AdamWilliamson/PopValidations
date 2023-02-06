using PopValidations;
using PopValidations.ExampleWebApi.Controllers;
using PopValidations.Swashbuckle;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

//== Swagger Setup Services
builder.Services.AddEndpointsApiExplorer();
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

if (app.Environment.IsDevelopment())
{
    //== Setup Swagger and UI
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
