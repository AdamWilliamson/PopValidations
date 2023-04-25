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
          <v-card-title><h3>Is Greater Than</h3></v-card-title>
          <v-card-text>Is Greater Than works on any IComparable based types, note that it will not return an error if both fields are nulls. It will report an error if the fields are of different types or if only one of them is null.</v-card-text>
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
            .IsGreaterThan(
            new ScopedData<int>(int.MaxValue));
        Describe(x => x.Duration)
            .IsGreaterThan(
            new ScopedData<double>(3));
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
            "Is not greater than &#39;2147483647&#39;"
        ],
        "duration": [
            "Is not greater than &#39;3&#39;"
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
        "minimum": 2147483647,
        "exclusiveMinimum": true,
        "type": "integer",
        "format": "int32"
      },
      "duration": {
        "minimum": 3,
        "exclusiveMinimum": true,
        "type": "number",
        "format": "double"
      }
    },
    "additionalProperties": false,
    "x-validation": {
      "trackNumber": [
        "Must be greater than &#39;2147483647&#39;"
      ],
      "duration": [
        "Must be greater than &#39;3&#39;"
      ]
    }
  }
}'
            ></CodeWindow>
      </template>
    </PanelsOrTabs>
    
  </v-container>
</template>
