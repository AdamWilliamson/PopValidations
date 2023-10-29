using PopValidations;

namespace PopValidations.ExampleWebApi.Features.AdvancedExample;

public record Artist(string? Name);

public record Song(
    List<Artist>? Artists,
    int? TrackNumber,
    string? TrackName,
    double? Duration,
    string? Genre
);

public enum AlbumType
{
    Single,
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

public class ArtistValidator : AbstractSubValidator<Artist>
{
    public ArtistValidator()
    {
        Describe(x => x.Name).IsNotNull();
    }
}

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

public class AlbumValidator : AbstractSubValidator<Album>
{
    public AlbumValidator(AlbumVerificationService albumVerificationService)
    {
        Describe(x => x.Title).Vitally().IsNotEmpty();

        Describe(x => x.Type).Vitally().IsNotNull();

        DescribeEnumerable(x => x.Artists).Vitally().IsNotNull()
            .ForEach(x => x.Vitally().IsNotNull().SetValidator(new ArtistValidator()));

        Describe(x => x.CoverImageUrl).Vitally().IsNotEmpty();

        Describe(x => x.Created)
            .Vitally().IsNotNull()
            .IsGreaterThan(new DateTime(1700, 01, 01, 13, 0, 0, 0))
            .IsLessThan(new DateTime(2024, 01, 01, 13, 0, 0, 0));

        DescribeEnumerable(x => x.Songs)
            .Vitally().IsNotNull()
            .IsLengthInclusivelyBetween(3, 45)
            .ForEach(song => song.Vitally().IsNotNull().SetValidator(new SongValidator()));

        Scope("Validate Album",
            async (album) => await albumVerificationService.GetAlbumChecker(album),
            (albumChecker) =>
            {
                When(
                    "Album is Compliation",
                    (album) => Task.FromResult(album?.Type == AlbumType.Compilation),
                    () =>
                    {
                        Describe(x => x.Artists)
                            .Is(
                                "Validated to must {{is_value}}",
                                "Failed to {{is_value}}",
                                albumChecker.To(
                                    "have different artists",
                                    (List<Artist>? x, AlbumChecker? i)
                                        =>
                                            x?.Any() == true
                                            && i is { IsAllTheSameArtist: false }
                                )
                            );
                    });

                When(
                    "Album is Single Artist",
                    (album) => Task.FromResult(album?.Type == AlbumType.SingleArtist),
                    () =>
                    {
                        Describe(x => x.Artists)
                            .Is(
                                "Must {{is_value}}",
                                "Does not {{is_value}}",
                                albumChecker.To(
                                    "have all the same artists",
                                    (List<Artist>? x, AlbumChecker? i)
                                        =>
                                            x?.Any() == true
                                            && i is not { IsAllTheSameArtist: true }
                                )
                            );
                    });
            });

        ScopeWhen(
            "When Album is Collaboration",
            (album) => Task.FromResult(album.Type == AlbumType.Collaboration),
            "Get Complex Album Validator",
            (album) => albumVerificationService.GetAlbumChecker(album),
            (albumChecker) =>
            {
                DescribeEnumerable(x => x.Songs)
                    .Is(
                        "All songs must contain atleast one album artist.",
                        "The songs in this album, being collaboration, must contain atleast 1 album artist.",
                        albumChecker.To("", (IEnumerable<Song?>? x, AlbumChecker? i) => i?.AllSongsContainAlbumArtist is true)
                    );
            });

        ScopeWhen(
            "Need the Database Checker to When",
            async (album) => await albumVerificationService.GetAlbumChecker(album),
            "When Album is Single",
            (album, albumChecker) => album?.Type == AlbumType.Single,
            (albumChecker) =>
            {
                Describe(x => x.Songs)
                    .Is(
                        "Album must match the rules for single.",
                        "Must Abide by the rules for singles.",
                        albumChecker.To(
                            "Album is Single",
                            (List<Song?>? x, AlbumChecker? i) => i?.IsSingle is true)
                    );
            });
    }
}

public class AlbumVerificationService
{
    public Task<AlbumChecker> GetAlbumChecker(Album album)
    {
        return Task.FromResult(new AlbumChecker(album));
    }
}

public class AlbumChecker
{
    private readonly Album album;

    public AlbumChecker(Album album)
    {
        this.album = album;
    }

    public bool IsAllTheSameArtist
    {
        get => album.Artists?.All(x => x != null && x?.Name == (album.Artists?.FirstOrDefault()?.Name ?? "")) ?? true;
    }

    public bool AllSongsContainAlbumArtist
    {
        get => album.Artists
            ?.Any(a =>
                album?.Songs
                    ?.All(song =>
                        song?.Artists
                            ?.Any(artist => artist.Name == a?.Name)
                            ?? false
                    ) ?? false
            ) ?? false;
    }

    public bool IsSingle => album.Type == AlbumType.Single && album?.Songs?.Count <= 7;
}
