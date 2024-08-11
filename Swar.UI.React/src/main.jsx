import React from "react";
import ReactDOM from "react-dom/client";
import { NextUIProvider } from "@nextui-org/react";
import App from "./App.jsx";
import "./index.css";

ReactDOM.createRoot(document.getElementById("root")).render(
  <React.StrictMode>
    <NextUIProvider>
      <main className="bg-gray-900 text-white min-h-screen">
        <App />
      </main>
    </NextUIProvider>
  </React.StrictMode>
);
