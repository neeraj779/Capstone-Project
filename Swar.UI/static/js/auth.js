function redirectToLogin() {
  window.location.href = "./login.html";
}

function getLocalStorageItem(item) {
  return localStorage.getItem(item);
}

function setLocalStorageItem(key, value) {
  localStorage.setItem(key, value);
}

async function checkAuth() {
  const accessToken = getLocalStorageItem("accessToken");
  const refreshToken = getLocalStorageItem("refreshToken");

  if (!accessToken && !refreshToken) {
    redirectToLogin();
    return;
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
}

async function verifyAccessToken(accessToken) {
  try {
    const response = await fetch(
      "https://swarapi.azurewebsites.net/api/v1/users/verify-token",
      {
        method: "GET",
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
      }
    );

    return response.ok;
  } catch (error) {
    console.error("Error verifying access token:", error);
    redirectToLogin();
    return false;
  }
}

async function refreshAccessToken(refreshToken) {
  try {
    const response = await fetch(
      "https://swarapi.azurewebsites.net/api/v1/users/refresh-token",
      {
        method: "POST",
        headers: {
          Authorization: `Bearer ${refreshToken}`,
        },
      }
    );

    if (response.ok) {
      const data = await response.json();
      setLocalStorageItem("accessToken", data.accessToken);
      window.location.reload();
    } else if (response.status === 401) {
      console.warn("Refresh token is invalid or expired.");
      redirectToLogin();
    }
  } catch (error) {
    console.error("Error refreshing access token:", error);
    redirectToLogin();
  }
}

checkAuth();
