import PropTypes from "prop-types";
import useAuth from "../hooks/useAuth";
import usePlayer from "../hooks/usePlayer";

import Navbar from "./Navbar";
import MobileNav from "./MobileNav";
import MiniPlayer from "./MiniPlayer";

const Layout = ({ children }) => {
  const { accessToken } = useAuth();
  const { isPlaying } = usePlayer();

  const isMobile = window.innerWidth < 768;
  const bottomPadding = isPlaying && isMobile ? "pb-24" : "pb-2";

  return (
    <div
      className={`flex flex-col bg-gray-900 text-white min-h-screen ${bottomPadding}`}
    >
      <Navbar />

      <main className="flex-grow">{children}</main>

      <MiniPlayer />
      {accessToken && <MobileNav />}
    </div>
  );
};

Layout.propTypes = {
  children: PropTypes.node.isRequired,
};

export default Layout;
