<script setup lang="ts">
import { onMounted, ref } from "vue";
import { GetFiles } from "@/services/LoadValidationsService";

const Validator = ref("");
const Request = ref("");
const OpenApi = ref("");
const Validation = ref("");

onMounted(async () => {
  [Validator.value, Request.value, OpenApi.value, Validation.value] =
    await GetFiles("Switch");
});
</script>

<template>
 <v-container fluid bg-color="surface">
    <v-row>
      <v-col>
        <v-card>
          <v-card-title><h3>Switch</h3></v-card-title>
          <v-card-text>Switch allows you to construct complex validations, that maintain the ability to describe them for OpenApi etc.
          <br />
          When using the built-in validations, it is possible to describe many things, in a very easy to read format. However in circumstances where the needs of validation are complex, or the existing easy to read format would create for a very large and complicated setup, the Switch, can help.
          <br />
          Switch comes with 2 functions that can be chained together.  Ignore, and Case.  And provides to them access to the value you wish to retrieve in the switch. This is useful for constructing objects that express your validation requirements, or retrieving database values, and using them without the normal ScopedData wrapper.
          <br />
          Ignore is purely decorative, and enables you to describe validations you have considered, but don't want to fail.  For example, a complex validation object, may have 10 unique combinations, where only 8 combinations should result in an error. In this case, you can safely note the remaining 2.
          <br />
          Case works with switch, in a way similar to validations within a Scope, without the ScopedData requirement. You provide it with a field you wish the error to be output to and use in your validation, the description for OpenApi etc, the test function that takes both the field value, and your Switch scoped data, and finally the error that is output.
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
