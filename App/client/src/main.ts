/**
 * main.ts
 *
 * Bootstraps Vuetify and other plugins then mounts the App`
 */

// Plugins
import { registerPlugins } from "@/plugins";
import { createAuth0 } from "@auth0/auth0-vue";

// Components
import App from "./App.vue";

// Composables
import { createApp } from "vue";

// Styles
import "unfonts.css";

const app = createApp(App);

app.use(
  createAuth0({
    domain: "dev-z9o0q5dh.eu.auth0.com",
    clientId: "QsPhZoHkhDNv6fS3izSfijpCrUM9Hk06",
    authorizationParams: {
      redirect_uri: window.location.origin,
      audience: "https://holocrononline.org",
    },
  })
);

registerPlugins(app);

app.mount("#app");
