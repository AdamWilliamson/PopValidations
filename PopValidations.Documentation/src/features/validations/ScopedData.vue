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
          <v-card-title><h3>ScopedData</h3></v-card-title>
          <v-card-text>ScopeData is returned from Scope calls, allowing values retreived from services to be used in comparrisons, without executing those values when describing those values. <br />
          ScopedData items, can be passed into all validators, and are executed when validating.<br />
          ScopedData have functions to allow you to access internal values of complex objects, without executing values until Validation.  Each function includes the ability to describe the subsequent value, for anything that uses those descriptions.
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <v-row>
        <v-col>
            <CodeWindow
              language="csharp"
              source='
              public record InputObject(string? Field);

public record ReturnedObject(string TestValue1, string TestValue2);

public static class DataRetriever
{
    public static Task<ReturnedObject> GetValue() { return Task.FromResult(new ReturnedObject("Test 1",  "Test 2")); }
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
                Describe(x => x.Field)
                    .IsEqualTo(retrievedData.To("Is the same as the database value", x => x.TestValue1));

                Describe(x => x.Field)
                    .IsEqualTo(retrievedData.To("Is other value", x => x.TestValue2));
            }
        );
    }
}'
            ></CodeWindow>
        </v-col>
    </v-row>

  </v-container>
</template>
