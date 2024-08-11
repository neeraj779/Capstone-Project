import { createContext, useState, useRef, useEffect, useCallback } from "react";
import PropTypes from "prop-types";

const PlayerContext = createContext();

export const PlayerProvider = ({ children }) => {
  const [currentSong, setCurrentSong] = useState(null);
  const [isPlaying, setIsPlaying] = useState(false);
  const [duration, setDuration] = useState(0);
  const [currentTime, setCurrentTime] = useState(0);
  const [loop, setLoop] = useState(false);

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
      }
      playSong();
    },
    [currentSong, playSong, updateMediaSession]
  );

  const resetPlayer = useCallback(() => {
    pauseSong();
    audioRef.current.currentTime = 0;
    setCurrentSong(null);
    setCurrentTime(0);
  }, [pauseSong]);

  const togglePlayPause = useCallback(() => {
    isPlaying ? pauseSong() : playSong();
  }, [isPlaying, pauseSong, playSong]);

  useEffect(() => {
    const audio = audioRef.current;

    const handleTimeUpdate = () => setCurrentTime(audio.currentTime);
    const handleLoadedMetadata = () => setDuration(audio.duration);
    const handleEnded = () => setIsPlaying(false);

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
