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
          <v-card-title><h3>Is Not Empty</h3></v-card-title>
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
        Describe(x => x.TrackName).IsNotEmpty();
        Describe(x => x.TrackNumber).IsNotEmpty();
        Describe(x => x.Genre)
          .Vitally().IsNotEmpty(options =>
            options
              .WithErrorMessage("Non Empty Fields are Invalid.")
              .WithDescription("This Field must be Empty.")
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
            "Is empty"
        ],
        "genre": [
            "Empty Fields are Invalid."
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
                    "validator": "IsNotEmptyValidation",
                    "message": "Must not be empty",
                    "values": []
                }
            ],
            "validationGroups": []
        },
        {
            "property": "TrackNumber",
            "outcomes": [
                {
                    "validator": "IsNotEmptyValidation",
                    "message": "Must not be empty",
                    "values": []
                }
            ],
            "validationGroups": []
        },
        {
            "property": "Genre",
            "outcomes": [
                {
                    "validator": "IsNotEmptyValidation",
                    "message": "This Field must not be Empty.",
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

  </v-container>
</template>
