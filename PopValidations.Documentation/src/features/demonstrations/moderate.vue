<template>
  <v-container fluid bg-color="surface">
    <v-row>
      <v-col>
        <v-card>
          <v-card-title><h3>Moderate Demonstration</h3></v-card-title>
          <v-card-text></v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <PanelsOrTabs>
      <template #code>
        <CodeWindow
              language="csharp"
              source='public class AlbumValidator : AbstractValidator<ModerateAlbum>
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
                });

            When(
                "When Genre is Not Rock",
                (album) => Task.FromResult(album?.Genre != "Rock"),
                () =>
                {
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
                            Include(new NotRockOrAnthologyAlbumValidator());
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

    public class NotRockOrAnthologyAlbumValidator : AbstractSubValidator<ModerateAlbum>
    {
        public NotRockOrAnthologyAlbumValidator()
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
    }'
            ></CodeWindow>
      </template>

      <template #request>
        <CodeWindow
              language="csharp"
              source='public record ModerateSong(
        string Artist,
        int? TrackNumber,
        string TrackName,
        double Duration,
        string Genre
    );

    public record ModerateAlbum(
        string Artist,
        string Genre,
        List<ModerateSong?> Songs
    );'
            ></CodeWindow>
      </template>

      <template #errorreport>
        <CodeWindow
              language="json"
              source='{
    "errors": {
        "songs": [
            "Is not between 6 and 15 exclusive."
        ],
        "songs[0].Duration": [
            "Is not greater than `1`.",
            "Is not less than `5`."
        ],
        "songs[1]": [
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
    "ModerateAlbum": {
        "required": [
            "artist",
            "genre",
            "songs"
        ],
        "type": "object",
        "properties": {
            "artist": {
                "maxLength": 500,
                "minLength": 3,
                "minItems": 1,
                "type": "string",
                "nullable": true
            },
            "genre": {
                "maxLength": 300,
                "minLength": 3,
                "minItems": 1,
                "type": "string",
                "nullable": true
            },
            "songs": {
                "type": "array",
                "items": {
                    "$ref": "#/components/schemas/ModerateSong"
                }
            }
        },
        "additionalProperties": false,
        "x-validation": {
            "artist": [
                "Must not be empty.",
                "Must be between 3 and 500 inclusive."
            ],
            "genre": [
                "Must not be empty.",
                "Must be between 3 and 300 inclusive."
            ],
            "songs": [
                "Must not be null.",
                "When Genre is Rock : Must be between 6 and 15 exclusive.",
                "Must not be null."
            ]
        }
    },
    "ModerateSong": {
        "type": "object",
        "properties": {
            "artist": {
                "type": "string",
                "nullable": true
            },
            "trackNumber": {
                "type": "integer",
                "format": "int32",
                "nullable": true
            },
            "trackName": {
                "type": "string",
                "nullable": true
            },
            "duration": {
                "type": "number",
                "format": "double"
            },
            "genre": {
                "type": "string",
                "nullable": true
            }
        },
        "additionalProperties": false,
        "x-validation": {
            "duration": [
                "When Genre is Rock : Must be greater than `1`.",
                "When Genre is Rock : Must be less than `5`.",
                "When Genre is Not Rock & When Track count is greater than 20 treat it like an anthology : Must be greater than `1`.",
                "When Genre is Not Rock & When Track count is greater than 20 treat it like an anthology : Must be less than `5`.",
                "When Genre is Not Rock & When Track count is less than 20 : Must be greater than `1`.",
                "When Genre is Not Rock & When Track count is less than 20 : Must be less than `5`."
            ],
            "genre": [
                "When Genre is Rock : Songs need Titles.",
                "When Genre is Not Rock & When Track count is greater than 20 treat it like an anthology : Songs need Titles.",
                "When Genre is Not Rock & When Track count is less than 20 : Songs need Titles."
            ]
        }
    }
}'
            ></CodeWindow>
      </template>
    </PanelsOrTabs>

  </v-container>
</template>