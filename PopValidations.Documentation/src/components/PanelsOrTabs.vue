<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  components: {},
  data() {
    return {
      tab: "code",
    };
  },
});
</script>
<template>
    <div fluid class="panel">
        <v-row class="d-sm-flex d-xl-none">
            <v-col>
                <v-tabs v-model="tab">
                    <v-tab value="code">Validator</v-tab>
                    <v-tab value="error">Error Report</v-tab>
                    <v-tab value="openapi">OpenApi Schema</v-tab>
                </v-tabs>

                <v-window v-model="tab">
                    <v-window-item value="code" class="bg-darkgrey">
                        <slot name="code"></slot>
                    </v-window-item>

                    <v-window-item value="error">
                        <slot name="errorreport"></slot>
                    </v-window-item>

                    <v-window-item value="openapi">
                        <slot name="openapi"></slot>
                    </v-window-item>
                </v-window>
            </v-col>
        </v-row>

        <v-row class="d-xl-flex">
            <v-col>
                <v-tabs>
                    <v-tab value="error">Code</v-tab>
                </v-tabs>

                <v-window>
                    <v-window-item >
                        <slot name="code"></slot>
                    </v-window-item>
                </v-window>
            </v-col>
            <v-col>
                <v-tabs v-model="tab">
                    <v-tab value="error">Error Report</v-tab>
                    <v-tab value="openapi">OpenApi Schema</v-tab>
                </v-tabs>

                <v-window v-model="tab">
                    <v-window-item value="error">
                        <slot name="errorreport"></slot>
                    </v-window-item>

                    <v-window-item value="openapi">
                        <slot name="openapi"></slot>
                    </v-window-item>
                </v-window>
            </v-col>
        </v-row>

</div>
</template>