import { useNavigate, Route, Routes, Navigate } from "react-router-dom";
import { NextUIProvider } from "@nextui-org/react";
import { SkeletonTheme } from "react-loading-skeleton";
import { Toaster } from "react-hot-toast";

import NavBar from "./components/Navbar";
import Home from "./pages/Home";
import Player from "./pages/Player/Player";
import Search from "./pages/Search";
import MiniPlayer from "./components/MiniPlayer";
import Login from "./pages/Login";
import Register from "./pages/Register";
import Profile from "./pages/Profile";

import { PlayerProvider } from "./contexts/PlayerContext";
import { AuthProvider } from "./contexts/AuthContext";
import ProtectedRoute from "./routes/ProtectedRoute";

const App = () => {
  const navigate = useNavigate();

  return (
    <NextUIProvider navigate={navigate}>
      <SkeletonTheme baseColor="#6B7280" highlightColor="#4B5563">
        <AuthProvider>
          <PlayerProvider>
            <NavBar />
            <Routes>
              <Route path="/login" element={<Login />} />
              <Route path="/register" element={<Register />} />
              <Route element={<ProtectedRoute />}>
                <Route path="/" element={<Home />} />
                <Route path="/song/:id" element={<Player />} />
                <Route path="/search" element={<Search />} />
                <Route path="/profile" element={<Profile />} />
              </Route>
              <Route path="*" element={<Navigate to="/" />} />
            </Routes>
            <MiniPlayer />
            <Toaster />
          </PlayerProvider>
        </AuthProvider>
      </SkeletonTheme>
    </NextUIProvider>
  );
};

export default App;
