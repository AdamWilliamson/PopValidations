<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  components: {},
  data() {
    return {
      tab: "one",
    };
  },
});
</script>
<template>
 <v-container fluid bg-color="surface">
    <v-row>
      <v-col>
        <v-card>
          <v-card-title><h3>Is Null</h3></v-card-title>
          <v-card-text>Is Null works on any null compatible type, both nullable, and class types.  It will report an error if the field contains a null (not default, for struct types), and will add "required" to the OpenApi schema.</v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <v-row>
      <v-col>
        <v-tabs v-model="tab">
          <v-tab value="one">Validator</v-tab>
          <v-tab value="two">Error Report</v-tab>
          <v-tab value="three">OpenApi Schema</v-tab>
        </v-tabs>

        <v-window v-model="tab">
          <!-- bg-blue-grey-lighten-4 -->
          <v-window-item value="one" class="bg-darkgrey">
            <CodeWindow
              language="csharp"
              source='
public class BasicSongValidator : AbstractValidator
{
    public BasicSongValidator()
    {
        Describe(x => x.TrackNumber).NotNull();
        Describe(x => x.Genre)
          .Vitally().NotNull(options => 
            options
              .SetErrorMessage("Null is Invalid")
              .SetDescription("Nulls are bad.")
          );
    }
}'
            ></CodeWindow>
          </v-window-item>

          <v-window-item value="two">
            <CodeWindow
              language="json"
              source='{}'
            ></CodeWindow>
          </v-window-item>

          <v-window-item value="three">
            <CodeWindow
              language="json"
              source='{}'
            ></CodeWindow>
          </v-window-item>
        </v-window>
      </v-col>
    </v-row>

  </v-container>
</template>
