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
          <v-card-title><h3>Is Length Inclusively Between</h3></v-card-title>
          <v-card-text>
            Checks if the item is something that can be counted, and checks if the number of items is between 2 values, inclusive of those values.
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
            .IsLengthInclusivelyBetween(5, 10);
        Describe(x => x.TrackNumber)
            .IsLengthInclusivelyBetween(2, 5);
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
            "Is not between 5 and 10 inclusive."
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
                    "validator": "IsLengthInclusivelyBetweenValidation",
                    "message": "Must be between 5 and 10 inclusive.",
                    "values": [
                        {
                            "key": "startValue",
                            "value": "5"
                        },
                        {
                            "key": "endValue",
                            "value": "10"
                        }
                    ]
                },
                {
                    "validator": "IsLengthInclusivelyBetweenValidation",
                    "message": "Must be between 2 and 5 inclusive.",
                    "values": [
                        {
                            "key": "startValue",
                            "value": "2"
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
