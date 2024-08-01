const button = document.getElementById("dropdown-button");
const logoutButton = document.getElementById("logout-button");
const dropdownContent = document.getElementById("dropdown-content");
const hamburger = document.getElementById("hamburger");
const mobileMenu = document.getElementById("mobile-menu");

button.addEventListener("click", () => {
  const isVisible = dropdownContent.classList.contains("show");
  dropdownContent.classList.toggle("show", !isVisible);
  button.setAttribute("aria-expanded", !isVisible);
});

document.addEventListener("click", (event) => {
  if (
    !button.contains(event.target) &&
    !dropdownContent.contains(event.target)
  ) {
    dropdownContent.classList.remove("show");
    button.setAttribute("aria-expanded", "false");
  }
});

logoutButton.addEventListener("click", () => {
  localStorage.clear();
  window.location.reload();
});

hamburger?.addEventListener("click", () => {
  mobileMenu.classList.toggle("show");
});
