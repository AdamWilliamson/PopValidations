<script setup lang="ts">
import { onMounted, ref } from "vue";
import { GetFiles } from "@/services/LoadValidationsService";

const Validator = ref("");
const Request = ref("");
const OpenApi = ref("");
const Validation = ref("");

onMounted(async () => {
  [Validator.value, Request.value, OpenApi.value, Validation.value] =
    await GetFiles("Vitally");
});
</script>

<template>
 <v-container fluid bg-color="surface">
    <v-row>
      <v-col>
        <v-card>
          <v-card-title><h3>Vitally</h3></v-card-title>
          <v-card-text>Vitally, is a Validation modification. It ensures no validations run, if the validation right after it, fails. This ensures you can test an object for nullability, for example, and not run any validation or custom validations that may fail with less than useful error messages.</v-card-text>
          <v-card-text>AllNextAreVital, is also a Validation modification. It ensures that the next validation that fails, stops any future ones running. It is the equivalent of putitng Vitally(), before each validation. It cannot be turned off once done, and applies only to the current instance of Describe().</v-card-text>          
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
