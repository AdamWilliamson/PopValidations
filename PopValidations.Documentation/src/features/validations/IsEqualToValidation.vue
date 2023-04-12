<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  components: {},
  data() {
    return {
      tab: "one",
    };
  },
});
</script>
<template>
 <v-container fluid bg-color="surface">
    <v-row>
      <v-col>
        <v-card>
          <v-card-title><h3>Is Equal To</h3></v-card-title>
          <v-card-text>Is Equal To works on any value that implements the IComparable interface,  It compares the value resolved from the field, to the value supplied to the function. In the OpenApi schema, this value is reported as the ONLY Enum value.</v-card-text>
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
        Describe(x => x.TrackName)
            .IsEqualTo("Mind numbing boredom");
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
            "Is not equal to &#39;Mind numbing boredom&#39;"
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
        "enum": [
          "Mind numbing boredom"
        ],
        "type": "string",
        "nullable": true
      },
      "artist": {
        "$ref": "#/components/schemas/Artist"
      }
    },
    "additionalProperties": false,
    "x-aemo-validation": {
      "trackName": [
        "Must equal to &#39;Mind numbing boredom&#39;"
      ]
    }
  }
}'
            ></CodeWindow>
      </template>
    </PanelsOrTabs>

  </v-container>
</template>
