using MediatR;
using Microsoft.AspNetCore.Mvc;
using PopValidations.ExampleWebApi.Handlers;

namespace PopValidations.ExampleWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HomeController : Controller
{
    private readonly IMediator mediator;

    public HomeController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    //[Route(nameof(Details) + "/{id}")]
    //[HttpGet]
    //public Album Details(int id)
    //{
    //    return new Album("Album Name", null, null);
    //}

    //[Route(nameof(Edit))]
    //[HttpPut]
    //public Task Edit(Song song)
    //{
    //    //, [FromBody]Album album 
    //    //, Album = album
    //    //return mediator.Send(album);
    //    return Task.CompletedTask;
    //}

    [Route(nameof(Edit2))]
    [HttpPut]
    public Task Edit2(EditAlbumRequest album)
    {
        //, [FromBody]Album album 
        //, Album = album
        return mediator.Send(album);
    }
}
