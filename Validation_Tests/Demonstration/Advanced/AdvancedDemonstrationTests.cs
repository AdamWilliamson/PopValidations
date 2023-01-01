using ApprovalTests;
using PopValidations_Tests.Demonstration.Advanced.Validators;
using PopValidations_Tests.TestHelpers;
using System.Threading.Tasks;
using Xunit;

namespace PopValidations_Tests.Demonstration.Advanced;

public class AdvancedDemonstrationTests
{
    [Fact]
    public async Task AdvancedValidator_ValidateWithSingleArtistAlbum_ToJson()
    {
        // Arrange
        var fakeAlbumDetailChecker = new FakeAlbumDetailsChecker();
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new AdvancedAlbumValidator(fakeAlbumDetailChecker));

        // Act
        var results = await runner.Validate(AdvancedTestData.SingleArtist());
        var json = JsonConverter.ToJson(results);

        // Assert
        Approvals.VerifyJson(json);
    }

    [Fact]
    public async Task AdvancedValidator_ValidateWithCollaborationAlbum_ToJson()
    {
        // Arrange
        var fakeAlbumDetailChecker = new FakeAlbumDetailsChecker();
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new AdvancedAlbumValidator(fakeAlbumDetailChecker));

        // Act
        var results = await runner.Validate(AdvancedTestData.Collaboration());
        var json = JsonConverter.ToJson(results);

        // Assert
        Approvals.VerifyJson(json);
    }

    [Fact]
    public void BasicValidator_Describe_ToJson()
    {
        // Arrange
        var fakeAlbumDetailChecker = new FakeAlbumDetailsChecker();
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new AdvancedAlbumValidator(fakeAlbumDetailChecker));

        // Act
        var results = runner.Describe();
        var json = JsonConverter.ToJson(results);

        // Assert
        Approvals.VerifyJson(json);
    }
}

