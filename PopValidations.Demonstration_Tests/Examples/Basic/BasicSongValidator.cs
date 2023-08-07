namespace PopValidations.Demonstration_Tests.Examples.Basic;

public class BasicSongValidator : AbstractValidator<BasicSong>
{
    public BasicSongValidator()
    {
        Describe(x => x.TrackNumber)
            .Vitally().IsNotNull()
            .IsGreaterThan(-1)
            .IsLessThan(200);

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
            .Vitally().IsNotEmpty()
            .IsLengthInclusivelyBetween(20, 400);
    }
}