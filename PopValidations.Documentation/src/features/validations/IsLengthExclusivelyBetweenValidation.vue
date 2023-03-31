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
          <v-card-title><h3>Is Length Exclusively Between</h3></v-card-title>
          <v-card-text>
            Checks if the item is something that can be counted, and checks if the number of items is between 2 values, exclusive of those values.
          </v-card-text>
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
            .IsLengthExclusivelyBetween(4, 41);
        Describe(x => x.TrackNumber)
            .IsLengthExclusivelyBetween(0, 5);
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
            "Is not between 4 and 41 exclusive."
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
            "property": "TrackNumber",
            "outcomes": [
                {
                    "validator": "IsLengthExclusivelyBetweenValidation",
                    "message": "Must be between 4 and 41 exclusive.",
                    "values": [
                        {
                            "key": "startValue",
                            "value": "4"
                        },
                        {
                            "key": "endValue",
                            "value": "41"
                        }
                    ]
                },
                {
                    "validator": "IsLengthExclusivelyBetweenValidation",
                    "message": "Must be between 0 and 5 exclusive.",
                    "values": [
                        {
                            "key": "startValue",
                            "value": "0"
                        },
                        {
                            "key": "endValue",
                            "value": "5"
                        }
                    ]
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
