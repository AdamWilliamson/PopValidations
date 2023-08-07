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
          <v-card-text>Is Greater Than works on any IComparable based types, note that it will not return an error if both fields are nulls. It will report an error if the fields are of different types or if only one of them is null.</v-card-text>
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
        Describe(x => x.NInteger).IsGreaterThan(5);
    }
}'
            ></CodeWindow>
      </template>

      <template #request>
        <CodeWindow
              language="csharp"
              source='public record InputObject(int? NInteger);'
            ></CodeWindow>
      </template>

      <template #errorreport>
        <CodeWindow
              language="json"
              source='{
    "errors": {
        "nInteger": [
            "Is not greater than `5`"
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
        "nInteger": {
            "minimum": 5,
            "exclusiveMinimum": true,
            "type": "integer",
            "format": "int32",
            "nullable": true
        }
    },
    "additionalProperties": false,
    "x-validation": {
        "nInteger": [
            "Must be greater than `5`"
        ]
    }
}'
            ></CodeWindow>
      </template>
    </PanelsOrTabs>
    
  </v-container>
</template>
