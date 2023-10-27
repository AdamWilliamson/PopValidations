using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace PopValidations.ExampleWebApi.Features.BasicExample;

[ApiController]
[Route("api/[controller]")]
public class BasicObjectController : Controller
{
    private readonly IMediator mediator;

    public BasicObjectController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [Route(nameof(Create))]
    [HttpPut]
    public Task<CreateSongResponse> Create(CreateSongRequest album)
    {
        return mediator.Send(album);
    }

    [Route(nameof(Update))]
    [HttpPut]
    public Task<UpdateSongResponse> Update(UpdateSongRequest album)
    {
        return mediator.Send(album);
    }
}
