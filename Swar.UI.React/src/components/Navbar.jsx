import { useState, useRef } from "react";
import { useNavigate, Link } from "react-router-dom";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faUser,
  faSignOutAlt,
  faSearch,
  faHome,
  faMusic,
} from "@fortawesome/free-solid-svg-icons";
import useClickOutside from "../hooks/useClickOutside";
import logo from "../assets/img/logo.png";
import profile from "../assets/img/profile.svg";

const Navbar = () => {
  const navigate = useNavigate();
  const [isDropdownOpen, setDropdownOpen] = useState(false);
  const dropdownRef = useRef(null);

  const toggleDropdown = () => setDropdownOpen((prev) => !prev);
  const closeDropdown = () => setDropdownOpen(false);

  const handleLogout = () => {
    localStorage.removeItem("accessToken");
    localStorage.removeItem("refreshToken");
    closeDropdown();
    navigate("/login");
  };

  useClickOutside(dropdownRef, closeDropdown);

  return (
    <>
      <nav className="bg-gray-800 py-4 shadow-md text-white">
        <div className="container mx-auto flex items-center justify-between px-4 md:px-8">
          {/* Logo and Navigation Links */}
          <div className="flex items-center space-x-6">
            <Link to="/" className="flex items-center">
              <img src={logo} alt="Logo" className="h-10 w-auto" />
            </Link>
            <div className="hidden md:flex space-x-6">
              <Link to="/" className="hover:text-gray-400 transition-colors">
                Home
              </Link>
              <Link
                to="/library"
                className="hover:text-gray-400 transition-colors"
              >
                Your Library
              </Link>
            </div>
          </div>

          {/* Search and Profile */}
          <div className="flex items-center space-x-4">
            <div className="relative hidden md:block">
              <input
                type="text"
                className="bg-gray-700 text-white placeholder-gray-400 rounded-full py-2 px-4 pl-10 focus:outline-none"
                placeholder="Search songs, artists..."
              />
              <FontAwesomeIcon
                icon={faSearch}
                className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400"
                size="lg"
              />
            </div>

            {/* Profile Dropdown */}
            <div className="relative" ref={dropdownRef}>
              <button
                className="text-gray-400 hover:text-gray-300 focus:outline-none"
                aria-haspopup="true"
                aria-expanded={isDropdownOpen}
                onClick={toggleDropdown}
              >
                <img
                  src={profile}
                  alt="Profile"
                  className="w-8 h-8 rounded-full"
                />
              </button>
              {isDropdownOpen && (
                <div className="absolute right-0 mt-2 w-48 bg-gray-700 text-gray-300 rounded-md shadow-lg z-20">
                  <Link
                    to="/profile"
                    className="block px-4 py-2 hover:bg-gray-600 transition-colors rounded-md"
                    onClick={closeDropdown}
                  >
                    <FontAwesomeIcon icon={faUser} className="mr-2" />
                    View Profile
                  </Link>
                  <button
                    onClick={handleLogout}
                    className="w-full text-left px-4 py-2 hover:bg-gray-600 transition-colors rounded-md"
                  >
                    <FontAwesomeIcon icon={faSignOutAlt} className="mr-2" />
                    Logout
                  </button>
                </div>
              )}
            </div>
          </div>
        </div>
      </nav>

      {/* Mobile Bottom Navigation */}
      <div className="fixed bottom-0 inset-x-0 bg-gray-800 text-white md:hidden z-30">
        <div className="flex justify-around items-center py-2">
          <Link to="/" className="flex flex-col items-center">
            <FontAwesomeIcon icon={faHome} size="lg" />
            <span className="text-xs">Home</span>
          </Link>
          <Link to="/library" className="flex flex-col items-center">
            <FontAwesomeIcon icon={faMusic} size="lg" />
            <span className="text-xs">Library</span>
          </Link>
          <button
            onClick={toggleDropdown}
            className="flex flex-col items-center"
            aria-haspopup="true"
            aria-expanded={isDropdownOpen}
          >
            <FontAwesomeIcon icon={faUser} size="lg" />
            <span className="text-xs">Profile</span>
          </button>
        </div>
      </div>
    </>
  );
};

export default Navbar;
