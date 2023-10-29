//using ApprovalTests;
//using FluentAssertions;
//using PopValidations.Swashbuckle_Tests.Models;
//using PopValidations_Tests.TestHelpers;
//using Xunit;

//namespace PopValidations.Swashbuckle_Tests.DisabledTests
//{

//    public class DisabledTest
//    {
//        [Fact]
//        public async Task Test()
//        {
//            // Arrange
//            var config = new DisabledConfig();
//            var factory = new ApiWebApplicationFactory()
//                .WithConfig(config)
//                .AddController<DisabledTestController>()
//                .AddValidator<DisabledTestRequestValidator, DisabledTestRequest>();
                
//            var client = factory.CreateClient();

//            // Act
//            var json = await client.GetAsync("/swagger/v1/swagger.json");
//            var content = (await json.Content.ReadAsStringAsync());
//            //var jobj = JObject.Parse(content);
            
//            var helper = new OpenApiHelper(config, content);

//            //Thing.Component("").Set(x => x.Required.Add("Id"))
//            var runner = ValidationRunnerHelper.BasicRunnerSetup(new BasicSongValidator());

//            // Act
//            var results = runner.Describe();

//            var idArray = helper.GetValidationAttribute("DisabledTestRequest");
            
//            //var json = JsonConverter.ToJson(results);



//            // Assert
//            content.Should().NotBeNull();
//            Approvals.VerifyJson(content);
//        }
//    }
//}
