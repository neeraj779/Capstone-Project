window.addEventListener("DOMContentLoaded", async (event) => {
  document
    .querySelector("#install-button")
    .addEventListener("click", installApp);
});

let deferredPrompt;
const installButton = document.querySelector("#install-button");

window.addEventListener("beforeinstallprompt", (e) => {
  e.preventDefault();
  deferredPrompt = e;
  installButton.classList.remove("hidden");
});

async function installApp() {
  if (deferredPrompt) {
    deferredPrompt.prompt();
    deferredPrompt = null;
    installButton.classList.add("hidden");
  }
}

window.addEventListener("appinstalled", () => {
  Swal.fire({
    icon: "success",
    title: "App Installed 😀",
    text: "The app has been successfully installed on your device. 🚀",
    showConfirmButton: false,
    toast: true,
    position: "top-end",
    timer: 4000,
    timerProgressBar: true,
    background: "#111828",
  });
});

Swal.fire({
  icon: "success",
  title: "🎉 Installation Complete!",
  html: '<div class="swal2-text">Great news! The app is now installed on your device. Enjoy exploring all the amazing features! 🚀</div>',
  showConfirmButton: false,
  toast: true,
  position: "top-end",
  timer: 4000,
  timerProgressBar: true,
  background: "#111828",
});
