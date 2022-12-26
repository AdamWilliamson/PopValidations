using Validations;

namespace Validations_Tests.Demonstration.Basic;

public class BasicSongValidator : AbstractValidator<BasicSong>
{
    public BasicSongValidator()
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
    }
}