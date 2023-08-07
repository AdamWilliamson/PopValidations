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
  public class Validator : AbstractValidator<InputObject>
  {
      public Validator()
      {
          Describe(x => x.NString).IsLengthExclusivelyBetween(1, 5);
          Describe(x => x.Array).IsLengthExclusivelyBetween(1, 5);
      }
  }'
            ></CodeWindow>
      </template>

      <template #request>
        <CodeWindow
              language="csharp"
              source='public record InputObject(string? NString, List<int> Array);'
            ></CodeWindow>
      </template>

      <template #errorreport>
        <CodeWindow
              language="json"
              source='{
    "errors": {
        "nString": [
            "Is not between 1 and 5 inclusive."
        ],
        "array": [
            "Is not between 1 and 5 inclusive."
        ]
    }
}'
            ></CodeWindow>
      </template>
      <template #openapi>
        <CodeWindow
              language="json"
              source='"InputObject": {
    "type": "object",
    "properties": {
        "nString": {
            "maxLength": 5,
            "minLength": 1,
            "type": "string",
            "nullable": true
        },
        "array": {
            "maxLength": 5,
            "minLength": 1,
            "type": "array",
            "items": {
                "type": "integer",
                "format": "int32"
            },
            "nullable": true
        }
    },
    "additionalProperties": false,
    "x-validation": {
        "nString": [
            "Must be between 1 and 5 inclusive."
        ],
        "array": [
            "Must be between 1 and 5 inclusive."
        ]
    }
}'
            ></CodeWindow>
      </template>
    </PanelsOrTabs>

  </v-container>
</template>
