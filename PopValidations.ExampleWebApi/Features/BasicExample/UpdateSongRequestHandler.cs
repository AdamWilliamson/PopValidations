using MediatR;

namespace PopValidations.ExampleWebApi.Features.BasicExample;

public class UpdateSongRequest : IRequest<UpdateSongResponse>
{
    public int Id { get; set; }
    public BasicSong Song { get; set; }
}

public class EditAlbumRequestValidator : AbstractValidator<UpdateSongRequest>
{
    public EditAlbumRequestValidator()
    {
        Describe(x => x.Song)
            .Vitally().IsNotNull()
            .SetValidator(new BasicSongValidator());
    }
}

public class UpdateSongRequestHandler : IRequestHandler<UpdateSongRequest, UpdateSongResponse>
{
    public Task<UpdateSongResponse> Handle(UpdateSongRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new UpdateSongResponse());
    }
}

public class UpdateSongResponse { }