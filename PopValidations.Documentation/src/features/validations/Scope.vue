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
          <v-card-title><h3>Scope</h3></v-card-title>
          <v-card-text>Scopes allow you to do tasked or non-tasked operations to retrieve extra data or do additional processes, to help test your objects validity. <br/>
          There are multiple versions of Scope, that allow you to achieve a variety of goals.  Scopes make no changes to values, or tests, but allow you to retrieve values, and describe those values.
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <PanelsOrTabs>
      <template #code>
        <CodeWindow
              language="csharp"
              source='    public static class DataRetriever
    {
        public static Task<string> GetValue() { return Task.FromResult("teststring"); }
        public static Task<string> GetMoreValue(InputObject obj) { return Task.FromResult(obj?.Field + " teststring2"); }
    }

    public class Validator : AbstractValidator<InputObject>
    {
        public Validator()
        {
            Scope(
                "Database Value",
                () => DataRetriever.GetValue(),
                (retrievedData) =>
                {
                    Describe(x => x.Field).IsEqualTo(retrievedData);    
                }
            );

            Scope(
                "Second Database Value",
                (validationObject) => DataRetriever.GetMoreValue(validationObject),
                (moreData) =>
                {
                    Describe(x => x.Field).IsEqualTo(moreData);
                }
            );

            Scope(
                "Third Database Value",
                (validationobject) =>  (validationobject?.Field ?? "") + " additional value",
                (moreData) =>
                {
                    Describe(x => x.Field).IsEqualTo(moreData);
                }
            );
        }
    }'
            ></CodeWindow>
      </template>

      <template #request>
        <CodeWindow
              language="csharp"
              source='public record InputObject(string? Field);'
            ></CodeWindow>
      </template>

      <template #errorreport>
        <CodeWindow
              language="json"
              source='{
    "errors": {
        "field": [
            "Is not equal to `teststring`.",
            "Is not equal to `a value teststring2`.",
            "Is not equal to `a value additional value`."
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
        "field": {
            "enum": [
                "Third Database Value"
            ],
            "type": "string",
            "nullable": true
        }
    },
    "additionalProperties": false,
    "x-validation": {
        "field": [
            "Must equal to `Database Value`.",
            "Must equal to `Second Database Value`.",
            "Must equal to `Third Database Value`."
        ]
    }
}'
            ></CodeWindow>
      </template>
    </PanelsOrTabs>

  </v-container>
</template>
