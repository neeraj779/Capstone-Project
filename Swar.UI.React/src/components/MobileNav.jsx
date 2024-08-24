import { Link } from "react-router-dom";
import { FaHome, FaUser } from "react-icons/fa";
import { LuLibrary } from "react-icons/lu";

const MobileNav = () => {
  return (
    <div className="fixed bottom-0 inset-x-0 bg-gray-800 text-white md:hidden z-20">
      <div className="flex justify-around items-center py-2">
        <Link to="/home" className="flex flex-col items-center">
          <FaHome className="text-xl" />
          <span className="text-xs">Home</span>
        </Link>
        <Link to="/library" className="flex flex-col items-center">
          <LuLibrary className="text-xl" />
          <span className="text-xs">Library</span>
        </Link>
        <Link to="/profile" className="flex flex-col items-center">
          <FaUser className="text-xl" />
          <span className="text-xs">Profile</span>
        </Link>
      </div>
    </div>
  );
};

export default MobileNav;
