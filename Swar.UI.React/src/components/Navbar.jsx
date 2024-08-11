import { useNavigate, Link } from "react-router-dom";
import {
  DropdownItem,
  DropdownTrigger,
  Dropdown,
  DropdownMenu,
  Avatar,
} from "@nextui-org/react";
import useAuth from "../hooks/useAuth";
import usePlayer from "../hooks/usePlayer";
import { House, Library, User } from "lucide-react";
import logo from "../assets/img/logo.png";
import profile from "../assets/img/profile.svg";
import SearchBar from "./SearchBar";

const Navbar = () => {
  const { accessToken, resetTokens } = useAuth();
  const { resetPlayer } = usePlayer();
  const userEmail = localStorage.getItem("email") || "User";
  const navigate = useNavigate();

  const handleLogout = () => {
    resetPlayer();
    resetTokens();
    navigate("/login");
  };

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
              {accessToken && (
                <>
                  <Link
                    to="/"
                    className="hover:text-gray-400 transition-colors"
                  >
                    Home
                  </Link>
                  <Link
                    to="/"
                    className="hover:text-gray-400 transition-colors"
                  >
                    Your Library
                  </Link>
                </>
              )}
            </div>
          </div>

          {/* Search and Profile */}
          <div className="flex items-center space-x-4">
            {accessToken && (
              <div className="hidden md:block">
                <SearchBar />
              </div>
            )}

            {/* Profile Dropdown */}
            {accessToken && (
              <Dropdown
                placement="bottom-end"
                className="bg-gray-800 text-white"
              >
                <DropdownTrigger>
                  <Avatar
                    as="button"
                    className="transition-transform bg-gray-800"
                    size="sm"
                    src={profile}
                  />
                </DropdownTrigger>
                <DropdownMenu aria-label="Profile Actions" variant="shadow">
                  <DropdownItem
                    textValue="profile"
                    key="user"
                    className="h-14 gap-2"
                    href="/profile"
                  >
                    <p className="font-semibold">Signed in as</p>
                    <p className="font-semibold">{userEmail}</p>
                  </DropdownItem>
                  <DropdownItem key="profile" href="/profile">
                    Profile
                  </DropdownItem>
                  <DropdownItem key="change-password">
                    Change Password
                  </DropdownItem>
                  <DropdownItem
                    key="logout"
                    color="danger"
                    onClick={handleLogout}
                  >
                    Log Out
                  </DropdownItem>
                </DropdownMenu>
              </Dropdown>
            )}
          </div>
        </div>
      </nav>

      {/* Mobile Bottom Navigation */}
      {accessToken && (
        <div className="fixed bottom-0 inset-x-0 bg-gray-800 text-white md:hidden z-30">
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
      )}
    </>
  );
};

export default Navbar;
