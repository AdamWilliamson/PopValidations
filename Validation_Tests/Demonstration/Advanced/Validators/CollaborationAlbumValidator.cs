using PopValidations;
using static PopValidations_Tests.Demonstration.Advanced.Validators.AdvancedSongValidator;

namespace PopValidations_Tests.Demonstration.Advanced.Validators;

public class CollaborationAlbumValidator : AbstractSubValidator<AdvancedAlbum>
{
    public CollaborationAlbumValidator()
    {
        Describe(x => x.CoverImageUrl);

        DescribeEnumerable(x => x.Songs)
            .ForEach(song => {
                song.SetValidator(new CollaborativeSongValidator());
            });
    }
}
