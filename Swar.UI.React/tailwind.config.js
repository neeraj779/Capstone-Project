const { nextui } = require("@nextui-org/react");

/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
    "./node_modules/@nextui-org/theme/dist/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      minHeight: {
        "screen-minus-navbar": "calc(100vh - 72px)",
      },
      colors: {
        "custom-dark": "#1a202c;",
      },
    },
  },
  darkMode: "class",
  plugins: [nextui()],
};
