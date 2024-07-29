function checkAuth() {
  const accessToken = localStorage.getItem("accessToken");
  const refreshToken = localStorage.getItem("refreshToken");

  if (!accessToken && !refreshToken) window.location.href = "./login.html";
}

window.onload = function () {
  checkAuth();
};
