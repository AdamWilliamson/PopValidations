using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PopValidations.Demonstration_Tests.Examples.Advanced;

public interface IFakeAlbumDetailsChecker
{
    Task<List<(AdvancedSong?, bool)>> DoWeOwnRightsToSong(List<AdvancedSong?>? songs);
}

public class FakeAlbumDetailsChecker : IFakeAlbumDetailsChecker
{
    public Task<List<(AdvancedSong?, bool)>> DoWeOwnRightsToSong(List<AdvancedSong?>? songs)
    {
        return Task.FromResult(
            songs?.Where(x => x != null)?.Select(song => (song, false)).ToList() ?? new()
        );
    }
}