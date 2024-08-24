import PropTypes from "prop-types";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth0 } from "@auth0/auth0-react";
import { Button } from "@nextui-org/react";
import { FaPlay, FaListUl, FaCloudDownloadAlt, FaLaptop } from "react-icons/fa";
import Typewriter from "typewriter-effect";
import InstallPWA from "../components/InstallPWA";
import logo from "../assets/img/logo.png";

const FeatureCard = ({ icon, title, description }) => (
  <div
    className="bg-gradient-to-br from-gray-800 to-gray-900 p-8 rounded-3xl shadow-2xl border border-gray-700 transition-transform transform hover:scale-105 hover:shadow-xl"
    aria-label={title}
  >
    <div className="mb-6 flex justify-center">{icon}</div>
    <h3 className="text-xl md:text-2xl font-semibold mb-4">{title}</h3>
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
      if (isAuthenticated) navigate("/home");
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
        <FaPlay className="text-6xl md:text-7xl lg:text-8xl text-green-500" />
      ),
      title: "High-Quality Streaming",
      description: "Enjoy your music in stunning high-definition audio.",
    },
    {
      icon: (
        <FaListUl className="text-6xl md:text-7xl lg:text-8xl text-blue-500" />
      ),
      title: "Create Unlimited Playlists",
      description: "Organize your music with unlimited playlists.",
    },
    {
      icon: (
        <FaCloudDownloadAlt className="text-6xl md:text-7xl lg:text-8xl text-yellow-500" />
      ),
      title: "Download Songs",
      description: "Save your favorite tracks for offline listening.",
    },
    {
      icon: (
        <FaLaptop className="text-6xl md:text-7xl lg:text-8xl text-purple-500" />
      ),
      title: "Cross-Device Sync",
      description: "Enjoy seamless music access across all your devices.",
    },
  ];

  return (
    <div>
      <nav className="bg-gray-800 bg-opacity-50 backdrop-blur-lg text-white py-4 fixed top-0 w-full z-50 shadow-lg border-b border-gray-700">
        <div className="container mx-auto flex justify-between items-center px-6">
          <div className="flex items-center space-x-4">
            <p className="text-2xl font-bold flex items-center">
              <img src={logo} alt="Logo" className="h-10 w-auto" />
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
      <div className="relative h-screen bg-cover bg-center bg-[url('../../public/bg.png')]">
        <div className="absolute inset-0 bg-gradient-to-b from-black via-transparent to-black flex flex-col justify-center items-center text-center p-6">
          <h1 className="text-4xl md:text-6xl text-white font-extrabold mb-6 shadow-lg drop-shadow-md">
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
          <p className="text-lg md:text-2xl text-gray-200 mb-10 max-w-3xl px-4">
            Stream the music you love, anytime, anywhere. Discover a world of
            sound at your fingertips.
          </p>
          <div className="flex gap-4">
            <Button
              isLoading={isButtonLoading}
              color="primary"
              variant="shadow"
              className="py-3 px-8"
              size="md"
              onClick={() => setIsButtonLoading(true)}
            >
              {!isButtonLoading && <FaPlay className="mr-2 text-lg" />} Start
              Listening
            </Button>
          </div>
        </div>
      </div>

      <div className="py-24 bg-gray-900 text-center text-white">
        <h2 className="text-4xl md:text-5xl font-bold mb-16">Features</h2>
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-12 max-w-7xl mx-auto px-6">
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

      <footer>
        <div className="max-w-7xl mx-auto px-6 text-center">
          <p className="text-lg">
            Made with ❤️ by <a href="https://github.com/neeraj779">Neeraj</a>
          </p>
        </div>
      </footer>
    </div>
  );
}

export default LandingPage;
