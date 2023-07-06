using ApprovalTests;
using FluentAssertions;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Linq;
using PopValidations.Swashbuckle_Tests.Models;
using PopValidations_Tests.TestHelpers;
using Xunit;

namespace PopValidations.Swashbuckle_Tests.DisabledTests
{
    public static class Thing
    {
        public static OtherThing Component(string name)
        {
            return new OtherThing(name);
        }
    }

    public class OtherThing
    {
        private OpenApiSchema openApiObject = new();
        private string name;

        public OtherThing(string name)
        {
            this.name = name;
        }

        public OtherThing Set(Action<OpenApiSchema> action)
        {
            action?.Invoke(openApiObject);
            return this;
        }
    }

    public class DisabledTest
    {
        [Fact]
        public async Task Test()
        {
            // Arrange
            var factory = new ApiWebApplicationFactory()
                .WithConfig(new DisabledConfig())
                .AddController<DisabledTestController>()
                .AddValidator<DisabledTestRequestValidator, DisabledTestRequest>();
                
            var client = factory.CreateClient();

            // Act
            var json = await client.GetAsync("/swagger/v1/swagger.json");
            var content = (await json.Content.ReadAsStringAsync());
            var jobj = JObject.Parse(content);
            
            //Thing.Component("").Set(x => x.Required.Add("Id"))

            var runner = ValidationRunnerHelper.BasicRunnerSetup(new BasicSongValidator());

            // Act
            var results = runner.Describe();
            foreach(var description in results.Results)
            {
                description.Property
            }
            //var json = JsonConverter.ToJson(results);


            // Assert
            content.Should().NotBeNull();
            Approvals.VerifyJson(content);
        }
    }
}
