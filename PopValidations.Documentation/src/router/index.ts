import { createRouter, createWebHistory } from "vue-router";
import Home from "@/features/home/home.vue";

//== Geting Started
import Installation from "@/features/getting-started/installation.vue";

//== Demonstrations
import BasicDemonstration from "@/features/demonstrations/basic.vue";

//== Integrations
import SwashbuckleIntegration from "@/features/integrations/swashbuckle.vue";
import MediatRIntegration from "@/features/integrations/mediatr.vue";

//== Configurability
import OverridngOptions from "@/features/configurability/overridingoptions.vue";

//== Localisations
import AddingALanguage from "@/features/localisation/addingalanguage.vue";

//== Testability
import TestingExtensions from "@/features/testability/testingextensions.vue";

//== Validations
import IsNullValidation from "@/features/validations/IsNullValidation.vue";
import IsEmptyValidation from "@/features/validations/IsEmptyValidation.vue";
import IsEqualToValidation from "@/features/validations/IsEqualToValidation.vue";
import IsValidation from "@/features/validations/IsValidation.vue";
import IsLengthInclusivelyBetweenValidation from "@/features/validations/IsLengthInclusivelyBetweenValidation.vue";
import IsLengthExclusivelyBetweenValidation from "@/features/validations/IsLengthExclusivelyBetweenValidation.vue";

import IsGreaterThanValidation from "@/features/validations/IsGreaterThanValidation.vue";
import IsGreaterThanOrEqualToValidation from "@/features/validations/IsGreaterThanOrEqualToValidation.vue";
import IsLessThanValidation from "@/features/validations/IsLessThanValidation.vue";
import IsLessThanOrEqualToValidation from "@/features/validations/IsLessThanOrEqualToValidation.vue";
import IsNotNullValidation from "@/features/validations/IsNotNullValidation.vue";
import IsNotEmptyValidation from "@/features/validations/IsNotEmptyValidation.vue";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: "/",
      name: "Home",
      component: Home,
    },
    {
      path: "/getting-started/",
      redirect: "/",
      children: [
        {
          path: "/getting-started/installation",
          name: "Installation",
          component: Installation,
        },
      ],
    },
    {
      path: "/demonstrations",
      redirect: "/",
      children: [
        {
          path: "/demonstrations/basic",
          name: "BasicDemonstration",
          component: BasicDemonstration,
        },
      ],
    },
    {
      path: "/integration",
      redirect: "/",
      children: [
        {
          path: "/integration/swashbuckle",
          name: "SwashbuckleIntegration",
          component: SwashbuckleIntegration,
        },
        {
          path: "/integration/mediatr",
          name: "MediatRIntegration",
          component: MediatRIntegration,
        },
      ],
    },
    {
      path: "/configurability",
      redirect: "/",
      children: [
        {
          path: "/configurability/overriding-options",
          name: "OverridingOptions",
          component: OverridngOptions,
        },
      ],
    },
    {
      path: "/localisations",
      redirect: "/",
      children: [
        {
          path: "/localisations/add-a-language",
          name: "AddingALanguage",
          component: AddingALanguage,
        },
      ],
    },
    {
      path: "/testability",
      redirect: "/",
      children: [
        {
          path: "/testability/testing-extensions",
          name: "TestingExtensions",
          component: TestingExtensions,
        },
      ],
    },
    {
      path: "/validation/",
      redirect: "/",
      children: [
        {
          path: "/validation/isnull",
          name: "IsNullValidation",
          component: IsNullValidation,
        },
        {
          path: "/validation/isempty",
          name: "IsEmptyValidation",
          component: IsEmptyValidation,
        },
        {
          path: "/validation/isequalto",
          name: "IsEqualToValidation",
          component: IsEqualToValidation,
        },
        {
          path: "/validation/is",
          name: "IsValidation",
          component: IsValidation,
        },
        {
          path: "/validation/islengthinclusivelybetween",
          name: "IsLengthInclusivelyBetweenValidation",
          component: IsLengthInclusivelyBetweenValidation,
        },
        {
          path: "/validation/islengthexclusivelybetween",
          name: "IsLengthExclusivelyBetweenValidation",
          component: IsLengthExclusivelyBetweenValidation,
        },
        {
          path: "/validation/isgreaterthan",
          name: "IsGreaterThanValidation",
          component: IsGreaterThanValidation,
        },
        {
          path: "/validation/isgreaterthanorequalto",
          name: "IsGreaterThanOrEqualToValidation",
          component: IsGreaterThanOrEqualToValidation,
        },
        {
          path: "/validation/islessthan",
          name: "IsLessThanValidation",
          component: IsLessThanValidation,
        },
        {
          path: "/validation/islessthanorequalto",
          name: "IsLessThanOrEqualToValidation",
          component: IsLessThanOrEqualToValidation,
        },
        {
          path: "/validation/isnotnull",
          name: "IsNotNullValidation",
          component: IsNotNullValidation,
        },
        {
          path: "/validation/isnotempty",
          name: "IsNotEmptyValidation",
          component: IsNotEmptyValidation,
        }
      ]
    }
  ]
});

export default router;
