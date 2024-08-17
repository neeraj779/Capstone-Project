import { useNavigate, Route, Routes, Navigate } from "react-router-dom";
import { NextUIProvider } from "@nextui-org/react";
import { SkeletonTheme } from "react-loading-skeleton";
import { Toaster } from "react-hot-toast";

import Layout from "./components/Layout";
import Home from "./pages/Home";
import Player from "./pages/Player";
import Search from "./pages/Search";
import Login from "./pages/Login";
import Register from "./pages/Register";
import Profile from "./pages/Profile";
import Library from "./pages/Library";
import Playlist from "./pages/Playlist";

import { PlayerProvider } from "./contexts/PlayerContext";
import ProtectedRoute from "./routes/ProtectedRoute";

const App = () => {
  const navigate = useNavigate();

  return (
    <NextUIProvider navigate={navigate}>
      <SkeletonTheme baseColor="#6B7280" highlightColor="#4B5563">
        <PlayerProvider>
          <Layout>
            <Routes>
              <Route path="/login" element={<Login />} />
              <Route path="/register" element={<Register />} />
              <Route element={<ProtectedRoute />}>
                <Route path="/" element={<Home />} />
                <Route path="/song/:id" element={<Player />} />
                <Route path="/search" element={<Search />} />
                <Route path="/profile" element={<Profile />} />
                <Route path="/library" element={<Library />} />
                <Route path="/playlist/:id" element={<Playlist />} />
              </Route>
              <Route path="*" element={<Navigate to="/" />} />
            </Routes>
            <Toaster />
          </Layout>
        </PlayerProvider>
      </SkeletonTheme>
    </NextUIProvider>
  );
};

export default App;
