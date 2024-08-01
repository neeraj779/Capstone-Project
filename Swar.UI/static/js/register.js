document.addEventListener("DOMContentLoaded", () => {
  const registerForm = document.getElementById("registerForm");
  const messageDiv = document.getElementById("message");
  const spinner = document.getElementById("spinner");
  const buttonText = document.getElementById("buttonText");
  const registerButton = document.getElementById("registerButton");
  const nameInput = document.getElementById("name");
  const emailInput = document.getElementById("email");
  const genderInput = document.getElementById("gender");
  const passwordInput = document.getElementById("password");
  const confirmPasswordInput = document.getElementById("confirmPassword");

  registerForm.addEventListener("submit", async (event) => {
    event.preventDefault();

    hideMessage();
    showSpinner();

    const name = nameInput.value.trim();
    const email = emailInput.value.trim();
    const gender = genderInput.value;
    const password = passwordInput.value;
    const confirmPassword = confirmPasswordInput.value;

    if (password !== confirmPassword) {
      showMessage("Passwords do not match", "error");
      stopSpinner();
      return;
    }

    try {
      const data = { name, email, gender, password };
      await CRUDService.create("users/register", data);
      handleSuccess();
    } catch (error) {
      showMessage(error.message || "An error occurred", "error");
    } finally {
      stopSpinner();
    }
  });

  function showMessage(message, type) {
    messageDiv.textContent = message;
    messageDiv.className = `mt-4 text-center ${type}`;
    messageDiv.classList.remove("hidden");
  }

  function hideMessage() {
    messageDiv.classList.add("hidden");
  }

  function showSpinner() {
    spinner.classList.remove("hidden");
    buttonText.textContent = "Registering...";
    registerButton.disabled = true;
  }

  function stopSpinner() {
    spinner.classList.add("hidden");
    buttonText.textContent = "Register";
    registerButton.disabled = false;
  }

  function handleSuccess() {
    registerButton.classList.add("hidden");
    showMessage(
      "Account created successfully! Redirecting to login...",
      "success"
    );
    setTimeout(() => {
      window.location.href = "./login.html";
    }, 2000);
  }
});
