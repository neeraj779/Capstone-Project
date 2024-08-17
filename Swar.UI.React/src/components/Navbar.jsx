import { useState } from "react";
import { Link } from "react-router-dom";
import { useAuth0 } from "@auth0/auth0-react";
import {
  DropdownItem,
  DropdownTrigger,
  Dropdown,
  DropdownMenu,
  Avatar,
} from "@nextui-org/react";
import UpdatePasswordModal from "../components/modals/UpdatePasswordModal";

import logo from "../assets/img/logo.png";
import profile from "../assets/img/profile.svg";
import SearchBar from "./SearchBar";

const Navbar = () => {
  const { isAuthenticated, logout } = useAuth0();
  const [isUpdatePasswordOpen, setIsUpdatePasswordOpen] = useState(false);
  const userEmail = localStorage.getItem("email") || "User";

  return (
    <>
      <nav className="bg-gray-800 py-4 shadow-md">
        <div className="container mx-auto flex items-center justify-between px-4 md:px-8">
          {/* Logo and Navigation Links */}
          <div className="flex items-center space-x-6">
            <Link to="/" className="flex items-center">
              <img src={logo} alt="Logo" className="h-10 w-auto" />
            </Link>
            <div className="hidden md:flex space-x-6">
              {isAuthenticated && (
                <>
                  <Link
                    to="/"
                    className="hover:text-gray-400 transition-colors"
                  >
                    Home
                  </Link>
                  <Link
                    to="/library"
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
            {isAuthenticated && (
              <div className="hidden md:block">
                <SearchBar />
              </div>
            )}

            {/* Profile Dropdown */}
            {isAuthenticated && (
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
                  <DropdownItem
                    key="change-password"
                    onClick={() => setIsUpdatePasswordOpen(true)}
                  >
                    Change Password
                  </DropdownItem>
                  <DropdownItem key="logout" color="danger" onClick={logout}>
                    Log Out
                  </DropdownItem>
                </DropdownMenu>
              </Dropdown>
            )}
            <UpdatePasswordModal
              isOpen={isUpdatePasswordOpen}
              onOpenChange={() => setIsUpdatePasswordOpen(false)}
            />
          </div>
        </div>
      </nav>
    </>
  );
};

export default Navbar;
