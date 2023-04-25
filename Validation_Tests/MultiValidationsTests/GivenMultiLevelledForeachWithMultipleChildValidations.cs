using ApprovalTests;
using FluentAssertions;
using PopValidations;
using PopValidations.Execution.Description;
using PopValidations_Tests.TestHelpers;
using System;
using System.Collections.Generic;
using Xunit;

namespace PopValidations_Tests.MultiValidationsTests
{

    public record Artist(string Name);
    public record Song(string Name, Artist? ArtistAgain);
    public record Album(string Name, Artist? Artist, List<Song>? Songs);

    public class SongSubValidator : AbstractSubValidator<Song>
    {
        public SongSubValidator()
        {
            Describe(x => x.Name)
                .NotNull()
                .IsLengthInclusivelyBetween(5, 200)
                ;
            Describe(x => x.ArtistAgain)
                .NotNull()
                ;
        }
    }

    public class AlbumValidator : AbstractSubValidator<Album>
    {
        public AlbumValidator()
        {
            DescribeEnumerable(x => x.Songs)
                .ForEach(x => x.SetValidator(new SongSubValidator()))
                ;
        }
    }

    public class EditAlbumRequest
    {
        public int Id { get; set; }
        public Album Album { get; set; }
    }

    public class EditAlbumRequestValidator : AbstractValidator<EditAlbumRequest>
    {
        public EditAlbumRequestValidator()
        {
            Describe(x => x.Album)
                .SetValidator(new AlbumValidator())
                ;
        }
    }


    public class GivenMultiLevelledForeachWithMultipleChildValidations
    {
        [Fact]
        public void ThenItDoesntFailWhenDescribing()
        {
            // Arrange
            var runner = ValidationRunnerHelper.BasicRunnerSetup(new EditAlbumRequestValidator());

            // Act
            DescriptionResult? descriptionResult = null;
            Action act = () => {
                descriptionResult = runner.Describe();
            };

            // Assert
            act.Should().NotThrow();
            var json = JsonConverter.ToJson(descriptionResult);

            // Assert
            Approvals.VerifyJson(json);
        }
    }
}
