using PopValidations.Swashbuckle_Tests.Helpers;
using System.Threading.Tasks;

namespace PopValidations.Demonstration_Tests.Examples.Basic;

public static class BasicDemonstration
{
    public record BasicSong(
        int TrackNumber,
        string TrackName,
        double Duration
    );

    public class BasicSongValidator : AbstractValidator<BasicSong>
    {
        public BasicSongValidator()
        {
            Describe(x => x.TrackName)
                .IsNotEmpty();

            Describe(x => x.Duration)
                .IsGreaterThan(2.0)
                .IsLessThan(5.0);

            When(
                "When track is 13",
                x => Task.FromResult(x.TrackNumber == 13),
                () =>
                {
                    Include(new ThirteenthTrackSongValidator());
                });
        }
    }

    public class ThirteenthTrackSongValidator : AbstractSubValidator<BasicSong>
    {
        public ThirteenthTrackSongValidator()
        {
            Describe(x => x.Duration)
                .IsLessThan(
                    1.0,
                    o => o
                        .WithErrorMessage("The 13th Song must be an intermission.")
                        .WithDescription("The 13th song must be a shorter than 1 minute long intermission.")
                );
        }
    }

    public class TestController : ControllerBase<BasicSong> { }
}