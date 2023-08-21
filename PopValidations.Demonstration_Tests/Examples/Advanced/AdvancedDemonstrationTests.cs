//using ApprovalTests;
//using PopValidations_Tests.Demonstration.Advanced.Validators;
//using PopValidations_Tests.TestHelpers;
//using System.Threading.Tasks;
//using Xunit;

//namespace PopValidations_Tests.Demonstration.Advanced;

//public class AdvancedDemonstrationTests
//{
//    [Fact]
//    public async Task AdvancedValidator_ValidateWithValidSingleArtistAlbum_ToJson()
//    {
//        // Arrange
//        var fakeAlbumDetailChecker = new FakeAlbumDetailsChecker();
//        var runner = ValidationRunnerHelper.BasicRunnerSetup(new AdvancedAlbumValidator(fakeAlbumDetailChecker));

//        // Act
//        var results = await runner.Validate(AdvancedTestData.ValidSingleArtist());
//        var json = JsonConverter.ToJson(results);

//        // Assert
//        Approvals.VerifyJson(json);
//    }

//    [Fact]
//    public async Task AdvancedValidator_ValidateWithInValidSingleArtistAlbum_ToJson()
//    {
//        // Arrange
//        var fakeAlbumDetailChecker = new FakeAlbumDetailsChecker();
//        var runner = ValidationRunnerHelper.BasicRunnerSetup(new AdvancedAlbumValidator(fakeAlbumDetailChecker));

//        // Act
//        var results = await runner.Validate(AdvancedTestData.InValidSingleArtist());
//        var json = JsonConverter.ToJson(results);

//        // Assert
//        Approvals.VerifyJson(json);
//    }


//    [Fact]
//    public async Task AdvancedValidator_ValidateWithValidCollaborationAlbum_ToJson()
//    {
//        // Arrange
//        var fakeAlbumDetailChecker = new FakeAlbumDetailsChecker();
//        var runner = ValidationRunnerHelper.BasicRunnerSetup(new AdvancedAlbumValidator(fakeAlbumDetailChecker));

//        // Act
//        var results = await runner.Validate(AdvancedTestData.ValidCollaboration());
//        var json = JsonConverter.ToJson(results);

//        // Assert
//        Approvals.VerifyJson(json);
//    }

//    [Fact]
//    public async Task AdvancedValidator_ValidateWithInValidCollaborationAlbum_ToJson()
//    {
//        // Arrange
//        var fakeAlbumDetailChecker = new FakeAlbumDetailsChecker();
//        var runner = ValidationRunnerHelper.BasicRunnerSetup(new AdvancedAlbumValidator(fakeAlbumDetailChecker));

//        // Act
//        var results = await runner.Validate(AdvancedTestData.InValidCollaboration());
//        var json = JsonConverter.ToJson(results);

//        // Assert
//        Approvals.VerifyJson(json);
//    }

//    [Fact]
//    public void BasicValidator_Describe_ToJson()
//    {
//        // Arrange
//        var fakeAlbumDetailChecker = new FakeAlbumDetailsChecker();
//        var runner = ValidationRunnerHelper.BasicRunnerSetup(new AdvancedAlbumValidator(fakeAlbumDetailChecker));

//        // Act
//        var results = runner.Describe();
//        var json = JsonConverter.ToJson(results);

//        // Assert
//        Approvals.VerifyJson(json);
//    }
//}

