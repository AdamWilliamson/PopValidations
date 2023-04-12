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
          <v-card-title><h3>Is Null</h3></v-card-title>
          <v-card-text>Is Null works on any null compatible type, both nullable, and class types.  It will report an error if the field contains a null (not default, for struct types), and will add "required" to the OpenApi schema.</v-card-text>
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
        Describe(x => x.TrackName).IsNull();
        Describe(x => x.Artist)
          .Vitally().IsNull(options => 
            options
            .WithErrorMessage("We don&#39;t like values")
              .WithDescription("Values are bad.")
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
            "Is not null."
        ],
        "artist": [
            "We don&#39;t like values"
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
          "null"
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
        "Must be null."
      ],
      "artist": [
        "Values are bad."
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
