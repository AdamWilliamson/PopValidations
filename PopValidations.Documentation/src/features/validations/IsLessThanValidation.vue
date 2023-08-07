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
          <v-card-title><h3>Is Less Than</h3></v-card-title>
          <v-card-text></v-card-text>
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
        Describe(x => x.NInteger).IsLessThan(5);
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
            "Is not less than `5`"
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
            "maximum": 5,
            "exclusiveMinimum": true,
            "type": "integer",
            "format": "int32",
            "nullable": true
        }
    },
    "additionalProperties": false,
    "x-validation": {
        "nInteger": [
            "Must be less than `5`"
        ]
    }
}'
            ></CodeWindow>
      </template>
    </PanelsOrTabs>
    
  </v-container>
</template>
