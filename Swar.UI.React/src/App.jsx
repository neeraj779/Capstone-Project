import { BrowserRouter, Route, Routes, Navigate } from "react-router-dom";
import { SkeletonTheme } from "react-loading-skeleton";

import NavBar from "./components/Navbar";
import Home from "./pages/Home";
import Player from "./pages/Player/Player";
import MiniPlayer from "./components/MiniPlayer";
import Login from "./pages/Login";

import { PlayerProvider } from "./contexts/PlayerContext";
import { AuthProvider } from "./contexts/AuthContext";
import ProtectedRoute from "./routes/ProtectedRoute";

const App = () => (
  <SkeletonTheme baseColor="#6B7280" highlightColor="#4B5563">
    <BrowserRouter>
      <PlayerProvider>
        <AuthProvider>
          <NavBar />
          <Routes>
            <Route path="/login" element={<Login />} />
            <Route element={<ProtectedRoute />}>
              <Route path="/" element={<Home />} />
              <Route path="/song/:id" element={<Player />} />
            </Route>
            <Route path="*" element={<Navigate to="/" />} />
          </Routes>
          <MiniPlayer />
        </AuthProvider>
      </PlayerProvider>
    </BrowserRouter>
  </SkeletonTheme>
);

export default App;
