<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  data() {
    return {
    };
  },
});
</script>

<template>
  <v-app>
    <v-app-bar color="background">
      <v-app-bar-title>Pop Validations</v-app-bar-title>

      <template v-slot:append>
        <v-btn
          icon="mdi-github"
          href="https://github.com/AdamWilliamson/PopValidations"
        ></v-btn>
      </template>
    </v-app-bar>

    <v-navigation-drawer permanent density="comfortable" color="background">
      <v-list nav bg-color="background">
        <v-list-item
          title="Home"
          value="Home"
          :to="{ name: 'Home' }"
          link
          color="secondary"
        >
          <template v-slot:prepend>
            <v-icon>mdi-home</v-icon>
          </template>
        </v-list-item>

        <v-list-subheader color="secondary">Getting Started</v-list-subheader>
        <v-list-item
          color="secondary"
          title="Installation"
          value="Installation"
          :to="{ name: 'Installation' }"
          link
        ></v-list-item>

        <v-list-subheader color="secondary">Demonstrations</v-list-subheader>

        <v-list-item
          color="secondary"
          title="Basic"
          value="Basic"
          :to="{ name: 'BasicDemonstration' }"
          link
          density="compact"
        ></v-list-item>

        <v-list-subheader color="secondary">Integrations</v-list-subheader>
        <v-list-item
          color="secondary"
          title="OpenApi - Swashbuckle"
          value="OpenApi - Swashbuckle"
          :to="{ name: 'SwashbuckleIntegration' }"
          link
        ></v-list-item>
        <v-list-item
          color="secondary"
          title="CQRS - MediatR"
          value="CQRS - MediatR"
          :to="{ name: 'MediatRIntegration' }"
          link
        ></v-list-item>

        <v-list-subheader color="secondary">Configurability</v-list-subheader>
        <v-list-item
          color="secondary"
          title="Overridng Options"
          value="Overridng Options"
          :to="{ name: 'OverridingOptions' }"
          link
        ></v-list-item>

        <v-list-subheader color="secondary">Validations</v-list-subheader>
        <v-list-item
          color="secondary"
          title="Is Null"
          value="Is Null"
          :to="{ name: 'IsNullValidation' }"
          link
        ></v-list-item>

        <v-list-subheader color="secondary">Localisation</v-list-subheader>
        <v-list-item
          color="secondary"
          title="Adding A Language"
          value="Adding A Language"
          :to="{ name: 'AddingALanguage' }"
          link
        ></v-list-item>

        <v-list-subheader color="secondary">Testability</v-list-subheader>
        <v-list-item
          color="secondary"
          title="Testing Extensions"
          value="Testing Extensions"
          :to="{ name: 'TestingExtensions' }"
          link
        ></v-list-item>
      </v-list>
    </v-navigation-drawer>

    <v-main>
      <RouterView />
    </v-main>
  </v-app>
</template>
