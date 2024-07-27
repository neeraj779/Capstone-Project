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
    title: "🎉 Installation Complete!",
    html: `
    <div class="custom-alert">
    <p>Great news! <span class="highlight">Swar is now installed on your device.</span> Enjoy exploring all the amazing features! 🚀</p>
    </div>`,
    showConfirmButton: false,
    toast: true,
    position: "top-end",
    timer: 4000,
    timerProgressBar: true,
    background: "#111828",
  });
});
