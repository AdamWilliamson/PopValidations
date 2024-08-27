using PopApiValidations.ExampleWebApi.Controllers;
using ApiValidations;
using PopApiValidations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    // Register the Global Filter that enables Validation to execute.
    .AddApiValidationsFilter();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// PopValidations Extensions Function for Registering The Validation Runner
builder.Services.RegisterApiValidationRunner()
    // And this extension and all the Validators in the same assembly as "SongValidator"
    .RegisterAllMainApiValidators(typeof(AddressOwnershipController).Assembly);

builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
