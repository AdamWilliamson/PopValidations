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
          <v-card-title><h3>Is Equal To</h3></v-card-title>
          <v-card-text>Is Equal To works on any value that implements the IComparable interface,  It compares the value resolved from the field, to the value supplied to the function. In the OpenApi schema, this value is reported as the ONLY Enum value.</v-card-text>
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
    public static bool IntTest(int? value)
    {
        return value == 1;
    }

    public BasicSongValidator()
    {
        Describe(x => x.TrackNumber).Is("Description", "Error", IntTest);
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
          <v-card-text>Written by Adam Williamson. <v-btn href="https://github.com/AdamWilliamson" flat link><v-icon icon="mdi-github"></v-icon></v-btn></v-card-text>
        </v-card>
      </v-col>
    </v-row>

  </v-container>
</template>
