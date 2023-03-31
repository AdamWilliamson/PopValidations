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
        Describe(x => x.TrackNumber).NotNull();
        Describe(x => x.Duration).NotNull();
        Describe(x => x.Genre)
          .Vitally().NotNull(options => 
            options
              .SetErrorMessage("Null is Invalid")
              .SetDescription("Nulls are bad.")
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
        "trackNumber": [
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
    "results": [
        {
            "property": "TrackName",
            "outcomes": [
                {
                    "validator": "NotNullValidation",
                    "message": "Must not be null.",
                    "values": []
                }
            ],
            "validationGroups": []
        },
        {
            "property": "TrackNumber",
            "outcomes": [
                {
                    "validator": "NotNullValidation",
                    "message": "Must not be null.",
                    "values": []
                }
            ],
            "validationGroups": []
        },
        {
            "property": "Duration",
            "outcomes": [
                {
                    "validator": "NotNullValidation",
                    "message": "Must not be null.",
                    "values": []
                }
            ],
            "validationGroups": []
        },
        {
            "property": "Genre",
            "outcomes": [
                {
                    "validator": "NotNullValidation",
                    "message": "Nulls are bad.",
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
          <v-card-text>Contributed by Adam Williamson. <v-btn href="https://github.com/AdamWilliamson" flat link><v-icon icon="mdi-github"></v-icon></v-btn></v-card-text>
        </v-card>
      </v-col>
    </v-row>
    
  </v-container>
</template>
