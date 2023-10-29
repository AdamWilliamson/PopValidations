using MediatR;

namespace PopValidations.ExampleWebApi.Features.AdvancedExample;

public record UpdateAlbumRequest(int Id, List<Album?>? Albums) : IRequest<UpdateAlbumResponse>;

public class EditAlbumRequestValidator : AbstractValidator<UpdateAlbumRequest>
{
    public EditAlbumRequestValidator(AlbumVerificationService albumVerificationService)
    {
        DescribeEnumerable(x => x.Albums)
            .Vitally().IsNotNull()
            .ForEach(x => x
                .Vitally().IsNotNull()
                .SetValidator(new AlbumValidator(albumVerificationService))
            );
    }
}

public class UpdateAlbumRequestHandler : IRequestHandler<UpdateAlbumRequest, UpdateAlbumResponse>
{
    public Task<UpdateAlbumResponse> Handle(UpdateAlbumRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new UpdateAlbumResponse());
    }
}

public class UpdateAlbumResponse { }