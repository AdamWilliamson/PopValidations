using System.Threading.Tasks;
using PopValidations;

namespace PopValidations.Demonstration_Tests.Examples.Moderate;

public class ModerateAlbumValidator : AbstractValidator<ModerateAlbum>
{
    public ModerateAlbumValidator()
    {
        Describe(x => x.Artist)
            .Vitally().IsEqualTo("John")
            ;

        When(
            "Artist is NOT 'Disturbed'",
            (album) => Task.FromResult(album?.Artist != "Disturbed"),
            () =>
            {
                Describe(x => x.CoverImageUrl);
                //.IsValidUrl(DateTime.MinValue);
            });

        DescribeEnumerable(x => x.Songs)
            .Vitally().IsNotNull()
            //.LengthExlusiveBetween(5, 40)
            .ForEach(song =>
                song
                    .Vitally().IsNotNull()
            );
    }
}