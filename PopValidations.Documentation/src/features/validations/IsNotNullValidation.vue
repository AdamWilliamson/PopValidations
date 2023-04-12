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
          <v-card-title><h3>Is Not Null</h3></v-card-title>
          <v-card-text>Is Not Null works on any null compatible type, both nullable, and class types.  It will report an error if the field contains a null.</v-card-text>
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
        Describe(x => x.TrackName).NotNull();
        Describe(x => x.TrackNumber)
            .NotNull(options =>
                options
                    .WithErrorMessage("Null is Invalid")
                    .WithDescription("Nulls are bad.")
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
            "Is null."
        ],
        "trackNumber": [
            "Null is Invalid"
        ],
    }
}'
            ></CodeWindow>
      </template>
      <template #openapi>
        <CodeWindow
              language="json"
              source='{
  "Song": {
    "required": [
      "trackName",
      "trackNumber"
    ],
    "type": "object",
    "properties": {
      "trackName": {
        "type": "string"
      },
      "artist": {
        "$ref": "#/components/schemas/Artist"
      },
      "trackNumber": {
        "type": "integer",
        "format": "int32"
      }
    },
    "additionalProperties": false,
    "x-aemo-validation": {
      "trackName": [
        "Must not be null."
      ],
      "trackNumber": [
        "Nulls are bad."
      ]
    }
  }
}'
            ></CodeWindow>
      </template>
    </PanelsOrTabs>

  </v-container>
</template>
