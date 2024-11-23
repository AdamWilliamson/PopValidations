using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using PopApiValidations.ExampleWebApi.Controllers;
using PopApiValidations.ExampleWebApi_Tests.Helpers;
using PopApiValidations.Swashbuckle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ApiValidations;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Collections.ObjectModel;
using System.Collections;

namespace PopApiValidations.ExampleWebApi_Tests;

//public class SwaggerWebFactory<Type> : WebApplicationFactory<Type>
//    where Type : class
//{
//    protected override void ConfigureWebHost(IWebHostBuilder builder)
//    {
//        builder.ConfigureServices(services =>
//        {

//            builder.Services.AddControllers()
//                // Register the Global Filter that enables Validation to execute.
//                .AddApiValidationsFilter();
//            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//            builder.Services.AddEndpointsApiExplorer();

//            // Register a Pop Validation Config that describes the configuration for describing the validations within OpenApi
//            builder.Services.RegisterPopApiValidationsOpenApiDefaults(new PopApiOpenApiConfig());

//            builder.Services.AddSwaggerGen(
//                options =>
//                {
//                    // Register PopValidation's Custom API decorations
//                    options.RegisterApiValidationOpenApiFilter();
//                });
//            // Optional: Replace merging of objects for end points.
//            builder.Services.RegisterApiValidationPerEndpointDefinitionsFilter();

//            app.UseSwagger();
//            app.UseSwaggerUI();
//            app.MapControllers();
//        });
//    }
//}

public class TestStartup<TControllerType>
{
    public TestStartup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        var part = new AssemblyPart(typeof(TControllerType).Assembly);

        services//.AddMvc()
            .AddControllersWithViews()
            .ConfigureApplicationPartManager(apm => apm.ApplicationParts.Add(part))
            //.AddApplicationPart(typeof(TControllerType).Assembly)
            //.AddControllersAsServices()
            //.AddControllers()
            // Register the Global Filter that enables Validation to execute.
            .AddApiValidationsFilter();
        
        //AssemblyPart _part = new AssemblyPart(typeof(TControllerType).Assembly);
        //_partManager.ApplicationParts.Add(_part);

//        services.AddTransient(typeof(TControllerType), typeof(TControllerType));

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        //services.AddEndpointsApiExplorer();

        // PopValidations Extensions Function for Registering The Validation Runner
        services.RegisterApiValidationRunner()
            // And this extension and all the Validators in the same assembly as "SongValidator"
            .RegisterAllMainApiValidators(typeof(AddressOwnershipController).Assembly);

        // Register a Pop Validation Config that describes the configuration for describing the validations within OpenApi
        services.RegisterPopApiValidationsOpenApiDefaults(new PopApiOpenApiConfig());

        services.AddSwaggerGen(
            options =>
            {
                // Register PopValidation's Custom API decorations
                options.RegisterApiValidationOpenApiFilter();
            });

        // Optional: Replace merging of objects for end points.
        services.RegisterApiValidationPerEndpointDefinitionsFilter();

    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger();
        
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseEndpoints(c => c.MapControllers());
        //app.UseEndpoints(endpoints =>
        //{
        //    endpoints.MapControllerRoute(
        //        name: "default",
        //        pattern: "{controller=Home}/{action=Index}/{id?}");
        //});
    }
}


public class CombinatorialTests
{
    public static IEnumerable<object[]> GetData()
    {
        List<Type> NullableTypes = new List<Type>
        {
            typeof(int),
            typeof(DateTime),
            typeof(decimal),
            typeof(long),
            typeof(double),
            typeof(bool),
            typeof(float),
            typeof(Guid),
            typeof(byte),
            typeof(short),
            typeof(char),
            typeof(sbyte),
            typeof(ushort),
            typeof(uint),
            typeof(ulong),
            typeof(TimeSpan),
            typeof(DateTimeOffset),
            typeof(Enum),
            typeof(IQueryable),
            typeof(IOrderedQueryable),
            typeof(IEnumerable),
            typeof(ICollection),
            typeof(IList),
            typeof(IDictionary),
            typeof(CancellationToken),
            typeof(ValueTask)
        };

        List<Type> NonNullableTypes = new List<Type>
        {
            typeof(string),
            typeof(object),
            typeof(Task),
            typeof(Uri),
            typeof(Guid),
            typeof(Exception),
            typeof(CancellationToken),
            typeof(Delegate),
            typeof(TaskCompletionSource),
            typeof(Delegate),
            typeof(Uri),
            typeof(Exception)
        };

        List<Type> WrapperTypes = new List<Type>
        {
            typeof(Nullable<>),
            typeof(Task<>),
            typeof(ActionResult<>),
            typeof(IEnumerable<>),
            typeof(ICollection<>),
            typeof(IList<>),
            typeof(IReadOnlyList<>),
            typeof(IReadOnlyCollection<>),
            typeof(IDictionary<,>),
            typeof(IReadOnlyDictionary<,>),
            typeof(Queue<>),
            typeof(Stack<>),
            typeof(SortedList<,>),
            typeof(SortedDictionary<,>),
            typeof(LinkedList<>),
            typeof(WeakReference<>),
            typeof(Lazy<>),
            typeof(IAsyncEnumerable<>),
            typeof(ValueTask<>),
            typeof(IObservable<>),
            typeof(TaskCompletionSource<>),
        };

        List<Type> WrapperReturnTypes = new List<Type>
        {
            typeof(Task<>),
            typeof(ActionResult<>)
        };

        List<Type> ReturnTypes = new List<Type>
        {
            typeof(void),
            typeof(Task),
            typeof(ActionResult),
            typeof(IActionResult)
        };


        List<Type> resultantTypes = new();
        foreach(var type in NullableTypes) { resultantTypes.Add(type); }
        foreach(var type in NonNullableTypes) { resultantTypes.Add(type); }
        foreach(var type in WrapperTypes) { foreach (var subType in NullableTypes) { resultantTypes.Add(type.MakeGenericType(subType)); } }
        foreach (var returnType in ReturnTypes) 
        {
            foreach (var type in WrapperTypes) 
            { 
                foreach (var subType in NullableTypes) 
                { 
                    resultantTypes.Add(returnType.MakeGenericType(type.MakeGenericType(subType))); 
                } 
            }
        }

        return resultantTypes.Select(x => new[] {x});
    }

    public async Task<string> GetSUT()
    {
        var newtype = ControllerGenerator.GenerateController();
        var type = typeof(TestStartup<>).MakeGenericType(newtype);

        var webHostBuilder = new WebHostBuilder()
            
                    .UseEnvironment("Test") // You can set the environment you want (development, staging, production)
                    .UseStartup(type); // Startup class of your web app project

        using (var server = new TestServer(webHostBuilder))
        {
            using (var client = server.CreateClient())
            {
                string result = await client.GetStringAsync("/swagger/v1/swagger.json");
                //JObject root = JObject.Parse(result);
                //return root;
                return result;
            }
        }
    }

    [Fact]
    public async Task Test()
    {
        var sut = await GetSUT();

    }
}
