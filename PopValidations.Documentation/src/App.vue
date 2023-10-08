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
        <v-list-item
          color="secondary"
          title="Main Validators"
          value="MainValidators"
          :to="{ name: 'MainValidators' }"
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

        <v-list-item
          color="secondary"
          title="Moderate"
          value="Moderate"
          :to="{ name: 'ModerateDemonstration' }"
          link
          density="compact"
        ></v-list-item>

        <v-list-item
          color="secondary"
          title="Advanced"
          value="Advanced"
          :to="{ name: 'AdvancedDemonstration' }"
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
        <v-list-item
          color="secondary"
          title="Is Not Null"
          value="Is Not Null"
          :to="{ name: 'IsNotNullValidation' }"
          link
        ></v-list-item>
        <v-list-item
          color="secondary"
          title="Is Empty"
          value="Is Empty"
          :to="{ name: 'IsEmptyValidation' }"
          link
        ></v-list-item>
        <v-list-item
          color="secondary"
          title="Is Not Empty"
          value="Is Not Empty"
          :to="{ name: 'IsNotEmptyValidation' }"
          link
        ></v-list-item>
        <v-list-item
          color="secondary"
          title="Is Equal To"
          value="Is Equal To"
          :to="{ name: 'IsEqualToValidation' }"
          link
        ></v-list-item>
        <v-list-item
          color="secondary"
          title="Is Greater Than"
          value="Is Greater Than"
          :to="{ name: 'IsGreaterThanValidation' }"
          link
        ></v-list-item>
        <v-list-item
          color="secondary"
          title="Is Greater Than Or Equal To"
          value="Is Greater Than Or Equal To"
          :to="{ name: 'IsGreaterThanOrEqualToValidation' }"
          link
        ></v-list-item>
        <v-list-item
          color="secondary"
          title="Is Less Than"
          value="Is Less Than"
          :to="{ name: 'IsLessThanValidation' }"
          link
        ></v-list-item>
        <v-list-item
          color="secondary"
          title="Is Less Than Or Equal To"
          value="Is Less Than Or Equal To"
          :to="{ name: 'IsLessThanOrEqualToValidation' }"
          link
        ></v-list-item>
        <v-list-item
          color="secondary"
          title="Is Length Inclusively Between"
          value="Is Length Inclusively Between"
          :to="{ name: 'IsLengthInclusivelyBetweenValidation' }"
          link
        ></v-list-item>
        <v-list-item
          color="secondary"
          title="Is Length Exclusively Between"
          value="Is Length Exclusively Between"
          :to="{ name: 'IsLengthExclusivelyBetweenValidation' }"
          link
        ></v-list-item>
        <v-list-item
          color="secondary"
          title="Is"
          value="Is"
          :to="{ name: 'IsValidation' }"
          link
        ></v-list-item>

        <v-list-item
          color="secondary"
          title="Vitally"
          value="Vitally"
          :to="{ name: 'VitallyValidation' }"
          link
        ></v-list-item>

        <v-list-item
          color="secondary"
          title="SetValidation"
          value="SetValidation"
          :to="{ name: 'SetValidation' }"
          link
        ></v-list-item>

        <v-list-item
          color="secondary"
          title="Include"
          value="Include"
          :to="{ name: 'Include' }"
          link
        ></v-list-item>

        <v-list-item
          color="secondary"
          title="When"
          value="When"
          :to="{ name: 'When' }"
          link
        ></v-list-item>

        <v-list-item
          color="secondary"
          title="Scope"
          value="Scope"
          :to="{ name: 'Scope' }"
          link
        ></v-list-item>

        <v-list-item
          color="secondary"
          title="ScopeWhen"
          value="ScopeWhen"
          :to="{ name: 'ScopeWhen' }"
          link
        ></v-list-item>

        <v-list-item
          color="secondary"
          title="ScopedData"
          value="ScopedData"
          :to="{ name: 'ScopedData' }"
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
