import { createContext, useState, useRef, useEffect, useCallback } from "react";
import { useNavigate } from "react-router-dom";
import PropTypes from "prop-types";
import useApiClient from "../hooks/useApiClient";

const PlayerContext = createContext();

export const PlayerProvider = ({ children }) => {
  const songApiClient = useApiClient(true);
  const navigate = useNavigate();
  const [currentSong, setCurrentSong] = useState(null);
  const [isPlaying, setIsPlaying] = useState(false);
  const [duration, setDuration] = useState(0);
  const [currentTime, setCurrentTime] = useState(0);
  const [loop, setLoop] = useState(false);
  const [isEnded, setIsEnded] = useState(false);
  const [suggestedSongs, setSuggestedSongs] = useState([]);
  const [suggestedSongIndex, setSuggestedSongIndex] = useState(0);

  const audioRef = useRef(new Audio());

  const pauseSong = useCallback(() => {
    audioRef.current.pause();
    setIsPlaying(false);
  }, []);

  const playSong = useCallback(() => {
    audioRef.current
      .play()
      .then(() => setIsPlaying(true))
      .catch(console.error);
  }, []);

  const seek = useCallback((time, isSeek = true) => {
    if (isSeek) {
      audioRef.current.currentTime += time;
    } else {
      audioRef.current.currentTime = time;
    }
    setCurrentTime(audioRef.current.currentTime);
  }, []);

  const updateMediaSession = useCallback(
    (data) => {
      if (navigator.mediaSession) {
        navigator.mediaSession.metadata = new MediaMetadata({
          title: data.song,
          artist: data.singers || data.primary_artists || "Unknown Artist",
          artwork: [{ src: data.image, sizes: "500x500", type: "image/png" }],
        });

        navigator.mediaSession.setActionHandler("play", playSong);
        navigator.mediaSession.setActionHandler("pause", pauseSong);
        navigator.mediaSession.setActionHandler("seekbackward", () =>
          seek(-10)
        );
        navigator.mediaSession.setActionHandler("seekforward", () => seek(10));
        navigator.mediaSession.setActionHandler("previoustrack", () =>
          seek(-10)
        );
        navigator.mediaSession.setActionHandler("nexttrack", () => seek(10));
      }
    },
    [playSong, pauseSong, seek]
  );

  const loadSong = useCallback(
    async (song) => {
      if (currentSong?.id !== song.id) {
        audioRef.current.src = song.media_url;
        setCurrentSong(song);
        await new Promise((resolve) => {
          audioRef.current.oncanplaythrough = resolve;
        });
        updateMediaSession(song);
        if (suggestedSongIndex === 9 || suggestedSongs.length === 0) {
          const { data } = await songApiClient.get(
            `SongsData/GetSongSuggestions?songId=${song.id}`
          );
          console.log(suggestedSongIndex);
          setSuggestedSongs(data);
          setSuggestedSongIndex(0);
        }
        setIsEnded(false);
      }
      playSong();
    },
    [
      currentSong,
      playSong,
      updateMediaSession,
      songApiClient,
      setSuggestedSongs,
      suggestedSongIndex,
    ]
  );

  const resetPlayer = useCallback(() => {
    pauseSong();
    audioRef.current.currentTime = 0;
    setCurrentSong(null);
    setCurrentTime(0);
    setIsEnded(false);
  }, [pauseSong]);

  const togglePlayPause = useCallback(() => {
    isPlaying ? pauseSong() : playSong();
  }, [isPlaying, pauseSong, playSong]);

  useEffect(() => {
    const audio = audioRef.current;

    const handleTimeUpdate = () => setCurrentTime(audio.currentTime);
    const handleLoadedMetadata = () => setDuration(audio.duration);
    const handleEnded = () => {
      setIsPlaying(false);
      setIsEnded(true);
    };

    audio.addEventListener("timeupdate", handleTimeUpdate);
    audio.addEventListener("loadedmetadata", handleLoadedMetadata);
    audio.addEventListener("ended", handleEnded);

    return () => {
      audio.removeEventListener("timeupdate", handleTimeUpdate);
      audio.removeEventListener("loadedmetadata", handleLoadedMetadata);
      audio.removeEventListener("ended", handleEnded);
    };
  }, []);

  useEffect(() => {
    if (isEnded && !loop) {
      const nextIndex = (suggestedSongIndex + 1) % suggestedSongs.length;
      const nextSongId = suggestedSongs[nextIndex];
      setSuggestedSongIndex(nextIndex);

      navigate(`/song/${nextSongId}`);
    }
  }, [isEnded]);

  useEffect(() => {
    audioRef.current.loop = loop;
  }, [loop]);

  const value = {
    currentSong,
    isPlaying,
    duration,
    currentTime,
    loop,
    resetPlayer,
    setLoop,
    loadSong,
    pauseSong,
    togglePlayPause,
    seek,
  };

  return (
    <PlayerContext.Provider value={value}>{children}</PlayerContext.Provider>
  );
};

PlayerProvider.propTypes = {
  children: PropTypes.node.isRequired,
};

export default PlayerContext;
