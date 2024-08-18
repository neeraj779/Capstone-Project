import { Outlet } from "react-router-dom";
import { useAuth0 } from "@auth0/auth0-react";
import { Spinner } from "@nextui-org/react";
import Logout from "../components/Logout";

const ProtectedRoute = () => {
  const { isAuthenticated, isLoading } = useAuth0();

  if (isLoading) {
    return (
      <div className="flex justify-center items-center h-[70vh]">
        <Spinner color="default" size="lg" />
      </div>
    );
  }

  if (!isAuthenticated) return <Logout />;

  return <Outlet />;
};

export default ProtectedRoute;
