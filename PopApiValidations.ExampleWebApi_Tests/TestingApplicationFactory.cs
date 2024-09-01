using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace PopApiValidations.ExampleWebApi_Tests;

public class TestingApplicationFactory : WebApplicationFactory<PopApiValidations.ExampleWebApi.Address>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            
        });
    }
}
