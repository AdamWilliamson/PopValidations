using System.Linq;
using System.Threading.Tasks;
using PopValidations;
using static PopValidations_Tests.Demonstration.Advanced.Validators.AdvancedSongValidator;

namespace PopValidations_Tests.Demonstration.Advanced.Validators;

public class DesignedToBePatternMatched
{
    private readonly AdvancedAlbum album;

    public DesignedToBePatternMatched(AdvancedAlbum album)
    {
        this.album = album;
    }

    public AlbumType? Type => album.Type;
    public bool HasAlbumType => album.Type == null;
    public bool HasSufficientlyLongName => album.Title == null? false : album.Title.Length > 5;
}

public class AdvancedAlbumValidator : AbstractValidator<AdvancedAlbum>
{
    public AdvancedAlbumValidator(IFakeAlbumDetailsChecker fakeAlbumDetailsChecker)
    {
        Scope("get X Data",
            (album) => Task.FromResult(new DesignedToBePatternMatched(album)),
            (data) =>
            {
                When(
                    "Artist is not null",
                    (album) => Task.FromResult(album.Artist != null),
                    () =>
                    {
                        Describe(x => x.Artist).SetValidator(new AdvancedArtistValidator());
                        Describe(x => x.Artist.Name)
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

        DescribeEnumerable(x => x.Songs)
            //.Vitally().NotEmpty()
            .ForEach(song =>
                song
                    .Vitally().NotNull()
                    .SetValidator(new AdvancedSongValidator())
            );

        When("Album is Single Artist",
            (album) => Task.FromResult(album?.Type == AlbumType.SingleArtist),
            () =>
            {
                Include(new SingleArtistAlbumValidator());
                Describe(x => x.Artist).NotNull();
            });

        ScopeWhen("Album constains songs we don't own the copyright to",
            (album) => Task.FromResult(album.Created != null && album.Created?.Year > 1900),
            "Collection of Song to Ownership",
            (album) => fakeAlbumDetailsChecker.DoWeOwnRightsToSong(album.Songs),
            (SongOwnedPair) =>
            {
                DescribeEnumerable(x => x.Songs)
                    .Vitally().ForEach((song) =>
                    {
                        song.NotNull()
                        .Is(
                            "{{value}} Was not found in list of songs we own the rights to",
                            "Checks to ensure the song's rights are owned by us.",
                            s => SongOwnedPair.To(
                                "Matches Track and Is Owned",
                                x => Task.FromResult(x.Any(u => u.Item1.TrackName == s.TrackName && u.Item2)))
                        );
                    });
            });

        When("Album is Collaboration",
            (album) => Task.FromResult(album?.Type == AlbumType.Collaboration),
            () => {
                Include(new CollaborationAlbumValidator());
                //Describe(x => x.Artist).IsNull();
            });

        When(
            "Album is SingleArtist",
            album => Task.FromResult(album.Type == AlbumType.SingleArtist),
            () =>
            {
                DescribeEnumerable(x => x.Songs)
                   .ForEach((song) => song.SetValidator(new SingleArtistSongValidator()));
            });

        When(
            "Album is Collaborative",
            album => Task.FromResult(album.Type == AlbumType.Collaboration),
            () =>
            {
                DescribeEnumerable(x => x.Songs)
                   .ForEach((song) => song.SetValidator(new CollaborativeSongValidator()));
            });
    }
}