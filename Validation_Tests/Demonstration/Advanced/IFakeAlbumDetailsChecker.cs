using System.Threading.Tasks;

namespace PopValidations_Tests.Demonstration.Advanced;

public interface IFakeAlbumDetailsChecker
{
    Task<bool> DoWeOwnRightsToSong(AdvancedSong song);
}

public class FakeAlbumDetailsChecker : IFakeAlbumDetailsChecker
{
    public Task<bool> DoWeOwnRightsToSong(AdvancedSong song)
    {
        return Task.FromResult(false);
    }
}