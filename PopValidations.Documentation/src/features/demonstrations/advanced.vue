<template>
  <v-container fluid bg-color="surface">
    <v-row>
      <v-col>
        <v-card>
          <v-card-title><h3>Advanced Demonstration</h3></v-card-title>
          <v-card-text>Some of the more advanced concepts all in one demo. From complex arrangements of Foreach and ScopeWhen, to ScopedData modifications, and injecting services.</v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <PanelsOrTabs>
      <template #code>
        <CodeWindow
              language="csharp"
              source='public class ArtistValidator : AbstractSubValidator<Artist>
    {
        public ArtistValidator()
        {
            Describe(x => x.Name).IsNotNull();
        }
    }

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
    
    public class AlbumValidator : AbstractSubValidator<Album>
    {
        public AlbumValidator(AlbumVerificationService albumVerificationService)
        {
            Describe(x => x.Title).Vitally().IsNotEmpty();

            Describe(x => x.Type).Vitally().IsNotNull();

            DescribeEnumerable(x => x.Artists).Vitally().IsNotNull()
                .ForEach(x => x.Vitally().IsNotNull().SetValidator(new ArtistValidator()));

            Describe(x => x.CoverImageUrl).Vitally().IsNotEmpty();

            Describe(x => x.Created)
                .Vitally().IsNotNull()
                .IsGreaterThan(new DateTime(1700, 01, 01, 13, 0, 0, 0))
                .IsLessThan(new DateTime(2024, 01, 01, 13, 0, 0, 0));

            DescribeEnumerable(x => x.Songs)
                .Vitally().IsNotNull()
                .IsLengthInclusivelyBetween(3, 45)
                .ForEach(song => song.Vitally().IsNotNull().SetValidator(new SongValidator()));

            Scope("Validate Album",
                async (album) => await albumVerificationService.GetAlbumChecker(album),
                (albumChecker) =>
                {
                    When(
                        "Album is Compliation",
                        (album) => Task.FromResult(album?.Type == AlbumType.Compilation),
                        () =>
                        {
                            Describe(x => x.Artists)
                                .Is(
                                    "Validated to must {{is_value}}",
                                    "Failed to {{is_value}}",
                                    albumChecker.To(
                                        "have different artists", 
                                        (List<Artist>? x, AlbumChecker? i) 
                                            => 
                                                x?.Any() == true 
                                                && i is { IsAllTheSameArtist: false }
                                    )
                                );
                        });

                    When(
                        "Album is Single Artist",
                        (album) => Task.FromResult(album?.Type == AlbumType.SingleArtist),
                        () =>
                        {
                            Describe(x => x.Artists)
                                .Is(
                                    "Must {{is_value}}",
                                    "Does not {{is_value}}",
                                    albumChecker.To(
                                        "have all the same artists",
                                        (List<Artist>? x, AlbumChecker? i)
                                            =>
                                                x?.Any() == true
                                                && i is not { IsAllTheSameArtist: true }
                                    )
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
                            albumChecker.To("", (IEnumerable<Song?>? x, AlbumChecker? i) => i?.AllSongsContainAlbumArtist is true)
                        );
                });

            ScopeWhen(
                "Need the Database Checker to When",
                async (album) => await albumVerificationService.GetAlbumChecker(album),
                "When Album is Single",
                (album, albumChecker) => album?.Type == AlbumType.Single,
                (albumChecker) =>
                {
                    Describe(x => x.Songs)
                        .Vitally().Is(
                            "Album must match the rules for single.",
                            "Must Abide by the rules for singles.",
                            albumChecker.To(
                                "Album is Single",
                                (List<Song?>? x, AlbumChecker? i) => i?.IsSingle is true)
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
                .Vitally().IsNotNull()
                .ForEach(x => x
                    .Vitally().IsNotNull()
                    .SetValidator(new AlbumValidator(albumVerificationService)));

            Scope("Validate Album In Submission",
                (album) => album != null,
                (albumChecker) =>
                {
                    Describe(x => x.Albums[0]).SetValidator(new AlbumListValidator());
                });
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
            get => album.Artists?.All(x => x != null && x?.Name == (album.Artists?.FirstOrDefault()?.Name ?? "")) ?? true;
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
    }'
            ></CodeWindow>
      </template>

      <template #request>
        <CodeWindow
              language="csharp"
              source='public record Artist(string? Name);

public record Song(
    List<Artist>? Artists,
    int? TrackNumber,
    string? TrackName,
    double? Duration,
    string? Genre
);

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
);'
            ></CodeWindow>
      </template>

      <template #errorreport>
        <CodeWindow
              language="json"
              source='{
    "errors": {
        "albums[0].Artists": [
            "Does not have all the same artists"
        ],
        "albums[0].Songs[0].Duration": [
            "Song does not have a duration."
        ],
        "albums[0].Songs[1].Duration": [
            "Song does not have a duration."
        ],
        "albums[0].Songs[2]": [
            "Is null."
        ],
        "albums[1].CoverImageUrl": [
            "Is empty."
        ],
        "albums[1].Created": [
            "Is null."
        ],
        "albums[1].Songs": [
            "Is not between 3 and 45 inclusive.",
            "The songs in this album, being collaboration, must contain atleast 1 album artist."
        ],
        "albums[1].Title": [
            "Is empty."
        ],
        "albums[2].Artists": [
            "Failed to have different artists"
        ],
        "albums[2].CoverImageUrl": [
            "Is empty."
        ],
        "albums[2].Created": [
            "Is null."
        ],
        "albums[2].Songs": [
            "Is not between 3 and 45 inclusive."
        ],
        "albums[2].Songs[0].Duration": [
            "Song does not have a duration."
        ],
        "albums[2].Songs[0].TrackName": [
            "Is empty."
        ],
        "albums[2].Songs[0].TrackNumber": [
            "Is null."
        ],
        "albums[2].Songs[1].Duration": [
            "Song does not have a duration."
        ],
        "albums[2].Songs[1].TrackName": [
            "Is empty."
        ],
        "albums[2].Songs[1].TrackNumber": [
            "Is null."
        ],
        "albums[2].Title": [
            "Is empty."
        ],
        "albums[3].CoverImageUrl": [
            "Is empty."
        ],
        "albums[3].Created": [
            "Is null."
        ],
        "albums[3].Songs": [
            "Must Abide by the rules for singles."
        ],
        "albums[3].Title": [
            "Is empty."
        ],
        "albums[4]": [
            "Is null."
        ]
    }
}'
            ></CodeWindow>
      </template>
      <template #openapi>
        <CodeWindow
              language="json"
              source='{
    "Album": {
        "required": [
            "artists",
            "coverImageUrl",
            "created",
            "songs",
            "title",
            "type"
        ],
        "type": "object",
        "properties": {
            "title": {
                "minLength": 1,
                "minItems": 1,
                "type": "string",
                "nullable": true
            },
            "type": {
                "$ref": "#/components/schemas/AlbumType"
            },
            "artists": {
                "type": "array",
                "items": {
                    "$ref": "#/components/schemas/Artist"
                }
            },
            "coverImageUrl": {
                "minLength": 1,
                "minItems": 1,
                "type": "string",
                "nullable": true
            },
            "created": {
                "type": "string",
                "format": "date-time"
            },
            "songs": {
                "maxLength": 45,
                "minLength": 3,
                "type": "array",
                "items": {
                    "$ref": "#/components/schemas/Song"
                }
            },
            "genres": {
                "type": "array",
                "items": {
                    "type": "string"
                },
                "nullable": true
            }
        },
        "additionalProperties": false,
        "x-validation": {
            "title": [
                "Must not be empty."
            ],
            "type": [
                "Must not be null."
            ],
            "artists": [
                "Must not be null.",
                "Album is Compliation : Validated to must have different artists",
                "Album is Single Artist : Must have all the same artists",
                "Must not be null."
            ],
            "coverImageUrl": [
                "Must not be empty."
            ],
            "created": [
                "Must not be null.",
                "Must be greater than `1/01/1700 1:00:00 PM`.",
                "Must be less than `1/01/2024 1:00:00 PM``."
            ],
            "songs": [
                "Must not be null.",
                "Must be between 3 and 45 inclusive.",
                "When Album is Collaboration : All songs must contain atleast one album artist.",
                "When Album is Single : Album must match the rules for single.",
                "Must not be null."
            ]
        }
    },
    "AlbumSubmission": {
        "required": [
            "albums"
        ],
        "type": "object",
        "properties": {
            "albums": {
                "type": "array",
                "items": {
                    "$ref": "#/components/schemas/Album"
                }
            }
        },
        "additionalProperties": false,
        "x-validation": {
            "albums": [
                "Must not be null.",
                "Must not be null."
            ]
        }
    },
    "AlbumType": {
        "enum": [
            0,
            1,
            2,
            3
        ],
        "type": "integer",
        "format": "int32"
    },
    "Artist": {
        "required": [
            "name"
        ],
        "type": "object",
        "properties": {
            "name": {
                "type": "string"
            }
        },
        "additionalProperties": false,
        "x-validation": {
            "name": [
                "Must not be null.",
                "Must not be null."
            ]
        }
    },
    "Song": {
        "required": [
            "artists",
            "duration",
            "genre",
            "trackName",
            "trackNumber"
        ],
        "type": "object",
        "properties": {
            "artists": {
                "type": "array",
                "items": {
                    "$ref": "#/components/schemas/Artist"
                }
            },
            "trackNumber": {
                "type": "integer",
                "format": "int32"
            },
            "trackName": {
                "minLength": 1,
                "minItems": 1,
                "type": "string",
                "nullable": true
            },
            "duration": {
                "minimum": 0,
                "exclusiveMinimum": true,
                "type": "number",
                "format": "double"
            },
            "genre": {
                "minLength": 1,
                "minItems": 1,
                "type": "string",
                "nullable": true
            }
        },
        "additionalProperties": false,
        "x-validation": {
            "artists": [
                "Must not be null.",
                "Must not be null."
            ],
            "trackNumber": [
                "Must not be null."
            ],
            "trackName": [
                "Must not be empty."
            ],
            "duration": [
                "Must not be null.",
                "Songs must have a positive duration."
            ],
            "genre": [
                "Must not be empty."
            ]
        }
    }
}'
            ></CodeWindow>
      </template>
    </PanelsOrTabs>

  </v-container>
</template>