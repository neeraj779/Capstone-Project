import React from "react";
import { BrowserRouter } from "react-router-dom";
import ReactDOM from "react-dom/client";
import App from "./App.jsx";
import "./index.css";

ReactDOM.createRoot(document.getElementById("root")).render(
  <React.StrictMode>
    <BrowserRouter>
      <main className="bg-gray-900 text-white min-h-screen">
        <App />
      </main>
    </BrowserRouter>
  </React.StrictMode>
);
