import {
  createContext,
  useCallback,
  useEffect,
  useMemo,
  useState,
} from "react";
import { useNavigate } from "react-router-dom";

const AuthContext = createContext();

// eslint-disable-next-line react/prop-types
export const AuthProvider = ({ children }) => {
  const [accessToken, setAccessToken] = useState(() =>
    localStorage.getItem("accessToken")
  );
  const [refreshToken, setRefreshToken] = useState(() =>
    localStorage.getItem("refreshToken")
  );
  const navigate = useNavigate();

  const updateLocalStorage = useCallback((key, value) => {
    if (value) {
      localStorage.setItem(key, value);
    } else {
      localStorage.removeItem(key);
    }
  }, []);

  useEffect(() => {
    updateLocalStorage("accessToken", accessToken);
  }, [accessToken, updateLocalStorage]);

  useEffect(() => {
    updateLocalStorage("refreshToken", refreshToken);
  }, [refreshToken, updateLocalStorage]);

  const redirectToLogin = useCallback(() => {
    navigate("/login");
  }, [navigate]);

  const verifyAccessToken = useCallback(
    async (token) => {
      try {
        const response = await fetch(
          "https://swarapi.azurewebsites.net/api/v1/users/verify-token",
          {
            method: "GET",
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }
        );
        return response.ok;
      } catch (error) {
        console.error("Error verifying access token:", error);
        redirectToLogin();
        return false;
      }
    },
    [redirectToLogin]
  );

  const refreshAccessToken = useCallback(
    async (token) => {
      try {
        const response = await fetch(
          "https://swarapi.azurewebsites.net/api/v1/users/refresh-token",
          {
            method: "POST",
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }
        );

        if (response.ok) {
          const data = await response.json();
          setAccessToken(data.accessToken);
          window.location.reload();
        } else if (response.status === 401) {
          console.warn("Refresh token is invalid or expired.");
          redirectToLogin();
        }
      } catch (error) {
        console.error("Error refreshing access token:", error);
        redirectToLogin();
      }
    },
    [redirectToLogin]
  );

  const checkAuth = useCallback(async () => {
    if (!accessToken && !refreshToken) {
      return redirectToLogin();
    }

    if (accessToken) {
      const isValid = await verifyAccessToken(accessToken);
      if (isValid) return;

      if (refreshToken) {
        await refreshAccessToken(refreshToken);
      } else {
        redirectToLogin();
      }
    } else if (refreshToken) {
      await refreshAccessToken(refreshToken);
    } else {
      redirectToLogin();
    }
  }, [
    accessToken,
    refreshToken,
    verifyAccessToken,
    refreshAccessToken,
    redirectToLogin,
  ]);

  useEffect(() => {
    checkAuth();
  }, [checkAuth]);

  const contextValue = useMemo(
    () => ({
      accessToken,
      refreshToken,
      updateAccessToken: setAccessToken,
      updateRefreshToken: setRefreshToken,
    }),
    [accessToken, refreshToken]
  );

  return (
    <AuthContext.Provider value={contextValue}>{children}</AuthContext.Provider>
  );
};

export default AuthContext;
