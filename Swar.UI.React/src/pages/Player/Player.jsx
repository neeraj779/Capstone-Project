import { useEffect, useState, useCallback, useMemo } from "react";
import { useParams, useNavigate } from "react-router-dom";

import Swal from "sweetalert2";
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

import { fetchAll, create } from "../../services/CRUDService";
import SearchBar from "../../components/SearchBar";
import PlayerSkeleton from "../../components/PlayerSkeleton";
import LikeButton from "../../components/LikeButton/LikeButton";
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
  const [loading, setLoading] = useState(true);

  const fetchSong = useCallback(async () => {
    setLoading(true);
    if (currentSong?.id === id) {
      setSong(currentSong);
      document.title = currentSong.song;
      setLoading(false);
      return;
    }

    try {
      const data = await fetchAll(
        `SongsData/GetSongById?id=${id}&lyrics=true`,
        true
      );
      document.title = data.song;
      setSong(data);
      if (currentSong?.id !== data.id) {
        playSong(data);
        await logSongHistory(id);
      }
    } catch (error) {
      console.error(error);
      navigate("/");
    } finally {
      setLoading(false);
    }
  }, [id, navigate, playSong, currentSong]);

  const logSongHistory = async (songId) => {
    try {
      await create("PlayHistory/LogSongHistory", songId);
    } catch (error) {
      console.error("Failed to log song history:", error);
    }
  };

  useEffect(() => {
    fetchSong();
  }, [fetchSong]);

  useEffect(() => {
    setSliderValue(currentTime);
  }, [currentTime]);

  const handleSeek = (e) => {
    const newValue = Number(e.target.value);
    seek(newValue);
    setSliderValue(newValue);
  };

  const formatTime = useMemo(
    () => (seconds) => {
      const minutes = String(Math.floor(seconds / 60)).padStart(2, "0");
      const secs = String(Math.floor(seconds % 60)).padStart(2, "0");
      return `${minutes}:${secs}`;
    },
    []
  );

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

  if (loading) return <PlayerSkeleton />;

  return (
    <div className="mb-3 mt-5">
      <div className="block md:hidden mb-6 mx-6">
        <SearchBar />
      </div>
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
            <p className="text-xs -mt-2 max-w-xl mx-auto">
              {song.singers || song.primary_artists || "Unknown Artist"}
            </p>
          </div>
          <div className="-mt-3 -mb-3 w-full max-w-[400px] mx-auto flex items-center justify-between">
            <LikeButton songId={song.id} />
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
          <div id="lyrics-container" className="lyrics-container mb-16">
            <h2 className="text-xl font-semibold mb-2">Lyrics</h2>
            <p
              id="song-lyrics"
              className="text-sm whitespace-pre-line"
              dangerouslySetInnerHTML={{ __html: song.lyrics }}
            />
          </div>
        </div>
      </div>
    </div>
  );
};

export default SongPlayer;
