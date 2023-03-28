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
        Describe(x => x.TrackNumber).IsNull();
        Describe(x => x.Duration).IsNull();
        Describe(x => x.Genre)
          .Vitally().IsNull(options => 
            options
            .WithErrorMessage("We don`t like values")
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
        "duration": [
            "Is not null."
        ],
        "genre": [
            "We don`t like values"
        ]
    }
}'
            ></CodeWindow>
      </template>
      <template #openapi>
        <CodeWindow
              language="json"
              source='{
    "results": [
        {
            "property": "TrackName",
            "outcomes": [
                {
                    "validator": "IsNullValidation",
                    "message": "Must be null.",
                    "values": []
                }
            ],
            "validationGroups": []
        },
        {
            "property": "TrackNumber",
            "outcomes": [
                {
                    "validator": "IsNullValidation",
                    "message": "Must be null.",
                    "values": []
                }
            ],
            "validationGroups": []
        },
        {
            "property": "Duration",
            "outcomes": [
                {
                    "validator": "IsNullValidation",
                    "message": "Must be null.",
                    "values": []
                }
            ],
            "validationGroups": []
        },
        {
            "property": "Genre",
            "outcomes": [
                {
                    "validator": "IsNullValidation",
                    "message": "Values are bad.",
                    "values": []
                }
            ],
            "validationGroups": []
        }
    ]
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
