<script setup lang="ts">
import { onMounted, ref } from "vue";
import { GetFiles } from "@/services/LoadValidationsService";

const Validator = ref("");
const Request = ref("");
const OpenApi = ref("");
const Validation = ref("");

onMounted(async () => {
  [Validator.value, Request.value, OpenApi.value, Validation.value] =
    await GetFiles("ScopedData");
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
