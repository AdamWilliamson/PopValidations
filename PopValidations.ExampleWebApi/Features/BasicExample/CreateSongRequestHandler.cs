using MediatR;

namespace PopValidations.ExampleWebApi.Features.BasicExample;

public class CreateSongRequest : IRequest<CreateSongResponse>
{
    public BasicSong Song { get; set; }
}

public class CreateSongRequestValidator : AbstractValidator<CreateSongRequest>
{
    public CreateSongRequestValidator()
    {
        Describe(x => x.Song)
            .Vitally().IsNotNull()
            .SetValidator(new BasicSongValidator());
    }
}

public class CreateSongRequestHandler : IRequestHandler<CreateSongRequest, CreateSongResponse>
{
    public Task<CreateSongResponse> Handle(CreateSongRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new CreateSongResponse());
    }
}

public class CreateSongResponse { }