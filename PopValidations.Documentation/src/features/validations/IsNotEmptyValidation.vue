<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  components: {},
  data() {
    return {
    };
  },
});
</script>
<template>
 <v-container fluid bg-color="surface">
    <v-row>
      <v-col>
        <v-card>
          <v-card-title><h3>Is Not Empty</h3></v-card-title>
          <v-card-text>Is Not Empty works on any null compatible type, both nullable, and class types, as well as any Array or IEnumerable based types.  It will report an error if the field contains a null, or the array or IEnumerable based item is empty.</v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <PanelsOrTabs>
      <template #code>
        <CodeWindow
              language="csharp"
              source='
public class BasicSongValidator : AbstractValidator
{
    public BasicSongValidator()
    {
        Describe(x => x.TrackName).IsNotEmpty();
        Describe(x => x.TrackNumber).IsNotEmpty();
        Describe(x => x.Artist)
          .Vitally().IsNotEmpty(options =>
            options
              .WithErrorMessage("Non Empty Fields are Invalid.")
              .WithDescription("This Field must be Empty.")
          );
    }
}'
            ></CodeWindow>
      </template>
      <template #errorreport>
        <CodeWindow
              language="json"
              source='{
    "errors": {
        "trackName": [
            "Is empty"
        ],
        "trackNumber": [
            "Is empty"
        ],
        "artist": [
            "Empty Fields are Invalid."
        ]
    }
}'
            ></CodeWindow>
      </template>
      <template #openapi>
        <CodeWindow
              language="json"
              source='{
  "Song": {
    "type": "object",
    "properties": {
      "trackName": {
        "minLength": 1,
        "minItems": 1,
        "type": "string",
        "nullable": true
      },
      "artist": {
        "$ref": "#/components/schemas/Artist"
      },
      "trackNumber": {
        "minLength": 1,
        "minItems": 1,
        "type": "integer",
        "format": "int32"
      }
    },
    "additionalProperties": false,
    "x-aemo-validation": {
      "trackName": [
        "Must not be empty"
      ],
      "artist": [
        "This Field must not be Empty."
      ],
      "trackNumber": [
        "Must not be empty"
      ]
    }
  }
}'
            ></CodeWindow>
      </template>
    </PanelsOrTabs>

    <v-row>
      <v-col>
        <v-card>
          <v-card-text>Contributed by Andrew Williamson. <v-btn href="https://github.com/AWilliamson88" flat link><v-icon icon="mdi-github"></v-icon></v-btn></v-card-text>
        </v-card>
      </v-col>
    </v-row>

  </v-container>
</template>
