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
          <v-card-title><h3>Is</h3></v-card-title>
          <v-card-text>The Is validation allows you to quickly and easily create your own validations.</v-card-text>
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
        Describe(x => x.NString).Is(
            "Must be `NString`",
            "`{{value}}` is an invalid option",
            i => i == "NString"
        );
    }
}'
            ></CodeWindow>
      </template>

      <template #request>
        <CodeWindow
              language="csharp"
              source='public record InputObject(string? NString);'
            ></CodeWindow>
      </template>

      <template #errorreport>
        <CodeWindow
              language="json"
              source='{
    "errors": {
        "nString": [
            "`NotNString` is an invalid option"
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
            "type": "string",
            "nullable": true
        }
    },
    "additionalProperties": false,
    "x-validation": {
        "nString": [
            "Must be `NString`"
        ]
    }
}'
            ></CodeWindow>
      </template>
    </PanelsOrTabs>

  </v-container>
</template>
