import { useEffect, useMemo } from "react";
import { useAuth0 } from "@auth0/auth0-react";
import apiClient from "../api/axios";

const useApiClient = (isSongService = false) => {
  const { getAccessTokenSilently } = useAuth0();
  const client = useMemo(() => apiClient(isSongService), [isSongService]);

  useEffect(() => {
    const requestInterceptor = client.interceptors.request.use(
      async (config) => {
        const accessToken = await getAccessTokenSilently();
        if (!config.headers.Authorization && accessToken) {
          config.headers.Authorization = `Bearer ${accessToken}`;
        }
        return config;
      },
      (error) => Promise.reject(error)
    );

    return () => {
      client.interceptors.request.eject(requestInterceptor);
    };
  }, [client, getAccessTokenSilently]);

  return client;
};

export default useApiClient;
