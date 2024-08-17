import { Outlet } from "react-router-dom";
import { useAuth0 } from "@auth0/auth0-react";
import { Spinner } from "@nextui-org/react";

const ProtectedRoute = () => {
  const { isAuthenticated, isLoading, loginWithRedirect } = useAuth0();

  if (isLoading) {
    return (
      <div className="flex justify-center items-center h-[70vh]">
        <Spinner label="Loading..." color="default" labelColor="secondary" />
      </div>
    );
  }

  if (!isAuthenticated) {
    loginWithRedirect();
    return null;
  }

  return <Outlet />;
};

export default ProtectedRoute;
