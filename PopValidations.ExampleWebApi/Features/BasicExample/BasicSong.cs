namespace PopValidations.ExampleWebApi.Features.BasicExample;

public record BasicSong(
       int TrackNumber,
       string TrackName,
       double Duration
   );

public class BasicSongValidator : AbstractSubValidator<BasicSong>
{
    public BasicSongValidator()
    {
        Describe(x => x.TrackName)
            .IsNotEmpty();

        Describe(x => x.Duration)
            .IsGreaterThan(2.0)
            .IsLessThan(5.0);

        When(
            "When track number is 13",
            x => Task.FromResult(x.TrackNumber == 13),
            () =>
            {
                Describe(x => x.Duration)
                     .IsLessThan(
                         1.0,
                         o => o
                             .WithErrorMessage("The 13th Song must be an intermission.")
                             .WithDescription("The 13th song must be a shorter than 1 minute long intermission.")
                     );
            });
    }
}