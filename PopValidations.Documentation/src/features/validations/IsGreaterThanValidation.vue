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
          <v-card-text>Is Greater Than works on any IEnumerable based types, note that it will not return an error if both fields are nulls. It will report an error if the fields are of different types or if only one of them is null.</v-card-text>
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
            .IsGreaterThan("Down With The Sickness");
        Describe(x => x.TrackNumber)
            .IsGreaterThan(new ScopedData<double>(double.MaxValue));
        Describe(x => x.Duration)
            .IsGreaterThan(new ScopedData<double>(2));
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
            "Is not greater than "Down With The Sickness""
        ],
        "trackNumber": [
            "Is not greater than "1.7976931348623157E+308""
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
                    "validator": "IsGreaterThanValidation",
                    "message": "Must be greater than "Down With The Sickness"",
                    "values": [
                        {
                            "key": "value",
                            "value": "Down With The Sickness"
                        }
                    ]
                }
            ],
            "validationGroups": []
        },
        {
            "property": "TrackNumber",
            "outcomes": [
                {
                    "validator": "IsGreaterThanValidation",
                    "message": "Must be greater than "1.7976931348623157E+308"",
                    "values": [
                        {
                            "key": "value",
                            "value": "1.7976931348623157E+308"
                        }
                    ]
                }
            ],
            "validationGroups": []
        },
        {
            "property": "Duration",
            "outcomes": [
                {
                    "validator": "IsGreaterThanValidation",
                    "message": "Must be greater than "2"",
                    "values": [
                        {
                            "key": "value",
                            "value": "2"
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
