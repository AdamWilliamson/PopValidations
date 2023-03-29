<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  components: {},
  data() {
    return {};
  },
});
</script>
<template>
 <v-container fluid bg-color="surface">
    <v-row>
      <v-col>
        <v-card>
          <v-card-title><h3>Is Empty</h3></v-card-title>
          <v-card-text>Is Empty works on any null compatible type, both nullable, and class types, as well as any Array or IEnumerable based types.  It will report an error if the field does NOT contain a null (not default, for struct types), or the array or IEnumerable based item is NOT empty.</v-card-text>
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
        Describe(x => x.TrackNumber).IsEmpty();
        Describe(x => x.Genre)
          .Vitally().IsEmpty(options => 
            options
              .SetErrorMessage("Non Empty Fields are Invalid.")
              .SetDescription("This Field must be Empty.")
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
            "Is not empty"
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
                    "validator": "IsEmptyValidation",
                    "message": "Must be empty",
                    "values": []
                }
            ],
            "validationGroups": []
        },
        {
            "property": "TrackNumber",
            "outcomes": [
                {
                    "validator": "IsEmptyValidation",
                    "message": "Must be empty",
                    "values": []
                }
            ],
            "validationGroups": []
        },
        {
            "property": "Genre",
            "outcomes": [
                {
                    "validator": "IsEmptyValidation",
                    "message": "This Field must be Empty.",
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
