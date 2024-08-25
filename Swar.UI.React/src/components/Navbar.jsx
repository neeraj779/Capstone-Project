import { Link } from "react-router-dom";
import { useAuth0 } from "@auth0/auth0-react";
import {
  DropdownItem,
  DropdownTrigger,
  Dropdown,
  DropdownMenu,
  Avatar,
} from "@nextui-org/react";

import logo from "../assets/img/logo.png";
import profile from "../assets/img/profile.svg";
import SearchBar from "./SearchBar";

const Navbar = () => {
  const { user, isAuthenticated, logout } = useAuth0();

  return (
    <>
      <nav className="py-4 bg-gray-800 shadow-md">
        <div className="container flex items-center justify-between px-4 mx-auto md:px-8">
          {/* Logo and Navigation Links */}
          <div className="flex items-center space-x-6">
            <Link to="/home" className="flex items-center">
              <img src={logo} alt="Logo" className="w-auto h-10" />
            </Link>
            <div className="hidden space-x-6 md:flex">
              {isAuthenticated && (
                <>
                  <Link
                    to="/home"
                    className="transition-colors hover:text-gray-400"
                  >
                    Home
                  </Link>
                  <Link
                    to="/library"
                    className="transition-colors hover:text-gray-400"
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
                className="text-white bg-gray-800"
              >
                <DropdownTrigger>
                  <Avatar
                    as="button"
                    size="md"
                    src={user ? user?.picture : profile}
                  />
                </DropdownTrigger>
                <DropdownMenu aria-label="Profile Actions" variant="shadow">
                  <DropdownItem
                    textValue="profile"
                    key="user"
                    className="gap-2 h-14"
                    href="/profile"
                  >
                    <p className="font-semibold">Signed in as</p>
                    <p className="font-semibold">{user?.nickname}</p>
                  </DropdownItem>
                  <DropdownItem key="profile" href="/profile">
                    Profile
                  </DropdownItem>
                  <DropdownItem key="logout" color="danger" onClick={logout}>
                    Log Out
                  </DropdownItem>
                </DropdownMenu>
              </Dropdown>
            )}
          </div>
        </div>
      </nav>
    </>
  );
};

export default Navbar;
