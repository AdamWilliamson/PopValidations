using ApprovalTests;
using System;
using System.Threading.Tasks;
using PopValidations_Tests.TestHelpers;
using Xunit;

namespace PopValidations_Tests.Demonstration.Moderate;

public class ModerateDemonstrationTests
{
    [Fact]
    public async Task BasicValidator_Validate_ToJson()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new ModerateAlbumValidator());
        var album = new ModerateAlbum(
            "Disturbed",
            "https://www.disturbed1.com/sites/g/files/g2000015236/files/inline-images/Divisive_Cover-FINAL_1.jpg",
            DateTime.MinValue,
            new()
            {
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
    public void AlbumValidator_Describe_ToJson()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new ModerateAlbumValidator());

        // Act
        var results = runner.Describe();
        var json = JsonConverter.ToJson(results);

        // Assert
        Approvals.VerifyJson(json);
    }
}
