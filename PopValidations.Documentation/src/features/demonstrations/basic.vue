<template>
  <v-container fluid bg-color="surface">
    <v-row>
      <v-col>
        <v-card>
          <v-card-title><h3>Basic Demonstration</h3></v-card-title>
          <v-card-text></v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <PanelsOrTabs>
      <template #code>
        <CodeWindow
              language="csharp"
              source='
              public class BasicSongValidator : AbstractValidator<BasicSong>
{
    public BasicSongValidator()
    {
        Describe(x => x.TrackNumber)
            .Vitally().IsNotNull()
            .IsGreaterThan(-1)
            .IsLessThan(200);

        Describe(x => x.TrackName)
            .IsEqualTo("Definitely Not The Correct Song Name.");

        Describe(x => x.Duration)
            .IsEqualTo(
                -1,
                o => o
                    .WithErrorMessage("Song must have a negative duration.")
                    .WithDescription("Songs must force you to travel slowly backwards in time to listen to.")
            );

        Describe(x => x.Genre)
            .Vitally().IsNotEmpty()
            .IsLengthInclusivelyBetween(20, 400);
    }
}'
            ></CodeWindow>
      </template>

      <template #request>
        <CodeWindow
              language="csharp"
              source='public record BasicSong(
    string Artist,
    int? TrackNumber,
    string TrackName,
    double Duration,
    string Genre
);'
            ></CodeWindow>
      </template>

      <template #errorreport>
        <CodeWindow
              language="json"
              source='{
    "errors": {
        "nString": [
            "`NotNString` is an invalid option"
        ]
    }
}'
            ></CodeWindow>
      </template>
      <template #openapi>
        <CodeWindow
              language="json"
              source='"InputObject": {
    "type": "object",
    "properties": {
        "nString": {
            "type": "string",
            "nullable": true
        }
    },
    "additionalProperties": false,
    "x-validation": {
        "nString": [
            "Must be `NString`"
        ]
    }
}'
            ></CodeWindow>
      </template>
    </PanelsOrTabs>


  </v-container>
</template>