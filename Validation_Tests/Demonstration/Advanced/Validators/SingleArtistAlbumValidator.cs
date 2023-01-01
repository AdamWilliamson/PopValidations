using PopValidations;

namespace PopValidations_Tests.Demonstration.Advanced.Validators;

public class SingleArtistAlbumValidator : AbstractSubValidator<AdvancedAlbum>
{
    public SingleArtistAlbumValidator()
    {
        Describe(x => x.CoverImageUrl);
        //.Contains(c => c.Artist);

        DescribeEnumerable(x => x.Songs)
            .ForEach(song => { }); // Must all be the same
    }
}