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
          <v-card-title><h3>Is</h3></v-card-title>
          <v-card-text>The Is validation allows you to quickly and easily create your own validations.</v-card-text>
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
    Describe(x => x.TrackNumber)
    .Is(
      "{Interesting description here}", 
      "{Humorous error message here}", 
      IntTest
      );
    }

    public static bool IntTest(int? value)
    {
        return value == 1;
    }
}'
            ></CodeWindow>
      </template>
      <template #errorreport>
        <CodeWindow
              language="json"
              source='{
    "errors": {
        "trackNumber": [
            "{Humorous error message here}"
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
        "type": "string",
        "nullable": true
      },
      "artist": {
        "$ref": "#/components/schemas/Artist"
      },
      "trackNumber": {
        "type": "integer",
        "format": "int32",
        "nullable": true
      }
    },
    "additionalProperties": false,
    "x-aemo-validation": {
      "trackNumber": [
        "{Interesting description here}"
      ]
    }
  }
}'
            ></CodeWindow>
      </template>
    </PanelsOrTabs>

  </v-container>
</template>
