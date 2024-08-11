import { Link } from "react-router-dom";
import { House, Library, User } from "lucide-react";

const MobileNav = () => {
  return (
    <div className="fixed bottom-0 inset-x-0 bg-gray-800 text-white md:hidden z-20">
      <div className="flex justify-around items-center py-2">
        <Link to="/" className="flex flex-col items-center">
          <House />
          <span className="text-xs">Home</span>
        </Link>
        <Link to="/" className="flex flex-col items-center">
          <Library />
          <span className="text-xs">Library</span>
        </Link>
        <Link to="/profile" className="flex flex-col items-center">
          <User />
          <span className="text-xs">Profile</span>
        </Link>
      </div>
    </div>
  );
};

export default MobileNav;
