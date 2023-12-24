<script setup lang="ts">
import { ref, onMounted } from "vue";
import { GetDemoFiles } from "@/services/LoadValidationsService";

const Validator = ref("");
const Request = ref("");
const OpenApi = ref("");
const Validation = ref("");

onMounted(async () => {
  [Validator.value, Request.value, OpenApi.value, Validation.value] =
    await GetDemoFiles("Basic");
});
</script>

<template>
  <v-container fluid bg-color="surface">
    <v-row>
      <v-col>
        <v-card>
          <v-card-title><h3>Basic Demonstration</h3></v-card-title>
          <v-card-text>Just the basics, showing how to use the basics, so you can start quickly.</v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <PanelsOrTabs>
      <template #code>
        <CodeWindow v-if="Validator" language="csharp" :source="Validator" />
      </template>

      <template #request>
        <CodeWindow v-if="Request" language="csharp" :source="Request" />
      </template>

      <template #errorreport>
        <CodeWindow v-if="Validation" language="csharp" :source="Validation" />
      </template>
      <template #openapi>
        <CodeWindow v-if="OpenApi" language="csharp" :source="OpenApi" />
      </template>
    </PanelsOrTabs>

  </v-container>
</template>