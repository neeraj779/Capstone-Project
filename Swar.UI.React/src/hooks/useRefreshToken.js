import axios from "axios";
import useAuth from "../hooks/useAuth";

const useRefreshToken = () => {
  const { refreshToken, updateAccessToken } = useAuth();

  const refresh = async () => {
    const response = await axios.post(
      import.meta.env.VITE_REFRESH_TOKEN_API_URL,
      null,
      {
        headers: { Authorization: `Bearer ${refreshToken}` },
      }
    );
    const newAccessToken = response.data.accessToken;
    updateAccessToken(newAccessToken);
    return newAccessToken;
  };
  return refresh;
};

export default useRefreshToken;
