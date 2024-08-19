import PropTypes from "prop-types";
import { useLocation } from "react-router-dom";
import { useAuth0 } from "@auth0/auth0-react";
import usePlayer from "../hooks/usePlayer";

import Navbar from "./Navbar";
import MobileNav from "./MobileNav";
import MiniPlayer from "./MiniPlayer";
import InstallPWA from "./InstallPWA";

const Layout = ({ children }) => {
  const location = useLocation();
  const { isAuthenticated } = useAuth0();
  const { isPlaying } = usePlayer();
  const showNavbar = location.pathname !== "/";

  const isMobile = window.innerWidth < 768;
  const bottomPadding = isPlaying && isMobile ? "pb-24" : "pb-8";

  return (
    <div
      className={`flex flex-col bg-gray-900 text-white min-h-screen ${bottomPadding}`}
    >
      {showNavbar && <Navbar />}

      <main className="flex-grow">{children}</main>

      <InstallPWA />
      <MiniPlayer />
      {showNavbar && isAuthenticated && <MobileNav />}
    </div>
  );
};

Layout.propTypes = {
  children: PropTypes.node.isRequired,
};

export default Layout;
