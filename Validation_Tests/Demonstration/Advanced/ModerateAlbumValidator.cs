using System.Linq;
using System.Threading.Tasks;
using PopValidations;

namespace PopValidations_Tests.Demonstration.Advanced;

public class AdvancedAlbumValidator : AbstractValidator<AdvancedAlbum>
{
    public AdvancedAlbumValidator(IFakeAlbumDetailsChecker fakeAlbumDetailsChecker)
    {
        Describe(x => x.Artist)
            .Vitally().IsEqualTo("John");

        DescribeEnumerable(x => x.Songs)
            .Vitally().NotNull()
            .ForEach(song =>
                song
                    .Vitally().NotNull()
                    //.SetValidator(new AdvancedSongValidator())

            );

        When(
            "Album is Collaboration",
            (album) => Task.FromResult(album?.Type == AlbumType.Collaboration), 
            () =>
            {
                //Include(new CollaborationAlbumValidator());
                
                Describe(x => x.CoverImageUrl);
                    //.IsValidUrl(DateTime.MinValue);
                    //.Must(x => x.Contains(
            });

        //ScopeWhen("Album constains songs we don't own the copyright to",
        //    (album) => album.Songs.Where(async song => await fakeAlbumDetailsChecker.DoWeOwnRightsToSong(song)).ToList(),
        //    (scopedData) => true,
        //    (scopedData) => {
        //        Describe(x => x.Artist)
        //            .AddError("");
        //    });
    }
}

public class CollaborationAlbumValidator : AbstractSubValidator<AdvancedAlbum>
{
    public CollaborationAlbumValidator()
    {

    }
}