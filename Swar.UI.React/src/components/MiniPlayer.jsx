import { useNavigate, useMatch } from "react-router-dom";
import usePlayer from "../hooks/usePlayer";
import { useAuth0 } from "@auth0/auth0-react";
import { FaPlay, FaPause } from "react-icons/fa";
import { Progress } from "@nextui-org/react";

const MiniPlayer = () => {
  const { isAuthenticated } = useAuth0();
  const {
    currentSong,
    isPlaying,
    togglePlayPause,
    duration,
    currentTime,
    isLoadingSong,
  } = usePlayer();
  const isSongPlayerPage = useMatch("/player");
  const isLandingPage = useMatch("/");
  const navigate = useNavigate();

  const handleCardClick = () => {
    if (currentSong) {
      navigate("/player", { state: { songId: currentSong.id } });
    }
  };

  if (!isAuthenticated || !currentSong || isSongPlayerPage || isLandingPage)
    return null;

  const progressPercentage = duration ? (currentTime / duration) * 100 : 0;

  return (
    <>
      <div className="fixed left-0 right-0 z-20 flex items-center p-2 mb-1 text-white bg-gray-800 border-t border-gray-700 shadow-lg bottom-14 md:bottom-0">
        <div
          className="flex items-center flex-grow space-x-2 cursor-pointer"
          onClick={handleCardClick}
        >
          <img
            src={currentSong.image}
            alt={currentSong.song}
            className="object-cover w-10 h-10 rounded-lg shadow-md"
          />
          <div className="flex flex-col justify-center overflow-hidden max-w-[calc(100% - 3rem)]">
            <div className="relative w-48 overflow-hidden md:w-96">
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
          className="flex items-center justify-center w-10 h-10 ml-1 mr-3 text-white transition-transform transform bg-gray-700 rounded-full shadow-md sm:w-12 sm:h-12 sm:ml-2 hover:scale-105"
          title={isPlaying ? "Pause" : "Play"}
        >
          {isPlaying ? <FaPause /> : <FaPlay />}
        </button>
      </div>
      <Progress
        size="sm"
        isIndeterminate={isLoadingSong}
        aria-label="Song progress"
        className="fixed left-0 right-0 z-20 bg-gray-800 bottom-14 md:bottom-0"
        value={progressPercentage}
        classNames={{
          indicator: "bg-white",
        }}
      />
    </>
  );
};

export default MiniPlayer;
