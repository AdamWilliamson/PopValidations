using ApprovalTests;
using System.Threading.Tasks;
using Validations_Tests.TestHelpers;
using Xunit;

namespace Validations_Tests.Demonstration.Basic;

public class BasicDemonstrationTests
{
    [Fact]
    public async Task BasicValidator_Validate_ToJson()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new BasicSongValidator());
        var song = new BasicSong(
            "Disturbed", 
            null, 
            "Down With The Sickness", 
            2.3, 
            string.Empty
        );

        // Act
        var results = await runner.Validate(song);
        var json = JsonConverter.ToJson(results);

        // Assert
        Approvals.VerifyJson(json);
    }

    [Fact]
    public void BasicValidator_Describe_ToJson()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new BasicSongValidator());

        // Act
        var results = runner.Describe();
        var json = JsonConverter.ToJson(results);

        // Assert
        Approvals.VerifyJson(json);
    }
}

