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
import IsNullValidationVue from "@/features/validations/IsNullValidation.vue";

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
      path:"/integration",
      redirect:"/",
      children:[
        {
          path:"/integration/swashbuckle",
          name:"SwashbuckleIntegration",
          component: SwashbuckleIntegration
        },
        {
          path:"/integration/mediatr",
          name:"MediatRIntegration",
          component: MediatRIntegration
        }
      ]
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
          component: IsNullValidationVue,
        },
      ],
    },
  ],
});

export default router;
