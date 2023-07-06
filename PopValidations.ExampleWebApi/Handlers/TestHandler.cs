using MediatR;
using System.Reflection;

namespace PopValidations.ExampleWebApi.Handlers;

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

//public class SongValidator : AbstractValidator<Song>
//{
//    public SongValidator()
//    {
//        Include(new SongSubValidator());
//        Describe(x => x.Name)
//            .NotNull()
//            .IsLengthInclusivelyBetween(50, 100);
//        //Describe(x => x.ArtistAgain)
//        //    .NotNull()
//        //    .SetValidator(new ArtistValidator());

//        //When(
//        //    "Song: Artist is not null",
//        //    x => Task.FromResult(x.ArtistAgain != null),
//        //    () =>
//        //    {
//        //        Describe(x => x.ArtistAgain)
//        //            .NotNull()
//        //            .SetValidator(new ArtistValidator());
//        //    });
//    }
//}

public class SongSubValidator : AbstractSubValidator<Song>
{
    public SongSubValidator()
    {
        Describe(x => x.Name)
            .NotNull()
            .IsLengthInclusivelyBetween(5, 200)
            ;

        Describe(x => x.ArtistAgain)
            .NotNull()
            .SetValidator(new ArtistValidator())
            ;

        //When(
        //    "Song: Artist is not null",
        //    x => Task.FromResult(x.ArtistAgain != null),
        //    () =>
        //    {
        //        Describe(x => x.ArtistAgain)
        //            .NotNull()
        //            //.SetValidator(new ArtistValidator())
        //            ;
        //    });
    }
}

public class AlbumValidator : AbstractSubValidator<Album>
{
    public AlbumValidator()
    {
        //Describe(x => x.Name)
        //    .NotNull()
        //    .IsEqualTo("Hello")
        //    .IsLengthExclusivelyBetween(4, 201); 
        DescribeEnumerable(x => x.Songs)
            .NotNull()
            .ForEach(x => x.SetValidator(new SongSubValidator()))
            ;

        //When(
        //    "Artist is not null",
        //    x => Task.FromResult(x.Artist != null),
        //    () =>
        //    {
        //        Describe(x => x.Artist)
        //            .SetValidator(new ArtistValidator())
        //            ;
        //        Describe(x => x.Name)
        //            .NotNull()
        //            .IsEqualTo("Hello")
        //            .IsEqualTo("Hello2")
        //            .IsEqualTo("Hello4");

        //        When(
        //            "Artist is not null",
        //            x => Task.FromResult(x.Artist != null),
        //            () =>
        //            {
        //                Describe(x => x.Artist)
        //                    .SetValidator(new ArtistValidator())
        //                    ;

        //                Describe(x => x.Name)
        //                    .NotNull()
        //                    .IsEqualTo("Hello")
        //                    .IsEqualTo("Hello2")
        //                    .IsEqualTo("Hello4");
        //            });
        //    });
    }
}

public class EditAlbumRequest : IRequest<TestResponse> 
{
    public  int Id { get; set; }
    public Album Album { get; set; }
}

public class EditAlbumRequestValidator : AbstractValidator<EditAlbumRequest> 
{
    public EditAlbumRequestValidator()
    {
        //Describe(x => x.Id)
        //    .Vitally().NotNull()
        //    .IsNotEmpty();

        Describe(x => x.Album)
            //.Vitally().NotNull()
            .SetValidator(new AlbumValidator())
            ;
    }
}

public class EditAlbumRequestHandler : IRequestHandler<EditAlbumRequest, TestResponse>
{
    public Task<TestResponse> Handle(EditAlbumRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new TestResponse());
    }
}

public class TestResponse { }