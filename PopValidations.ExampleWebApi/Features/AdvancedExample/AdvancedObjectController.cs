using MediatR;
using Microsoft.AspNetCore.Mvc;
using PopValidations.ExampleWebApi.Features.BasicExample;

namespace PopValidations.ExampleWebApi.Features.AdvancedExample;

[ApiController]
[Route("api/[controller]")]
public class AdvancedObjectController : Controller
{
    private readonly IMediator mediator;

    public AdvancedObjectController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [Route(nameof(Create))]
    [HttpPut]
    public Task<CreateAlbumResponse> Create(CreateAlbumRequest album)
    {
        return mediator.Send(album);
    }

    [Route(nameof(Update))]
    [HttpPut]
    public Task<UpdateAlbumResponse> Update(UpdateAlbumRequest album)
    {
        return mediator.Send(album);
    }
}
