using System.Linq;
using System.Threading.Tasks;
using PopValidations;
using PopValidations.FieldDescriptors.Base;

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
                Describe(x => x.Artist)
                    .IsEqualTo(
                        data.To("'artist' with Something appended", artist => Task.FromResult<string?>(artist + "Something")),
                        o => o.WithErrorMessage("{{value}} is ensured to be false by adding Something to the end.")
                    );

                Describe(x => x.Created)
                    .Must(
                        "Has a SufficientlyLong Name",
                        "Name is too short",
                        c => data.To("", x => Task.FromResult(x is { HasSufficientlyLongName: true }))
                    );
            }
        );

        Describe(x => x.Artist);
        //.Vitally().NotEmpty();

        DescribeEnumerable(x => x.Songs)
            //.Vitally().NotEmpty()
            .ForEach(song =>
                song
                    .Vitally().NotNull()
                    .SetValidator(new AdvancedSongValidator())
            );

        When("Album is Collaboration",
            (album) => Task.FromResult(album?.Type == AlbumType.Collaboration),
            () => Include(new CollaborationAlbumValidator()));

        When("Album is Single Artist",
            (album) => Task.FromResult(album?.Type == AlbumType.SingleArtist),
            () => Include(new SingleArtistAlbumValidator()));

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
                        .Must(
                            "{{value}} Was not found in list of songs we own the rights to",
                            "Checks to ensure the song's rights are owned by us.",
                            s => SongOwnedPair.To(
                                "Matches Track and Is Owned",
                                x => Task.FromResult(x.Any(u => u.Item1.TrackName == s.TrackName && u.Item2)))
                        );
                    });
            });
    }
}