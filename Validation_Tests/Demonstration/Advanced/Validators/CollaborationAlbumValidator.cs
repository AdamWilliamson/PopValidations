using PopValidations;

namespace PopValidations_Tests.Demonstration.Advanced.Validators;

public class CollaborationAlbumValidator : AbstractSubValidator<AdvancedAlbum>
{
    public CollaborationAlbumValidator()
    {
        Describe(x => x.CoverImageUrl);

        DescribeEnumerable(x => x.Songs)
            .ForEach(song => { });      // Contain different songs.      
    }
}
