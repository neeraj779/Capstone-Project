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
      const response = await fetch("https://swarapi.azurewebsites.net/api/v1/Auth/Login", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ email, password }),
      });

      if (!response.ok) {
        const error = await response.json();
        errorMessageDiv.textContent = error.message || "An error occurred.";
        errorMessageDiv.classList.remove("hidden");
        return;
      }

      const data = await response.json();
      localStorage.setItem("accessToken", data.accessToken);
      localStorage.setItem("refreshToken", data.refreshToken);
      window.location.href = "./index.html";
    } catch {
      errorMessageDiv.textContent = "An error occurred while trying to log in.";
      errorMessageDiv.classList.remove("hidden");
    } finally {
      toggleUI(false);
    }
  });
