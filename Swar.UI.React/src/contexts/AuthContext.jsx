import {
  createContext,
  useCallback,
  useEffect,
  useMemo,
  useState,
} from "react";

const AuthContext = createContext();

// eslint-disable-next-line react/prop-types
export const AuthProvider = ({ children }) => {
  const [accessToken, setAccessToken] = useState(() =>
    localStorage.getItem("accessToken")
  );
  const [refreshToken, setRefreshToken] = useState(() =>
    localStorage.getItem("refreshToken")
  );

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

  const resetTokens = () => {
    setAccessToken(null);
    setRefreshToken(null);
  };

  const contextValue = useMemo(
    () => ({
      accessToken,
      refreshToken,
      updateAccessToken: setAccessToken,
      updateRefreshToken: setRefreshToken,
      resetTokens,
    }),
    [accessToken, refreshToken]
  );

  return (
    <AuthContext.Provider value={contextValue}>{children}</AuthContext.Provider>
  );
};

export default AuthContext;
