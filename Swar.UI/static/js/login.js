document
  .getElementById("loginForm")
  .addEventListener("submit", async (event) => {
    event.preventDefault();

    const email = document.getElementById("email").value;
    const password = document.getElementById("password").value;
    const errorMessageDiv = document.getElementById("error-message");
    const loginButton = document.getElementById("loginButton");
    const buttonText = document.getElementById("buttonText");
    const spinner = document.getElementById("spinner");

    const toggleUI = (isLoading) => {
      loginButton.classList.toggle("opacity-50", isLoading);
      loginButton.classList.toggle("cursor-not-allowed", isLoading);
      spinner.classList.toggle("hidden", !isLoading);
      buttonText.classList.toggle("hidden", isLoading);
    };

    toggleUI(true);

    try {
      const data = { email, password };
      const response = await CRUDService.create("Auth/Login", data);

      localStorage.setItem("accessToken", response.accessToken);
      localStorage.setItem("refreshToken", response.refreshToken);
      window.location.href = "./index.html";
    } catch(error) {
      errorMessageDiv.textContent = error.message || "An error occurred.";
      errorMessageDiv.classList.remove("hidden");
    } finally {
      toggleUI(false);
    }
  });
