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
          <v-card-title><h3>Vitally</h3></v-card-title>
          <v-card-text>Vitally, is a Validation modification. It ensures no validations run, if the validation right after it, fails. This ensures you can test an object for nullability, for example, and not run any validation or custom validations that may fail with less than useful error messages.</v-card-text>
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
            Describe(x => x.NString)
              .Vitally().IsNotEmpty()
              .IsEqualTo("Test");
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
            "Is empty."
        ]
    }
}'
            ></CodeWindow>
      </template>
      <template #openapi>
        <CodeWindow
              language="json"
              source='"InputObject": {
    "required": [
        "nString"
    ],
    "type": "object",
    "properties": {
        "nString": {
            "minLength": 1,
            "minItems": 1,
            "enum": [
                "Test"
            ],
            "type": "string",
            "nullable": true
        }
    },
    "additionalProperties": false,
    "x-validation": {
        "nString": [
            "Must not be empty.",
            "Must equal to `Test`"
        ]
    }
}'
            ></CodeWindow>
      </template>
    </PanelsOrTabs>

  </v-container>
</template>
