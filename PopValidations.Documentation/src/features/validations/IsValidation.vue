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
          <v-card-title><h3>Is</h3></v-card-title>
          <v-card-text>The Is validation allows you to quickly and easily create your own validations.</v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <PanelsOrTabs>
      <template #code>
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
      </template>
      <template #errorreport>
        <CodeWindow
              language="json"
              source='{}'
            ></CodeWindow>
      </template>
      <template #openapi>
        <CodeWindow
              language="json"
              source='{}'
            ></CodeWindow>
      </template>
    </PanelsOrTabs>

  </v-container>
</template>
