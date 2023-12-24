<script setup lang="ts">
import { ref, onMounted } from "vue";
import { GetFiles } from "@/services/LoadValidationsService";

const Validator = ref("");
const Request = ref("");
const OpenApi = ref("");
const Validation = ref("");

onMounted(async () => {
  [Validator.value, Request.value, OpenApi.value, Validation.value] =
    await GetFiles("ForEach");
});
</script>

<template>
  <v-container fluid bg-color="surface">
    <v-row>
      <v-col>
        <v-card>
          <v-card-title><h3>ForEach</h3></v-card-title>
          <v-card-text>
            ForEach allows you to repeat a set of validations for each element
            in the list.  It can only be used after DescribeEnumerable, a version of Describe explicitly for ForEach.  DescribeEnumerable allows you to validate the list as a property of the object, and using ForEach, each element within that list.
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

    <v-row>
      <v-col>
        <v-card>
          <v-card-text
            >Contributed by Andrew Williamson.
            <v-btn href="https://github.com/AWilliamson88" flat link
              ><v-icon icon="mdi-github"></v-icon></v-btn
          ></v-card-text>
        </v-card>
      </v-col>
    </v-row>

  </v-container>
</template>
