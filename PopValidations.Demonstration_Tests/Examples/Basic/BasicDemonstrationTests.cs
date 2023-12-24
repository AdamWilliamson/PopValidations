using ApprovalTests;
using Newtonsoft.Json.Linq;
using PopValidations.Swashbuckle_Tests.Helpers;
using PopValidations_Tests.TestHelpers;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PopValidations.Demonstration_Tests.Examples.Basic;

public class BasicDemonstrationTests
{
    [Fact]
    public async Task Validation()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new BasicDemonstration.BasicSongValidator());
        var song = new BasicDemonstration.BasicSong(
            13,
            "",
            1.4
        );

        // Act
        var results = await runner.Validate(song);

        // Assert
        Approvals.VerifyJson(JsonConverter.ToJson(results));
    }

    [Fact]
    public void Description()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new BasicDemonstration.BasicSongValidator());

        // Act
        var results = runner.Describe();

        // Assert
        Approvals.VerifyJson(JsonConverter.ToJson(results));
    }

    [Fact]
    public async Task OpenApi()
    {
        // Arrange
        var config = new WebApiConfig();
        var setup = new TestSetup<
               BasicDemonstration.TestController,
               BasicDemonstration.BasicSongValidator,
               BasicDemonstration.BasicSong
           >();

        // Act
        var helper = await setup.GetHelper(config);
        JObject json = JObject.Parse(helper.Content!);
        var match = json["components"]!["schemas"]!.FirstOrDefault();

        // Assert
        Approvals.VerifyJson(match?.ToString(Newtonsoft.Json.Formatting.None));
    }
}

