import "prismjs";
import "prismjs/components/prism-csharp";

import { createApp } from "vue";
import App from "./App.vue";
import router from "./router";

import "./assets/main.scss";

// Vuetify
import "vuetify/styles";
import { createVuetify } from "vuetify";
import * as components from "vuetify/components";
import * as directives from "vuetify/directives";
import { aliases, mdi } from "vuetify/iconsets/mdi";

import "@mdi/font/css/materialdesignicons.css"; // Ensure you are using css-loader

const vuetify = createVuetify({
  components,
  directives,
  theme: {
    defaultTheme: "dark",
    themes: {
      dark: {
        dark: true,
        colors: {
          primary: "#9155fd", //"#A87CD7",//"#c792ea",
          secondary: "#AA77FF", //0x2196f3,
          accent: "#9c27b0",
          error: "#e91e63",
          warning: "#ff5722",
          info: "#009688",
          success: "#607d8b",
          "text-background": "#363636",
          // background: "#312d4b",
          // surface: "#28243d"
          surface: "#312d4b",
          background: "#28243d",
        },
      },
    },
  },
  icons: {
    defaultSet: "mdi",
    aliases,
    sets: {
      mdi,
    },
  },
});

import { defineAsyncComponent } from "vue";

createApp(App)
  .use(vuetify)
  .use(router)
  .component(
    "CodeWindow",
    defineAsyncComponent(() => import("@/components/CodeWindow.vue"))
  )
  .component(
    "PanelsOrTabs",
    defineAsyncComponent(() => import("@/components/PanelsOrTabs.vue"))
  )
  .mount("#app");