import { useEffect } from "react";
import { Navigate, Outlet } from "react-router-dom";
import { useAuth0 } from "@auth0/auth0-react";
import { Spinner } from "@nextui-org/react";
import toast from "react-hot-toast";

const ProtectedRoute = () => {
  const { isAuthenticated, isLoading, user } = useAuth0();

  useEffect(() => {
    if (isAuthenticated && user) {
      toast(`Hello, ${user?.name}!`, { icon: "👏" });
    }
  }, [isAuthenticated, user]);

  if (isLoading) {
    return (
      <div className="flex justify-center items-center h-[70vh]">
        <Spinner color="default" size="lg" />
      </div>
    );
  }

  if (!isAuthenticated) {
    return <Navigate to="/" replace />;
  }

  return <Outlet />;
};

export default ProtectedRoute;
