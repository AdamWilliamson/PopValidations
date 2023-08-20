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
          <v-card-title><h3>When</h3></v-card-title>
          <v-card-text>When allows you to restrict the validations that run, dependant on an incoming value, or a value gained from an external service. <br/>
          In order to use this filter, you must supply a description of the check used, a function that does the check, and the validators you wish to have the limitation applied.
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <PanelsOrTabs>
      <template #code>
        <CodeWindow
              language="csharp"
              source='public class Validator : AbstractValidator<InputObject>
    {
        public Validator()
        {
            When(
                "When Check is True",
                x => Task.FromResult(x.Check == true),
                () =>
                {
                    When(
                        "When 10 == 10",
                        x => Task.FromResult(true),
                        () =>
                        {
                            Describe(x => x.DependantField).IsEqualTo("Test1");
                        }
                    );

                    Describe(x => x.DependantField).Vitally().IsNotEmpty();

                    When(
                        "When 5 == 5",
                        x => Task.FromResult(true),
                        () =>
                        {
                            Describe(x => x.DependantField).IsEqualTo("Test2");
                        }
                    );
                }
            );

            Describe(x => x.DependantField).IsEqualTo("Test3");
        }
    }'
            ></CodeWindow>
      </template>

      <template #request>
        <CodeWindow
              language="csharp"
              source='public record InputObject(bool Check, string? DependantField);'
            ></CodeWindow>
      </template>

      <template #errorreport>
        <CodeWindow
              language="json"
              source='{
    "errors": {
        "dependantField": [
            "Is not equal to `Test1`.",
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
    "type": "object",
    "properties": {
        "check": {
            "type": "boolean"
        },
        "dependantField": {
            "enum": [
                "Test3"
            ],
            "type": "string",
            "nullable": true
        }
    },
    "additionalProperties": false,
    "x-validation": {
        "dependantField": [
            "Must equal to `Test3`.",
            "When Check is True : Must not be empty.",
            "When Check is True & When 10 == 10 : Must equal to `Test1``.",
            "When Check is True & When 5 == 5 : Must equal to `Test2`."
        ]
    }
}'
            ></CodeWindow>
      </template>
    </PanelsOrTabs>

  </v-container>
</template>
