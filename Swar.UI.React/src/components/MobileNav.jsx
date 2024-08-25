import { Link } from "react-router-dom";
import { FaHome } from "react-icons/fa";
import { IoLibrary } from "react-icons/io5";
import { useAuth0 } from "@auth0/auth0-react";
import { Avatar } from "@nextui-org/react";

const MobileNav = () => {
  const { user } = useAuth0();

  return (
    <div className="fixed inset-x-0 bottom-0 z-20 text-white bg-gray-800 md:hidden">
      <div className="flex items-center justify-around py-2">
        <Link to="/home" className="flex flex-col items-center">
          <FaHome className="text-xl" />
          <span className="text-xs">Home</span>
        </Link>
        <Link to="/library" className="flex flex-col items-center">
          <IoLibrary className="text-xl" />
          <span className="text-xs">Library</span>
        </Link>
        <Link to="/profile" className="flex flex-col items-center">
          <Avatar src={user.picture} className="w-6 h-6" />
          <span className="text-xs">Profile</span>
        </Link>
      </div>
    </div>
  );
};

export default MobileNav;
