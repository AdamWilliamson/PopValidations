using PopValidations;
using System.Threading.Tasks;

namespace PopValidations_Tests.Demonstration.Advanced.Validators;

public class AdvancedSongValidator : AbstractSubValidator<AdvancedSong>
{
    public AdvancedSongValidator()
    {
        Describe(x => x.TrackNumber)
            .NotNull();

        Describe(x => x.TrackName)
            .IsEqualTo("Definitely Not The Correct Song Name.");

        Describe(x => x.Duration)
            .IsEqualTo(
                -1,
                o => o
                    .WithErrorMessage("Song must have a negative duration.")
                    .WithDescription("Songs must force you to travel slowly backwards in time to listen to.")
            );

        Describe(x => x.Genre)
            //.Vitally().NotEmpty();
            //.HasLengthBetween(20, 400)
            ;

        When(
            "Song Duration is greater than 5 minutes",
            song => Task.FromResult(song.Duration > 5),
            () =>
            {
                Include(new RockSongValidator());
            });
    }

    public class SingleArtistSongValidator : AbstractSubValidator<AdvancedSong>
    {
        public SingleArtistSongValidator()
        {
            Describe(x => x.Artist).IsNull();
        }
    }

    public class CollaborativeSongValidator : AbstractSubValidator<AdvancedSong>
    {
        public CollaborativeSongValidator()
        {
            Describe(x => x.Artist)
                //.NotNull()
                .SetValidator(new AdvancedArtistValidator());
        }
    }

    internal class RockSongValidator : AbstractSubValidator<AdvancedSong>
    {
        public RockSongValidator()
        {
            Describe(x => x.Duration)
                .IsEqualTo(10.1);  // DeleteMe
            //.LessThan(15.0);

            Describe(x => x.Genre);
            //.Contains("Rock");
        }
    }
}