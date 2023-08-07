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
          <v-card-title><h3>ForEach</h3></v-card-title>
          <v-card-text>ForEach allows you to repeat a set of validations for each element in the list.</v-card-text>
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
        // If ArrayOfStrings is Not Null, then Check each item
        DescribeEnumerable(x => x.ArrayOfStrings)
            .Vitally().IsNotNull()
            .ForEach(x => x
                .Vitally().IsNotNull()
                .IsNotEmpty() // Only run if Not Null
            );

        // Tests each item, but stops at the first item that fails.
        DescribeEnumerable(x => x.ArrayOfStrings)
            .Vitally().ForEach(x => x
                .IsEqualTo("Test")
            );
    }
}'
            ></CodeWindow>
      </template>

      <template #request>
        <CodeWindow
              language="csharp"
              source='public record InputObject(List<string?> ArrayOfStrings);'
            ></CodeWindow>
      </template>

      <template #errorreport>
        <CodeWindow
              language="json"
              source='{
    "errors": {
        "nString": [
            "Is not empty"
        ]
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
            "maxLength": 0,
            "maxItems": 0,
            "type": "string",
            "nullable": true
        }
    },
    "additionalProperties": false,
    "x-validation": {
        "nString": [
            "Must be empty"
        ]
    }
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
