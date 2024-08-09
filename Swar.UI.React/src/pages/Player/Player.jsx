import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import Swal from "sweetalert2";
import { fetchAll, create } from "../../services/CRUDService";
import MobileSearchBar from "../../components/MobileSearchBar";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faCirclePlus,
  faRepeat,
  faRotateRight,
  faPlay,
  faPause,
  faRotateLeft,
  faDownload,
} from "@fortawesome/free-solid-svg-icons";
import usePlayer from "../../hooks/usePlayer";
import "./player.css";

const SongPlayer = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const {
    currentSong,
    isPlaying,
    duration,
    currentTime,
    loop,
    setLoop,
    playSong,
    togglePlayPause,
    seek,
  } = usePlayer();
  const [song, setSong] = useState(null);
  const [sliderValue, setSliderValue] = useState(0);

  useEffect(() => {
    const fetchSong = async () => {
      if (currentSong?.id === id) {
        setSong(currentSong);
        document.title = currentSong.song;
        return;
      }

      try {
        const data = await fetchAll(
          `SongsData/GetSongById?id=${id}&lyrics=true`,
          true
        );
        if (!data) throw new Error("Song not found");
        document.title = data.song;
        setSong(data);
        if (currentSong?.id !== data.id) {
          playSong(data);
          await logSongHistory(id);
        }
      } catch (error) {
        console.error(error);
        navigate("/");
      }
    };

    const logSongHistory = async (songId) => {
      try {
        await create("PlayHistory/LogSongHistory", songId);
      } catch (error) {
        console.error("Failed to log song history:", error);
      }
    };

    fetchSong();
  }, [id, navigate, playSong, currentSong?.id]);

  useEffect(() => {
    setSliderValue(currentTime);
  }, [currentTime]);

  const handleSeek = (e) => {
    const newValue = Number(e.target.value);
    seek(newValue);
    setSliderValue(newValue);
  };

  const formatTime = (seconds) => {
    const minutes = String(Math.floor(seconds / 60)).padStart(2, "0");
    const secs = String(Math.floor(seconds % 60)).padStart(2, "0");
    return `${minutes}:${secs}`;
  };

  const toggleLoop = () => setLoop((prev) => !prev);

  const downloadSong = async () => {
    try {
      const response = await fetch(currentSong.media_url);
      if (!response.ok) throw new Error("Failed to download song");
      const blob = await response.blob();
      const url = URL.createObjectURL(blob);
      const a = document.createElement("a");
      a.href = url;
      a.download = `${song.song}.mp3`;
      a.click();
      URL.revokeObjectURL(url);
      Swal.fire("Success", "Song downloaded successfully", "success");
    } catch (error) {
      console.error("Download failed:", error);
      Swal.fire("Error", "Failed to download song", "error");
    }
  };

  if (!song) {
    return (
      <div className="mb-3 mt-5">
        <MobileSearchBar />
        <div id="skeleton-loader" className="grid gap-6 px-6">
          <div className="grid text-center place-content-center gap-3">
            <div className="h-52 w-52 bg-gray-700 rounded-full mx-auto animate-pulse"></div>
            <div className="h-6 bg-gray-700 rounded mx-auto w-3/4 animate-pulse"></div>
            <div className="h-4 bg-gray-700 rounded mx-auto w-1/2 animate-pulse"></div>
          </div>
          <div className="w-full max-w-[400px] mx-auto">
            <div className="h-2 bg-gray-700 rounded animate-pulse mb-2"></div>
          </div>

          <div className="flex items-center justify-center gap-4">
            <div className="h-10 w-10 bg-gray-700 rounded-full animate-pulse"></div>
            <div className="flex items-center justify-center gap-2">
              <div className="h-10 w-10 bg-gray-700 rounded-full animate-pulse"></div>
              <div className="h-10 w-10 bg-gray-700 rounded-full animate-pulse"></div>
              <div className="h-10 w-10 bg-gray-700 rounded-full animate-pulse"></div>
            </div>
            <div className="h-10 w-10 bg-gray-700 rounded-full animate-pulse"></div>
          </div>
          <div className="lyrics-container">
            <h2 className="text-xl font-semibold mb-2">Lyrics</h2>
            <div className="space-y-2">
              <div className="h-4 bg-gray-700 rounded animate-pulse"></div>
              <div className="h-4 bg-gray-700 rounded animate-pulse"></div>
              <div className="h-4 bg-gray-700 rounded animate-pulse"></div>
              <div className="h-4 bg-gray-700 rounded animate-pulse"></div>
            </div>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="mb-3 mt-5">
      <MobileSearchBar />
      <div id="content">
        <div className="grid gap-6 px-6">
          <div className="grid text-center place-content-center gap-3">
            <img
              src={song.image}
              alt={song.song}
              className="h-52 w-52 rounded-full object-cover mx-auto"
            />
            <h1 className="text-lg mx-auto font-bold md:max-w-lg max-w-[260px]">
              {song.song}
            </h1>
            <p className="text-xs -mt-2 max-w-xl mx-auto">{song.artists}</p>
          </div>
          <div className="-mt-3 -mb-3 w-full max-w-[400px] mx-auto flex items-center justify-between">
            <div className="heart-container" title="Like">
              <input type="checkbox" className="checkbox" id="like-checkbox" />
              <div className="svg-container">
                <svg
                  viewBox="0 0 24 24"
                  className="svg-outline"
                  xmlns="http://www.w3.org/2000/svg"
                >
                  <path d="M17.5,1.917a6.4,6.4,0,0,0-5.5,3.3,6.4,6.4,0,0,0-5.5-3.3A6.8,6.8,0,0,0,0,8.967c0,4.547,4.786,9.513,8.8,12.88a4.974,4.974,0,0,0,6.4,0C19.214,18.48,24,13.514,24,8.967A6.8,6.8,0,0,0,17.5,1.917Zm-3.585,18.4a2.973,2.973,0,0,1-3.83,0C4.947,16.006,2,11.87,2,8.967a4.8,4.8,0,0,1,4.5-5.05A4.8,4.8,0,0,1,11,8.967a1,1,0,0,0,2,0,4.8,4.8,0,0,1,4.5-5.05A4.8,4.8,0,0,1,22,8.967C22,11.87,19.053,16.006,13.915,20.313Z"></path>
                </svg>
                <svg
                  viewBox="0 0 24 24"
                  className="svg-filled"
                  xmlns="http://www.w3.org/2000/svg"
                >
                  <path d="M17.5,1.917a6.4,6.4,0,0,0-5.5,3.3,6.4,6.4,0,0,0-5.5-3.3A6.8,6.8,0,0,0,0,8.967c0,4.547,4.786,9.513,8.8,12.88a4.974,4.974,0,0,0,6.4,0C19.214,18.48,24,13.514,24,8.967A6.8,6.8,0,0,0,17.5,1.917Z"></path>
                </svg>
                <svg
                  className="svg-celebrate"
                  width="100"
                  height="100"
                  xmlns="http://www.w3.org/2000/svg"
                >
                  <polygon points="10,10 20,20"></polygon>
                  <polygon points="10,50 20,50"></polygon>
                  <polygon points="20,80 30,70"></polygon>
                  <polygon points="90,10 80,20"></polygon>
                  <polygon points="90,50 80,50"></polygon>
                  <polygon points="80,80 70,70"></polygon>
                </svg>
              </div>
            </div>
            <button
              id="add-to-playlist-btn"
              className="text-gray-400 hover:text-blue-500 focus:outline-none"
            >
              <FontAwesomeIcon icon={faCirclePlus} size="2x" />
            </button>
          </div>
          <input
            type="range"
            id="slider"
            className="w-full max-w-[400px] mx-auto"
            min="0"
            max={duration}
            step="1"
            value={sliderValue}
            onChange={handleSeek}
          />
          <div className="-mt-6 -mb-3 w-full max-w-[400px] mx-auto flex items-center justify-between">
            <span className="text-xs">{formatTime(currentTime)}</span>
            <span className="text-xs">{formatTime(duration)}</span>
          </div>
          <div className="flex items-center justify-center gap-4">
            <button
              id="loop-btn"
              onClick={toggleLoop}
              className={`icon-button text-white p-4 rounded-lg focus:outline-none focus:ring-2 focus:ring-gray-500 ${
                loop ? "bg-blue-700" : "bg-gray-700"
              }`}
            >
              <FontAwesomeIcon icon={faRepeat} />
            </button>
            <div className="flex items-center justify-center gap-2">
              <button
                id="forward-btn"
                onClick={() => seek(10)}
                className="icon-button bg-gray-700 text-white p-4 rounded-lg focus:outline-none focus:ring-2 focus:ring-gray-500"
              >
                <FontAwesomeIcon icon={faRotateRight} />
              </button>
              <button
                id="play-pause-btn"
                onClick={togglePlayPause}
                className="icon-button bg-gray-700 text-white p-4 rounded-lg focus:outline-none focus:ring-2 focus:ring-gray-500"
              >
                <FontAwesomeIcon icon={isPlaying ? faPause : faPlay} />
              </button>
              <button
                id="backward-btn"
                onClick={() => seek(-10)}
                className="icon-button bg-gray-700 text-white p-4 rounded-lg focus:outline-none focus:ring-2 focus:ring-gray-500"
              >
                <FontAwesomeIcon icon={faRotateLeft} />
              </button>
            </div>
            <button
              id="download-btn"
              onClick={downloadSong}
              className="bg-gray-700 hover:bg-gray-600 text-white p-4 rounded-lg focus:outline-none focus:ring-2 focus:ring-gray-500"
            >
              <FontAwesomeIcon icon={faDownload} size="lg" />
            </button>
          </div>
          <div id="lyrics-container" className="lyrics-container">
            <h2 className="text-xl font-semibold mb-2">Lyrics</h2>
            <p
              id="song-lyrics"
              className="text-sm whitespace-pre-line"
              dangerouslySetInnerHTML={{ __html: song.lyrics }}
            ></p>
          </div>
        </div>
      </div>
    </div>
  );
};

export default SongPlayer;
