import { createRouter, createWebHistory,  createWebHashHistory } from "vue-router";
import Home from "@/features/home/home.vue";

//== Geting Started
import Installation from "@/features/getting-started/installation.vue";
import MainValidators from "@/features/getting-started/MainValidators.vue";

//== Demonstrations

import ModerateDemonstration from "@/features/demonstrations/moderate.vue";
import AdvancedDemonstration from "@/features/demonstrations/advanced.vue";

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
// import IsNullValidation from "@/features/validations/IsNullValidation.vue";
// import IsEmptyValidation from "@/features/validations/IsEmptyValidation.vue";
// import IsEqualToValidation from "@/features/validations/IsEqualToValidation.vue";
// import IsValidation from "@/features/validations/IsValidation.vue";
// import IsLengthInclusivelyBetweenValidation from "@/features/validations/IsLengthInclusivelyBetweenValidation.vue";
// import IsLengthExclusivelyBetweenValidation from "@/features/validations/IsLengthExclusivelyBetweenValidation.vue";
// import IsGreaterThanValidation from "@/features/validations/IsGreaterThanValidation.vue";
// import IsGreaterThanOrEqualToValidation from "@/features/validations/IsGreaterThanOrEqualToValidation.vue";
// import IsLessThanValidation from "@/features/validations/IsLessThanValidation.vue";
// import IsLessThanOrEqualToValidation from "@/features/validations/IsLessThanOrEqualToValidation.vue";
// import IsNotNullValidation from "@/features/validations/IsNotNullValidation.vue";
// import IsNotEmptyValidation from "@/features/validations/IsNotEmptyValidation.vue";
// import VitallyValidation from "@/features/validations/Vitally.vue";
// import SetValidator from "@/features/validations/SetValidator.vue";
// import Include from "@/features/validations/Include.vue";
// import When from "@/features/validations/When.vue";
// import Scope from "@/features/validations/Scope.vue";
// import ScopeWhen from "@/features/validations/ScopeWhen.vue";
// import ScopedData from "@/features/validations/ScopedData.vue";

const router = createRouter({
  history: createWebHashHistory(import.meta.env.BASE_URL),
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
        {
          path: "/getting-started/mainvalidators",
          name: "MainValidators",
          component: MainValidators,
        }
      ],
    },
    {
      path: "/demonstrations",
      redirect: "/",
      children: [
        {
          path: "/demonstrations/basic",
          name: "BasicDemonstration",
          component: () => import("@/features/demonstrations/basic.vue")
        },{
          path: "/demonstrations/moderate",
          name: "ModerateDemonstration",
          component: ModerateDemonstration,
        },{
          path: "/demonstrations/advanced",
          name: "AdvancedDemonstration",
          component: AdvancedDemonstration,
        }
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
          component: () => import("@/features/validations/IsNullValidation.vue"),
        },
        {
          path: "/validation/isempty",
          name: "IsEmptyValidation",
          component: () => import("@/features/validations/IsEmptyValidation.vue"),
        },
        {
          path: "/validation/isequalto",
          name: "IsEqualToValidation",
          component: () => import("@/features/validations/IsEqualToValidation.vue"),
        },
        {
          path: "/validation/is",
          name: "IsValidation",
          component: () => import("@/features/validations/IsValidation.vue"),
        },
        {
          path: "/validation/islengthinclusivelybetween",
          name: "IsLengthInclusivelyBetweenValidation",
          component: () => import("@/features/validations/IsLengthInclusivelyBetweenValidation.vue"),
        },
        {
          path: "/validation/islengthexclusivelybetween",
          name: "IsLengthExclusivelyBetweenValidation",
          component: () => import("@/features/validations/IsLengthExclusivelyBetweenValidation.vue"),
        },
        {
          path: "/validation/isgreaterthan",
          name: "IsGreaterThanValidation",
          component: () => import("@/features/validations/IsGreaterThanValidation.vue"),
        },
        {
          path: "/validation/isgreaterthanorequalto",
          name: "IsGreaterThanOrEqualToValidation",
          component: () => import("@/features/validations/IsGreaterThanOrEqualToValidation.vue"),
        },
        {
          path: "/validation/islessthan",
          name: "IsLessThanValidation",
          component: () => import("@/features/validations/IsLessThanValidation.vue"),
        },
        {
          path: "/validation/islessthanorequalto",
          name: "IsLessThanOrEqualToValidation",
          component: () => import("@/features/validations/IsLessThanOrEqualToValidation.vue"),
        },
        {
          path: "/validation/isnotnull",
          name: "IsNotNullValidation",
          component: () => import("@/features/validations/IsNotNullValidation.vue"),
        },
        {
          path: "/validation/isnotempty",
          name: "IsNotEmptyValidation",
          component: () => import("@/features/validations/IsNotEmptyValidation.vue"),
        },
        {
          path: "/validation/vitally",
          name:"VitallyValidation",
          component: () => import("@/features/validations/Vitally.vue")
        },
        {
          path:"/validation/setvalidator",
          name:"SetValidator",
          component: () => import("@/features/validations/SetValidator.vue")
        },
        {
          path:"/validation/include",
          name:"Include",
          component: () => import("@/features/validations/Include.vue")
        },
        {
          path:"/validation/when",
          name:"When",
          component: () => import("@/features/validations/When.vue")
        },
        {
          path:"/validation/scope",
          name:"Scope",
          component: () => import("@/features/validations/Scope.vue")
        },
        {
          path:"/validation/scopewhen",
          name:"ScopeWhen",
          component: () => import("@/features/validations/ScopeWhen.vue")
        },
        {
          path:"/validation/scopeddata",
          name:"ScopedData",
          component: () => import("@/features/validations/ScopedData.vue")
        },
        {
          path:"/validation/foreach",
          name:"ForEach",
          component: () => import("@/features/validations/ForEach.vue")
        }
      ]
    }
  ]
});

export default router;
