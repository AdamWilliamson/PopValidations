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
          <v-card-title><h3>SetValidation</h3></v-card-title>
          <v-card-text>Breaking up objects into parent and children can help make sense of your data.  SetValidation allows you to import a validator for a subobject.</v-card-text>
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
        Describe(x => x.Child)
            .SetValidator(new ChildValidator());
    }
}

public class ChildValidator : AbstractSubValidator<ChildInputObject>
{
    public ChildValidator()
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
              source='    public record InputObject(ChildInputObject? Child);
public record ChildInputObject(string? NString);'
            ></CodeWindow>
      </template>

      <template #errorreport>
        <CodeWindow
              language="json"
              source='{
    "errors": {
        "child.NString": [
            "Is null."
        ]
    }
}'
            ></CodeWindow>
      </template>
      <template #openapi>
        <CodeWindow
              language="json"
              source='"ChildInputObject": {
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
