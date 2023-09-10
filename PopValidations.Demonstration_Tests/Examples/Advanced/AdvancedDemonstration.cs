﻿using PopValidations.Swashbuckle_Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PopValidations.Demonstration_Tests.Examples.Advanced;

public static class AdvancedDemonstration
{
    public record Artist(string? Name);

    public class ArtistValidator : AbstractSubValidator<Artist>
    {
        public ArtistValidator()
        {
            Describe(x => x.Name).IsNotNull();
        }
    }

    public record Song(
        List<Artist>? Artists,
        int? TrackNumber,
        string? TrackName,
        double? Duration,
        string? Genre
    );

    public class SongValidator : AbstractSubValidator<Song>
    {
        public SongValidator()
        {
            Describe(x => x.TrackNumber).Vitally().IsNotNull();

            Describe(x => x.TrackName).Vitally().IsNotEmpty();

            Describe(x => x.Duration)
                .Vitally().IsNotNull()
                .IsGreaterThan(0,
                    o => o
                        .WithErrorMessage("Song does not have a duration.")
                        .WithDescription("Songs must have a positive duration.")
                );

            Describe(x => x.Genre).Vitally().IsNotEmpty();

            DescribeEnumerable(x => x.Artists)
                .Vitally().IsNotNull()
                .ForEach(x => x.Vitally().IsNotNull().SetValidator(new ArtistValidator()));
        }
    }

    public enum AlbumType
    {
        Single,
        SingleArtist,
        Collaboration,
        Compilation
    }

    public record Album(
        string? Title,
        AlbumType? Type,
        List<Artist>? Artists,
        string? CoverImageUrl,
        DateTime? Created,
        List<Song?>? Songs,
        List<string>? Genres
    );

    public class AlbumValidator : AbstractSubValidator<Album>
    {
        public AlbumValidator(AlbumVerificationService albumVerificationService)
        {
            //Describe(x => x.Title).Vitally().IsNotEmpty();

            //Describe(x => x.Type).Vitally().IsNotNull();

            //DescribeEnumerable(x => x.Artists).Vitally().IsNotNull()
            //    .ForEach(x => x.Vitally().IsNotNull().SetValidator(new ArtistValidator()));

            //Describe(x => x.CoverImageUrl).Vitally().IsNotEmpty();

            //Describe(x => x.Created)
            //    .Vitally().IsNotNull()
            //    .IsGreaterThan(new DateTime(1700, 01, 01, 13, 0, 0, 0))
            //    .IsLessThan(new DateTime(2024, 01, 01, 13, 0, 0, 0));

            //DescribeEnumerable(x => x.Songs)
            //    .Vitally().IsNotNull()
            //    .IsLengthInclusivelyBetween(3, 45)
            //    .ForEach(song => song.Vitally().IsNotNull().SetValidator(new SongValidator()));

            Scope("Validate Album",
                async (album) => await albumVerificationService.GetAlbumChecker(album),
                (albumChecker) =>
                {
                    When(
                        "Album is Compliation",
                        (album) => Task.FromResult(album.Type == AlbumType.Compilation),
                        () =>
                        {
                            Describe(x => x.Artists)
                                .Is(
                                    "Validated to {{value}}",
                                    "Something",
                                    albumChecker.To<bool>("have different artists", x => x is { IsAllTheSameArtist: true })
                                );
                        });

                    When(
                        "Album is Single Artist",
                        (album) => Task.FromResult(album.Type == AlbumType.SingleArtist),
                        () =>
                        {
                            Describe(x => x.Artists)
                                .Is(
                                    "Must {{value}}",
                                    "Does not {{value}}",
                                    albumChecker.To("have all the same artists", x => x is { IsAllTheSameArtist: false })
                                );
                        });
                }
            );

            ScopeWhen(
                "When Album is Collaboration",
                (album) => Task.FromResult(album.Type == AlbumType.Collaboration),
                "Get Complex Album Validator",
                (album) => albumVerificationService.GetAlbumChecker(album),
                (albumChecker) =>
                {
                    DescribeEnumerable(x => x.Songs)
                        .Vitally().Is(
                            "All songs must contain atleast one album artist.",
                            "The songs in this album, being collaboration, must contain atleast 1 album artist.",
                            albumChecker.To("", i => i.AllSongsContainAlbumArtist is true)
                        );
                });

            ScopeWhen(
                "When Album is Single",
                "Need the Database Checker to When",
                async (album) => await albumVerificationService.GetAlbumChecker(album),
                (album, albumChecker) => album.Type == AlbumType.Single,
                (albumChecker) =>
                {
                    Describe(x => x.Songs)
                        .Vitally().Is(
                            "Album must match the rules for single.",
                            "Must Abide by the rules for singles.",
                            albumChecker.To("Album is Single", i => i.IsSingle is true)
                        );
                });
        }
    }

    public record AlbumSubmission(List<Album?>? Albums);

    public class AlbumSubmissionValidator : AbstractValidator<AlbumSubmission>
    {
        public AlbumSubmissionValidator(AlbumVerificationService albumVerificationService)
        {
            DescribeEnumerable(x => x.Albums)
                //.Vitally().IsNotNull()
                .ForEach(x => x
                    //.Vitally().IsNotNull()
                    .SetValidator(new AlbumValidator(albumVerificationService)));

            //Scope("Validate Album In Submission",
            //    (album) => album != null,
            //    (albumChecker) =>
            //    {
            //        Describe(x => x.Albums[0]).SetValidator(new AlbumListValidator());
            //    });
        }
    }

    public class AlbumListValidator : AbstractSubValidator<Album?>
    {
        public AlbumListValidator()
        {
            Scope("Validate Albumis list validator",
                (album) => album != null,
                (albumChecker) =>
                {
                    Describe(x => x.Type).IsEqualTo(AdvancedDemonstration.AlbumType.SingleArtist);
                });
        }
    }

    public class AlbumVerificationService
    {
        public Task<AlbumChecker> GetAlbumChecker(Album album)
        {
            return Task.FromResult(new AlbumChecker(album));
        }
    }

    public class AlbumChecker
    {
        private readonly Album album;

        public AlbumChecker(Album album)
        {
            this.album = album;
        }

        public bool IsAllTheSameArtist
        {
            get => album.Artists?.All(x => x?.Name == (album.Artists?.FirstOrDefault()?.Name ?? "")) ?? true;
        }

        public bool AllSongsContainAlbumArtist
        {
            get => album.Artists
                ?.Any(a => 
                    album?.Songs
                        ?.All(song => 
                            song?.Artists
                                ?.Any(artist => artist.Name == a?.Name)
                                ?? false
                        ) ?? false
                ) ?? false;
        }

        public bool IsSingle => album.Type == AlbumType.Single && album?.Songs?.Count <= 7;
    }

    public class TestController : ControllerBase<AlbumSubmission> { }
}