import { useEffect, useState } from "react";
import usePlayer from "../hooks/usePlayer";
import toast from "react-hot-toast";
import { FaDownload } from "react-icons/fa6";

const InstallPWA = () => {
  const [deferredPrompt, setDeferredPrompt] = useState(null);
  const [installButton, setInstallButton] = useState(false);

  const { isPlaying } = usePlayer();
  const isMobile = window.innerWidth < 768;
  const bottomMargin = isPlaying && isMobile ? "mb-24" : "mb-8";

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
      className={`fixed bottom-6 right-6 bg-gray-700 hover:bg-gray-600 text-white p-4 rounded-lg focus:outline-none focus:ring-2 focus:ring-gray-500 shadow-lg z-50 ${bottomMargin}`}
      onClick={installApp}
    >
      <FaDownload />
    </button>
  );
};

export default InstallPWA;
