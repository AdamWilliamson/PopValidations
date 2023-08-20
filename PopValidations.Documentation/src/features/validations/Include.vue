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
          <v-card-title><h3>Include</h3></v-card-title>
          <v-card-text>Breaking up a set of validations into reusable components, or into similar validations, can make managing your code easier. To allow you to do this, Include allows you to merge multiple validation classes into 1 set of validations.</v-card-text>
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
        Include(new SecondaryValidator());
    }
}

public class SecondaryValidator : AbstractSubValidator<InputObject>
{
    public SecondaryValidator()
    {
        Describe(x => x.NString)
            .IsNotNull();
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
            "Is null."
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
            "type": "string"
        }
    },
    "additionalProperties": false,
    "x-validation": {
        "nString": [
            "Must not be null."
        ]
    }
}'
            ></CodeWindow>
      </template>
    </PanelsOrTabs>

  </v-container>
</template>
