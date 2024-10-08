import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";
import { VitePWA } from "vite-plugin-pwa";

const manifestForPlugin = {
  registerType: "autoUpdate",
  includeAssets: ["favicon.ico", "apple-touch-icon.png"],
  manifest: {
    short_name: "Swar",
    name: "Swar",
    icons: [
      {
        src: "/icons-vector.svg",
        type: "image/svg+xml",
        sizes: "512x512",
      },
      {
        src: "/android-chrome-192x192.png",
        sizes: "192x192",
        type: "image/png",
      },
      {
        src: "/android-chrome-512x512.png",
        sizes: "512x512",
        type: "image/png",
      },
    ],
    screenshots: [
      {
        src: "/mockup-dark.png",
        sizes: "640x320",
        type: "image/png",
        form_factor: "wide",
        label: "swar UI",
      },
      {
        src: "/mockup-light.png",
        sizes: "640x320",
        type: "image/png",
        form_factor: "wide",
        label: "swar UI",
      },
      {
        src: "/mockup-dark.png",
        sizes: "640x320",
        type: "image/png",
        form_factor: "narrow",
        label: "swar UI",
      },
      {
        src: "/mockup-light.png",
        sizes: "640x320",
        type: "image/png",
        form_factor: "narrow",
        label: "swar UI",
      },
    ],
    description: "Swar | Your one-stop solution for amazing music experiences.",
    display: "standalone",
    theme_color: "#111828",
    background_color: "#111828",
    scope: "/",
    start_url: "/home",
    orientation: "portrait",
  },
};

export default defineConfig({
  plugins: [react(), VitePWA(manifestForPlugin)],
});
