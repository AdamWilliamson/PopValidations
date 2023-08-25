using ApprovalTests;
using System.Threading.Tasks;
using PopValidations_Tests.TestHelpers;
using Xunit;
using PopValidations.Swashbuckle_Tests.Helpers;
using Newtonsoft.Json.Linq;

namespace PopValidations.Demonstration_Tests.Examples.Moderate;

public class ModerateDemonstrationTests
{
    [Fact]
    public async Task Validation()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new ModerateDemonstration.AlbumValidator());
        var album = new ModerateDemonstration.ModerateAlbum(
            "Disturbed",
            "Rock",
            new()
            {
                new ModerateDemonstration.ModerateSong("Down With The Sickness", -1, "", 17, "Pop"),
                null
            }
        );

        // Act
        var results = await runner.Validate(album);
        var json = JsonConverter.ToJson(results);

        // Assert
        Approvals.VerifyJson(json);
    }

    [Fact]
    public void Description()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new ModerateDemonstration.AlbumValidator());

        // Act
        var results = runner.Describe();
        var json = JsonConverter.ToJson(results);

        // Assert
        Approvals.VerifyJson(json);
    }

    [Fact]
    public async Task OpenApi()
    {
        // Arrange
        var config = new WebApiConfig();
        var setup = new TestSetup<
               ModerateDemonstration.TestController,
               ModerateDemonstration.AlbumValidator,
               ModerateDemonstration.ModerateAlbum
           >();

        // Act
        var helper = await setup.GetHelper(config);
        JObject json = JObject.Parse(helper.Content);
        var match = json["components"]["schemas"];//.FirstOrDefault();

        // Assert
        Approvals.VerifyJson(match?.ToString(Newtonsoft.Json.Formatting.None));
    }
}
