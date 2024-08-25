import PropTypes from "prop-types";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth0 } from "@auth0/auth0-react";
import { Button } from "@nextui-org/react";
import { FaPlay, FaListUl, FaCloudDownloadAlt, FaLaptop } from "react-icons/fa";
import Typewriter from "typewriter-effect";
import InstallPWA from "../components/InstallPWA";
import Footer from "../components/Footer";
import logo from "../assets/img/logo.png";

const FeatureCard = ({ icon, title, description }) => (
  <div
    className="p-8 transition-transform transform border border-gray-700 shadow-2xl bg-gradient-to-br from-gray-800 to-gray-900 rounded-3xl hover:scale-105 hover:shadow-xl"
    aria-label={title}
  >
    <div className="flex justify-center mb-6">{icon}</div>
    <h3 className="mb-4 text-xl font-semibold md:text-2xl">{title}</h3>
    <p className="text-gray-300">{description}</p>
  </div>
);

FeatureCard.propTypes = {
  icon: PropTypes.element.isRequired,
  title: PropTypes.string.isRequired,
  description: PropTypes.string.isRequired,
};

function LandingPage() {
  const navigate = useNavigate();
  const { loginWithRedirect, isAuthenticated, isLoading } = useAuth0();
  const [isButtonLoading, setIsButtonLoading] = useState(false);

  useEffect(() => {
    if (!isLoading && isButtonLoading) {
      if (isAuthenticated) navigate("/home", { replace: true });
      else loginWithRedirect();
      setIsButtonLoading(false);
    }
  }, [
    isAuthenticated,
    isButtonLoading,
    isLoading,
    loginWithRedirect,
    navigate,
  ]);

  const features = [
    {
      icon: (
        <FaPlay className="text-6xl text-green-500 md:text-7xl lg:text-8xl" />
      ),
      title: "High-Quality Streaming",
      description: "Enjoy your music in stunning high-definition audio.",
    },
    {
      icon: (
        <FaListUl className="text-6xl text-blue-500 md:text-7xl lg:text-8xl" />
      ),
      title: "Create Unlimited Playlists",
      description: "Organize your music with unlimited playlists.",
    },
    {
      icon: (
        <FaCloudDownloadAlt className="text-6xl text-yellow-500 md:text-7xl lg:text-8xl" />
      ),
      title: "Download Songs",
      description: "Save your favorite tracks for offline listening.",
    },
    {
      icon: (
        <FaLaptop className="text-6xl text-purple-500 md:text-7xl lg:text-8xl" />
      ),
      title: "Cross-Device Sync",
      description: "Enjoy seamless music access across all your devices.",
    },
  ];

  return (
    <div>
      <nav className="fixed top-0 z-50 w-full py-4 text-white bg-gray-800 bg-opacity-50 border-b border-gray-700 shadow-lg backdrop-blur-lg">
        <div className="container flex items-center justify-between px-6 mx-auto">
          <div className="flex items-center space-x-4">
            <p className="flex items-center text-2xl font-bold">
              <img src={logo} alt="Logo" className="w-auto h-10" />
            </p>
          </div>
          <Button
            isLoading={isButtonLoading}
            color="primary"
            variant="shadow"
            size="md"
            onClick={() => setIsButtonLoading(true)}
          >
            {!isButtonLoading && <FaPlay className="mr-2 text-lg" />} Get
            Started
          </Button>
        </div>
      </nav>
      <div className="relative h-screen bg-center bg-cover bg-custom-bg">
        <div className="absolute inset-0 flex flex-col items-center justify-center p-6 text-center bg-gradient-to-b from-black via-transparent to-black">
          <h1 className="mb-6 text-4xl font-extrabold text-white shadow-lg md:text-6xl drop-shadow-md">
            <Typewriter
              options={{
                strings: [
                  "Find Your Rhythm with Swar",
                  "Discover Music Like Never Before",
                  "Stream Your Favorite Tracks",
                  "Elevate Your Listening Experience",
                ],
                autoStart: true,
                loop: true,
                delay: 90,
                cursor: "|",
              }}
            />
          </h1>
          <p className="max-w-3xl px-4 mb-10 text-lg text-gray-200 md:text-2xl">
            Stream the music you love, anytime, anywhere. Discover a world of
            sound at your fingertips.
          </p>
          <div className="flex gap-4">
            <Button
              isLoading={isButtonLoading}
              color="primary"
              variant="shadow"
              className="px-8 py-3"
              size="md"
              onClick={() => setIsButtonLoading(true)}
            >
              {!isButtonLoading && <FaPlay className="mr-2 text-lg" />} Start
              Listening
            </Button>
          </div>
        </div>
      </div>

      <div className="py-24 text-center text-white bg-gray-900">
        <h2 className="mb-16 text-4xl font-bold md:text-5xl">Features</h2>
        <div className="grid grid-cols-1 gap-12 px-6 mx-auto md:grid-cols-2 lg:grid-cols-4 max-w-7xl">
          {features.map((feature, index) => (
            <FeatureCard
              key={index}
              icon={feature.icon}
              title={feature.title}
              description={feature.description}
            />
          ))}
        </div>
      </div>

      <InstallPWA />
      <Footer />
    </div>
  );
}

export default LandingPage;
