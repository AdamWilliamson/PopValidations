using ApprovalTests;
using PopValidations_Tests.TestHelpers;
using System.Threading.Tasks;
using Xunit;

namespace PopValidations.Demonstration_Tests.Examples.Basic;

public class BasicDemonstrationTests
{
    [Fact]
    public async Task Validation()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new BasicSongValidator());
        var song = new BasicSong(
            "Disturbed",
            null,
            "Down With The Sickness",
            2.4,
            string.Empty
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
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new BasicSongValidator());

        // Act
        var results = runner.Describe();

        // Assert
        Approvals.VerifyJson(JsonConverter.ToJson(results));
    }
}

