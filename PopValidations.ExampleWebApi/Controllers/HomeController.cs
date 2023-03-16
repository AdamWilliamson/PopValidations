using MediatR;
using Microsoft.AspNetCore.Mvc;
using PopValidations.ExampleWebApi.Handlers;

namespace PopValidations.ExampleWebApi.Controllers;

public record Artist(string Name);
public record Song(string Name, Artist? ArtistAgain);
public record Album(string Name, Artist? Artist, List<Song>? Songs);

public class ArtistValidator : AbstractSubValidator<Artist>
{
    public ArtistValidator()
    {
        Describe(x => x.Name).NotNull();
    }
}

public class SongValidator: AbstractSubValidator<Song>
{
    public SongValidator()
    {
        Describe(x => x.Name)
            .NotNull()
            .IsLengthInclusivelyBetween(5, 200);
        Describe(x => x.ArtistAgain)
            .NotNull()
            .SetValidator(new ArtistValidator());

        When(
            "Song: Artist is not null",
            x => Task.FromResult(x.ArtistAgain != null),
            () =>
            {
                Describe(x => x.ArtistAgain)
                    .NotNull()
                    .SetValidator(new ArtistValidator());
            });
    }
}

public class AlbumValidator : AbstractValidator<Album> 
{
    public AlbumValidator()
    {
        Describe(x => x.Name)
            .NotNull()
            .IsEqualTo("Hello")
            .IsLengthExclusivelyBetween(4, 201);
        DescribeEnumerable(x => x.Songs)
            .NotNull()
            .ForEach(x => x.SetValidator(new SongValidator()));

        When(
            "Artist is not null",
            x => Task.FromResult(x.Artist != null),
            () =>
            {
                Describe(x => x.Artist)
                    .SetValidator(new ArtistValidator());
                Describe(x => x.Name)
                    .NotNull()
                    .IsEqualTo("Hello")
                    .IsEqualTo("Hello2")
                    .IsEqualTo("Hello4");

                When(
                    "Artist is not null",
                    x => Task.FromResult(x.Artist != null),
                    () =>
                    {
                        Describe(x => x.Artist)
                            .SetValidator(new ArtistValidator());

                        Describe(x => x.Name)
                            .NotNull()
                            .IsEqualTo("Hello")
                            .IsEqualTo("Hello2")
                            .IsEqualTo("Hello4");
                    });
            });
    }
}

[ApiController]
[Route("api/[controller]")]
public class HomeController : ControllerBase
{
    private readonly IMediator mediator;

    public HomeController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [Route(nameof(Details))]
    [HttpGet]
    public Album Details(int id)
    {
        return new Album("Album Name", null, null);
    }

    [Route(nameof(Edit))]
    [HttpPut]
    public Task Edit(int id, Album album)
    {
        return mediator.Send(new TestRequest());
    }
}
