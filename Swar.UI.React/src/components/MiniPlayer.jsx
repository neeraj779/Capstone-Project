import { useNavigate, useMatch } from "react-router-dom";
import usePlayer from "../hooks/usePlayer";
import { useAuth0 } from "@auth0/auth0-react";
import { FaPlay, FaPause } from "react-icons/fa";

const MiniPlayer = () => {
  const { isAuthenticated } = useAuth0();
  const { currentSong, isPlaying, togglePlayPause } = usePlayer();
  const isSongPlayerPage = useMatch("/player");
  const navigate = useNavigate();

  const handleCardClick = () => {
    if (currentSong) {
      navigate("/player", { state: { songId: currentSong.id } });
    }
  };

  if (!isAuthenticated || !currentSong || isSongPlayerPage) return null;

  return (
    <div className="fixed bottom-14 mb-1 md:bottom-0 left-1 right-1 bg-gray-800 text-white flex items-center p-2 shadow-lg border-t border-gray-700 rounded-lg z-20">
      <div
        className="flex items-center space-x-2 flex-grow cursor-pointer"
        onClick={handleCardClick}
      >
        <img
          src={currentSong.image}
          alt={currentSong.song}
          className="w-10 h-10 object-cover rounded-lg shadow-md"
        />
        <div className="flex flex-col justify-center overflow-hidden max-w-[calc(100% - 3rem)]">
          <div className="relative w-40 sm:w-48 overflow-hidden">
            <p className="font-bold truncate">{currentSong.song}</p>
            <p className="text-sm text-gray-300 truncate">
              {currentSong.singers ||
                currentSong.primary_artists ||
                "Unknown Artist"}
            </p>
          </div>
        </div>
      </div>
      <button
        onClick={togglePlayPause}
        className="flex items-center justify-center w-10 h-10 sm:w-12 sm:h-12 ml-1 sm:ml-2 mr-3 bg-gray-700 text-white rounded-full shadow-md transition-transform transform hover:scale-105"
        title={isPlaying ? "Pause" : "Play"}
      >
        {isPlaying ? <FaPause /> : <FaPlay />}
      </button>
    </div>
  );
};

export default MiniPlayer;
