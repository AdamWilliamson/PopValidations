<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  components: {},
  data() {
    return {
      tab: "code",
      secondtab: "error"
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
                    <v-tab value="request">Object To Validate</v-tab>
                    <v-tab value="error">Error Report</v-tab>
                    <v-tab value="openapi">OpenApi Schema</v-tab>
                </v-tabs>

                <v-window v-model="tab">
                    <v-window-item value="code" class="bg-darkgrey">
                        <slot name="code"></slot>
                    </v-window-item>

                    <v-window-item value="request" class="bg-darkgrey">
                        <slot name="request"></slot>
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
                <v-tabs  v-model="tab">
                    <v-tab value="code">Validator</v-tab>
                    <v-tab value="request">Object To Validate</v-tab>
                </v-tabs>

                <v-window  v-model="tab">
                    <v-window-item value="code">
                        <slot name="code"></slot>
                    </v-window-item>
                    <v-window-item value="request">
                        <slot name="request"></slot>
                    </v-window-item>
                </v-window>
            </v-col>
            <v-col>
                <v-tabs v-model="secondtab">
                    <v-tab value="error">Error Report</v-tab>
                    <v-tab value="openapi">OpenApi Schema</v-tab>
                </v-tabs>

                <v-window v-model="secondtab">
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