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
          <v-card-title><h3>Is Less Than Or Equal To</h3></v-card-title>
          <v-card-text></v-card-text>
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
        Describe(x => x.Duration)
            .IsLessThanOrEqualTo(new ScopedData<double>(2));
    }
}'
            ></CodeWindow>
      </template>
      <template #errorreport>
        <CodeWindow
              language="json"
              source='{
    "errors": {
        "duration": [
            "Is not less than or equal to &#39;2&#39;"
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
            "duration": {
              "maximum": 2,
              "exclusiveMaximum": false,
              "type": "number",
              "format": "double"
            }
        },
        "additionalProperties": false,
        "x-validation": {
            "duration": [
            "Must be less than or equal to &#39;2&#39;"
            ]
        }
    }
}'
            ></CodeWindow>
      </template>
    </PanelsOrTabs>

  </v-container>
</template>
