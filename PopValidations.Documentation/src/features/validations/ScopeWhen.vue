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
          <v-card-title><h3>ScopeWhen</h3></v-card-title>
          <v-card-text>ScopeWhen combines The Scope's ability to get external values, and When's ability to allow you to restrict the validations that run, dependant on an incoming value. <br/>
          There are several varieties of ScopeWhen, for Tasked and non-Tasked values, and for if you need to use the Scope's value within the When check. <br />
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <PanelsOrTabs>
      <template #code>
        <CodeWindow
              language="csharp"
              source='public static class DataRetriever
    {
        public static Task<string> GetValue(Level1 v) 
        { 
            return Task.FromResult(v?.DependantField + " GetValue"); 
        }
        public static Task<string> GetMoreValue(Level1 v) 
        { 
            return Task.FromResult(v?.DependantField + " GetMoreValue"); 
        }
    }

    public class Validator : AbstractValidator<Level1>
    {
        public Validator()
        {
            ScopeWhen(
                "When Check is True 1",
                x => Task.FromResult(x.Check),
                "Database Value 1",
                (x) => DataRetriever.GetValue(x),
                (retrievedData) =>
                {
                    Describe(x => x.DependantField).IsEqualTo(retrievedData);
                }
            );

            ScopeWhen(
                "When Check is True 2",
                x => x.Check,
                "Database Value 2",
                (x) => (x?.DependantField ?? "null value") + " thing 1",
                (retrievedData) =>
                {
                    Describe(x => x.DependantField).IsEqualTo(retrievedData);
                }
            );

            ScopeWhen(
                "When Check is True 3",
                x => Task.FromResult(x.Check),
                "Database Value 3",
                (x) => DataRetriever.GetMoreValue(x),
                (moreData) =>
                {
                    Describe(x => x.DependantField).IsEqualTo(moreData);
                }
            );

            ScopeWhen(
                "When Check is True 4",
                x => x.Check,
                "Database Value 4",
                (x) => (x?.DependantField ?? "null value") + " thing 2",
                (moreData) =>
                {
                    Describe(x => x.DependantField).IsEqualTo(moreData);
                }
            );
        }
    }'
            ></CodeWindow>
      </template>

      <template #request>
        <CodeWindow
              language="csharp"
              source='public record Level1(bool Check, string? DependantField);'
            ></CodeWindow>
      </template>

      <template #errorreport>
        <CodeWindow
              language="json"
              source='{
    "errors": {
        "dependantField": [
            "Is not equal to `1 GetValue`.",
            "Is not equal to `1 thing 1`.",
            "Is not equal to `1 GetMoreValue`.",
            "Is not equal to `1 thing 2`."
        ]
    }
}'
            ></CodeWindow>
      </template>
      <template #openapi>
        <CodeWindow
              language="json"
              source='"Level1": {
    "type": "object",
    "properties": {
        "check": {
            "type": "boolean"
        },
        "dependantField": {
            "type": "string",
            "nullable": true
        }
    },
    "additionalProperties": false,
    "x-validation": {
        "dependantField": [
            "When Check is True 1 : Must equal to `Database Value 1`.",
            "When Check is True 2 : Must equal to `Database Value 2`.",
            "When Check is True 3 : Must equal to `Database Value 3`.",
            "When Check is True 4 : Must equal to `Database Value 4`."
        ]
    }
}'
            ></CodeWindow>
      </template>
    </PanelsOrTabs>

  </v-container>
</template>
