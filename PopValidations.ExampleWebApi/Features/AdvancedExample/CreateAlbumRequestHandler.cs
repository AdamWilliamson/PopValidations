using MediatR;

namespace PopValidations.ExampleWebApi.Features.AdvancedExample;

public record CreateAlbumRequest(List<Album?>? Albums) : IRequest<CreateAlbumResponse>;

public class AlbumSubmissionValidator : AbstractValidator<CreateAlbumRequest>
{
    public AlbumSubmissionValidator(AlbumVerificationService albumVerificationService)
    {
        DescribeEnumerable(x => x.Albums)
            .Vitally().IsNotNull()
            .ForEach(x => x
                .Vitally().IsNotNull()
                .SetValidator(new AlbumValidator(albumVerificationService))
            );
    }
}

public class CreateAlbumRequestHandler : IRequestHandler<CreateAlbumRequest, CreateAlbumResponse>
{
    public Task<CreateAlbumResponse> Handle(CreateAlbumRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new CreateAlbumResponse());
    }
}

public class CreateAlbumResponse { }