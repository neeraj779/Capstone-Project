import PropTypes from "prop-types";
import { useAuth0 } from "@auth0/auth0-react";
import usePlayer from "../hooks/usePlayer";

import Navbar from "./Navbar";
import MobileNav from "./MobileNav";
import MiniPlayer from "./MiniPlayer";

const Layout = ({ children }) => {
  const { isAuthenticated } = useAuth0();
  const { isPlaying } = usePlayer();

  const isMobile = window.innerWidth < 768;
  const bottomPadding = isPlaying && isMobile ? "pb-24" : "pb-8";

  return (
    <div
      className={`flex flex-col bg-gray-900 text-white min-h-screen ${bottomPadding}`}
    >
      <Navbar />

      <main className="flex-grow">{children}</main>

      <MiniPlayer />
      {isAuthenticated && <MobileNav />}
    </div>
  );
};

Layout.propTypes = {
  children: PropTypes.node.isRequired,
};

export default Layout;
