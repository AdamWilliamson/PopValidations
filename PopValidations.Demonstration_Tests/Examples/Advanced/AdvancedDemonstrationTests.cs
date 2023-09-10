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
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new AdvancedDemonstration.AlbumSubmissionValidator(new AdvancedDemonstration.AlbumVerificationService()));
        var albumSubmission = new AdvancedDemonstration.AlbumSubmission(
            new() {
                new AdvancedDemonstration.Album(
                    "Down with the Sickness",
                     AdvancedDemonstration.AlbumType.SingleArtist,
                    new (){ new AdvancedDemonstration.Artist("") },
                    "https://en.wikipedia.org/wiki/File:TheSickness.jpg",
                    new System.DateTime(2000, 03, 07),
                    new()
                    {
                        new AdvancedDemonstration.Song(
                            new(){ new AdvancedDemonstration.Artist("Disturbed") },
                            4,
                            "Down With The Sickness",
                            4.38,
                            "Nu Metal"
                            ),
                        null
                    },
                    new (){ "Rock", "Nu Metal" }
                ),
                //new AdvancedDemonstration.Album(
                //    "",
                //     AdvancedDemonstration.AlbumType.Collaboration,
                //    new() { new AdvancedDemonstration.Artist("Disturbed") },
                //    null,
                //    null,
                //    new()
                //    {
                //        new AdvancedDemonstration.Song(
                //            new() { new AdvancedDemonstration.Artist("Fake-Disturbed") },
                //            null,
                //            "",
                //            -1,
                //            "Pop"
                //            ),
                //        null
                //    },
                //    new() { "Rock", "Nu Metal" }
                //),
                //null
            }
        );

        // Act
        var results = await runner.Validate(albumSubmission);
        var json = JsonConverter.ToJson(results);

        // Assert
        Approvals.VerifyJson(json);
    }

    [Fact]
    public void Description()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new AdvancedDemonstration.AlbumSubmissionValidator(new AdvancedDemonstration.AlbumVerificationService()));

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
           >(new AdvancedDemonstration.AlbumSubmissionValidator(new AdvancedDemonstration.AlbumVerificationService()));

        // Act
        var helper = await setup.GetHelper(config);
        JObject json = JObject.Parse(helper.Content);
        var match = json["components"]["schemas"];//.FirstOrDefault();

        // Assert
        Approvals.VerifyJson(match?.ToString(Newtonsoft.Json.Formatting.None));
    }
}

