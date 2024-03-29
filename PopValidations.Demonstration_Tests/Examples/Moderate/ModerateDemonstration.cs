﻿using PopValidations.Swashbuckle_Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PopValidations.Demonstration_Tests.Examples.Moderate;

public static class ModerateDemonstration
{
    //Begin-Request
    public record ModerateSong(
        string? Artist,
        int? TrackNumber,
        string? TrackName,
        double? Duration,
        string? Genre
    );

    public record ModerateAlbum(
        string? Artist,
        string? Genre,
        List<ModerateSong?>? Songs
    );
    //End-Request

    //Begin-Validator
    public class AlbumValidator : AbstractValidator<ModerateAlbum>
    {
        public AlbumValidator()
        {
            Describe(x => x.Artist)
                .Vitally().IsNotEmpty()
                .IsLengthInclusivelyBetween(3, 500);

            Describe(x => x.Genre)
                .Vitally().IsNotEmpty()
                .IsLengthInclusivelyBetween(3, 300);

            DescribeEnumerable(x => x.Songs)
                .Vitally().IsNotNull()
                .ForEach(song =>
                    song
                        .Vitally().IsNotNull()
                );

            When(
                "When Genre is Rock",
                (album) => Task.FromResult(album?.Genre == "Rock"),
                () =>
                {
                    Include(new RockAlbumValidator());

                    When(
                        "When Track count is greater than 20 treat it like an anthology",
                        (album) => Task.FromResult((album.Songs?.Count ?? 0) > 20), 
                        () => 
                        {
                            Include(new AnthologyAlbumValidator());
                        });

                    When(
                        "When Track count is less than 20",
                        (album) => Task.FromResult((album.Songs?.Count ?? 0) > 20), 
                        () =>
                        {
                            Include(new RockNotAnthologyAlbumValidator());
                        });
                });
        }
    }

    public class RockAlbumValidator : AbstractSubValidator<ModerateAlbum>
    {
        public RockAlbumValidator()
        {
            DescribeEnumerable(x => x.Songs)
                .IsLengthExclusivelyBetween(6, 15)
                .ForEach(song =>
                    song.SetValidator(new RockSongValidator())
                );
        }
    }

    public class AnthologyAlbumValidator : AbstractSubValidator<ModerateAlbum>
    {
        public AnthologyAlbumValidator()
        {
            DescribeEnumerable(x => x.Songs)
                .ForEach(song =>
                    song.SetValidator(new AnthologySongValidator())
                );
        }
    }

    public class RockNotAnthologyAlbumValidator : AbstractSubValidator<ModerateAlbum>
    {
        public RockNotAnthologyAlbumValidator()
        {
            DescribeEnumerable(x => x.Songs)
                .ForEach(song =>
                    song .SetValidator(new NotRockOrAnthologySongValidator())
                );
        }
    }

    public class BaseSongValidator : AbstractSubValidator<ModerateSong>
    {
        public BaseSongValidator()
        {
            Describe(x => x.Genre)
                .Vitally().IsNotEmpty(o => o
                       .WithErrorMessage("Song does not have a Title.")
                       .WithDescription("Songs need Titles."));
        }
    }

    public class RockSongValidator : AbstractSubValidator<ModerateSong>
    {
        public RockSongValidator()
        {
            Include(new BaseSongValidator());

            Describe(x => x.Duration)
                .IsGreaterThan(1)
                .IsLessThan(5);
        }
    }

    public class AnthologySongValidator : AbstractSubValidator<ModerateSong>
    {
        public AnthologySongValidator()
        {
            Include(new BaseSongValidator());

            Describe(x => x.Duration)
                .IsGreaterThan(1)
                .IsLessThan(5);
        }
    }

    public class NotRockOrAnthologySongValidator : AbstractSubValidator<ModerateSong>
    {
        public NotRockOrAnthologySongValidator()
        {
            Include(new BaseSongValidator());

            Describe(x => x.Duration)
                .IsGreaterThan(1)
                .IsLessThan(5);
        }
    }
    //End-Validator

    public class TestController : ControllerBase<ModerateAlbum> { }
}