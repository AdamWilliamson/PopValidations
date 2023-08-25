using PopValidations.Swashbuckle_Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PopValidations.Demonstration_Tests.Examples.Advanced;

public static class AdvancedDemonstration
{
    public record Artist(string? Name);

    public class ArtistValidator : AbstractSubValidator<Artist>
    {
        public ArtistValidator()
        {
            Describe(x => x.Name).IsNotNull();
        }
    }

    public record Song(
        List<Artist>? Artists,
        int? TrackNumber,
        string? TrackName,
        double? Duration,
        string? Genre
    );

    public class SongValidator : AbstractSubValidator<Song>
    {
        public SongValidator()
        {
            Describe(x => x.TrackNumber).Vitally().IsNotNull();

            Describe(x => x.TrackName).Vitally().IsNotEmpty();

            Describe(x => x.Duration)
                .Vitally().IsNotNull()
                .IsGreaterThan(0,
                    o => o
                        .WithErrorMessage("Song does not have a duration.")
                        .WithDescription("Songs must have a positive duration.")
                );

            Describe(x => x.Genre).Vitally().IsNotEmpty();

            DescribeEnumerable(x => x.Artists)
                .Vitally().IsNotNull()
                .ForEach(x => x.Vitally().IsNotNull().SetValidator(new ArtistValidator()));
        }
    }

    public enum AlbumType
    {
        SingleArtist,
        Collaboration,
        Compilation
    }

    public record Album(
        string? Title,
        AlbumType? Type,
        List<Artist>? Artists,
        string? CoverImageUrl,
        DateTime? Created,
        List<Song?>? Songs,
        List<string>? Genres
    );

    public class AlbumValidator : AbstractSubValidator<Album>
    {
        public AlbumValidator()
        {
            Describe(x => x.Title).Vitally().IsNotEmpty();
            
            Describe(x => x.Type).Vitally().IsNotNull();
            
            DescribeEnumerable(x => x.Artists).Vitally().IsNotNull()
                .ForEach(x => x.Vitally().IsNotNull().SetValidator(new ArtistValidator()));

            Describe(x => x.CoverImageUrl).Vitally().IsNotEmpty();

            Describe(x => x.Created)
                .Vitally().IsNotNull()
                .IsGreaterThan(new DateTime(1700, 0, 0, 0, 0, 0, 0))
                .IsLessThan(DateTime.Now);

            DescribeEnumerable(x => x.Songs)
                .ForEach(song => song.SetValidator(new SongValidator()));

            DescribeEnumerable(x => x.Songs)
                .ForEach(song => song.Vitally().IsNotNull().IsLengthInclusivelyBetween(3, 45));

            Scope("get X Data",
                (album) => Task.FromResult<DesignedToBePatternMatched?>(new DesignedToBePatternMatched(album)),
                (data) =>
                {
                    When(
                        "Artist is not null",
                        (album) => Task.FromResult(album.Artist != null),
                        () =>
                        {
                            Describe(x => x.Artist).SetValidator(new AdvancedArtistValidator());
                            Describe(x => x.Artist!.Name)
                                .IsEqualTo(
                                    data.To("'artist' with Something appended", artist => Task.FromResult<string?>(artist + "Something")),
                                    o => o.WithErrorMessage("{{value}} is ensured to be false by adding Something to the end.")
                                );
                        });

                    Describe(x => x.Created)
                        .Is(
                            "Has a SufficientlyLong Name",
                            "Name is too short",
                            c => data.To("", x => Task.FromResult(x is { HasSufficientlyLongName: true }))
                        );
                }
            );

            ScopeWhen("Album constains songs we don't own the copyright to",
                (album) => Task.FromResult(album.Created != null && album.Created?.Year > 1900),
                "Collection of Song to Ownership",
                (album) => fakeAlbumDetailsChecker.DoWeOwnRightsToSong(album.Songs),
                (SongOwnedPair) =>
                {
                    DescribeEnumerable(x => x.Songs)
                        .Vitally().ForEach((song) =>
                        {
                            song.IsNotNull()
                            .Is(
                                "{{value}} Was not found in list of songs we own the rights to",
                                "Checks to ensure the song's rights are owned by us.",
                                s => SongOwnedPair.To(
                                    "Matches Track and Is Owned",
                                    x => Task.FromResult(x.Any(u => u.Item1!.TrackName == s.TrackName && u.Item2)))
                            );
                        });
                });
        }
    }

    public record AlbumSubmission(List<Album>? Albums);

    public class AlbumSubmissionValidator : AbstractValidator<AlbumSubmission>
    {
        public AlbumSubmissionValidator()
        {
            DescribeEnumerable(x => x.Albums).Vitally().IsNotNull()
                .ForEach(x => x.Vitally().IsNotNull().SetValidator(new AlbumValidator()));
        }
    }

    public class TestController : ControllerBase<AlbumSubmission> { }
}