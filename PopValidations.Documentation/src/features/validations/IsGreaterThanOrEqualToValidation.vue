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
          <v-card-title><h3>Is Greater Than Or Equal To</h3></v-card-title>
          <v-card-text></v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <PanelsOrTabs>
      <template #code>
        <CodeWindow
              language="csharp"
              source='public class BasicSongValidator : AbstractValidator
{
    public BasicSongValidator()
    {
        Describe(x => x.TrackName)
            .IsGreaterThanOrEqualTo("Down With The Sickness");
        Describe(x => x.TrackNumber)
            .IsGreaterThanOrEqualTo(new ScopedData<double>(double.MaxValue));
        Describe(x => x.Duration)
            .IsGreaterThanOrEqualTo(new ScopedData<double>(2));
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
            "Is not greater than or equal to "1.7976931348623157E+308""
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
                    "validator": "IsGreaterThanOrEqualToValidation",
                    "message": "Must be greater than or equal to "Down With The Sickness"",
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
                    "validator": "IsGreaterThanOrEqualToValidation",
                    "message": "Must be greater than or equal to "1.7976931348623157E+308"",
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
                    "validator": "IsGreaterThanOrEqualToValidation",
                    "message": "Must be greater than or equal to "2"",
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

  </v-container>
</template>
