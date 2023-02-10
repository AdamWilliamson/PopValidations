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
          <v-card-title><h3>Is Empty</h3></v-card-title>
          <v-card-text>Is Empty works on any null compatible type, both nullable, and class types, as well as any Array or IEnumerable based types.  It will report an error if the field does NOT contain a null (not default, for struct types), or the array or IEnumerable based item is NOT empty.</v-card-text>
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
        Describe(x => x.TrackNumber).IsEmpty();
        Describe(x => x.Genre)
          .Vitally().IsEmpty(options => 
            options
              .SetErrorMessage("Non Empty Fields are Invalid.")
              .SetDescription("This Field must be Empty.")
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

    <v-row>
      <v-col>
        <v-card>
          <v-card-text>Written by Andrew Williamson. <v-btn href="https://github.com/AWilliamson88" flat link><v-icon icon="mdi-github"></v-icon></v-btn></v-card-text>
        </v-card>
      </v-col>
    </v-row>

  </v-container>
</template>
