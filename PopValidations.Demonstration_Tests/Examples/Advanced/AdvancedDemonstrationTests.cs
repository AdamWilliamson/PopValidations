using ApprovalTests;
using Newtonsoft.Json.Linq;
using PopValidations.Swashbuckle_Tests.Helpers;
using PopValidations_Tests.TestHelpers;
using System.Threading.Tasks;
using Xunit;

namespace PopValidations.Demonstration_Tests.Examples.Advanced;

public class AdvancedDemonstrationTests
{
    [Fact]
    public async Task Validation()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new AdvancedDemonstration.AlbumSubmissionValidator());
        var album = new AdvancedDemonstration.Album(
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
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new AdvancedDemonstration.AlbumSubmissionValidator());

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
               AdvancedDemonstration.TestController,
               AdvancedDemonstration.AlbumSubmissionValidator,
               AdvancedDemonstration.AlbumSubmission
           >();

        // Act
        var helper = await setup.GetHelper(config);
        JObject json = JObject.Parse(helper.Content);
        var match = json["components"]["schemas"];//.FirstOrDefault();

        // Assert
        Approvals.VerifyJson(match?.ToString(Newtonsoft.Json.Formatting.None));
    }
}

