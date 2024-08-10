import { useEffect, useMemo } from "react";
import useAuth from "./useAuth";
import apiClient from "../api/axios";
import useRefreshToken from "./useRefreshToken";
import toast from "react-hot-toast";

const useApiClient = (isSongService = false) => {
  const { accessToken, resetTokens } = useAuth();
  const refresh = useRefreshToken();

  const client = useMemo(() => apiClient(isSongService), [isSongService]);

  useEffect(() => {
    const requestInterceptor = client.interceptors.request.use(
      (config) => {
        if (!config.headers.Authorization && accessToken) {
          config.headers.Authorization = `Bearer ${accessToken}`;
        }
        return config;
      },
      (error) => Promise.reject(error)
    );

    const responseInterceptor = client.interceptors.response.use(
      (response) => response,
      async (error) => {
        const originalRequest = error.config;
        if (error.response?.status === 401 && !originalRequest._retry) {
          originalRequest._retry = true;
          try {
            const newAccessToken = await refresh();
            originalRequest.headers.Authorization = `Bearer ${newAccessToken}`;

            return client(originalRequest);
          } catch (refreshError) {
            resetTokens();
            toast.error("Session expired. Please log in again");
            return Promise.reject(refreshError);
          }
        }
        return Promise.reject(error);
      }
    );

    return () => {
      client.interceptors.request.eject(requestInterceptor);
      client.interceptors.response.eject(responseInterceptor);
    };
  }, [accessToken, refresh, resetTokens, client]);

  return client;
};

export default useApiClient;
