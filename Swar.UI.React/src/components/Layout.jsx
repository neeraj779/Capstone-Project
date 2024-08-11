import PropTypes from "prop-types";
import useAuth from "../hooks/useAuth";
import Navbar from "./Navbar";
import MobileNav from "./MobileNav";
import MiniPlayer from "./MiniPlayer";

const Layout = ({ children }) => {
  const { accessToken } = useAuth();

  return (
    <div className="flex flex-col bg-gray-900 text-white min-h-screen pb-12 md:pb-0">
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
