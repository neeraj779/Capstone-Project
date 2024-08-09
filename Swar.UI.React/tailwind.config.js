/** @type {import('tailwindcss').Config} */
export default {
  content: ["./index.html", "./src/**/*.{js,ts,jsx,tsx}"],
  theme: {
    extend: {
      minHeight: {
        "screen-minus-navbar": "calc(100vh - 72px)",
      },
    },
  },
  plugins: [],
};
