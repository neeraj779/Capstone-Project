import { useEffect, useState } from "react";
import toast from "react-hot-toast";
import { FaDownload } from "react-icons/fa6";

const InstallPWA = () => {
  const [deferredPrompt, setDeferredPrompt] = useState(null);
  const [installButton, setInstallButton] = useState(false);

  useEffect(() => {
    const handleBeforeInstallPrompt = (e) => {
      e.preventDefault();
      setDeferredPrompt(e);
      setInstallButton(true);
    };

    const handleAppInstalled = () => {
      toast.success("App installed successfully!");
    };

    window.addEventListener("beforeinstallprompt", handleBeforeInstallPrompt);
    window.addEventListener("appinstalled", handleAppInstalled);

    return () => {
      window.removeEventListener(
        "beforeinstallprompt",
        handleBeforeInstallPrompt
      );
      window.removeEventListener("appinstalled", handleAppInstalled);
    };
  }, []);

  const installApp = async () => {
    if (deferredPrompt) {
      deferredPrompt.prompt();
      setDeferredPrompt(null);
      setInstallButton(false);
    }
  };

  if (!installButton) return null;

  return (
    <button
      className="fixed z-50 p-4 text-white bg-gray-700 rounded-lg shadow-lg bottom-6 right-6 hover:bg-gray-600 focus:outline-none focus:ring-2 focus:ring-gray-500"
      onClick={installApp}
    >
      <FaDownload />
    </button>
  );
};

export default InstallPWA;
