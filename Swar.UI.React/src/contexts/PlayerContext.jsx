import { createContext, useState, useRef, useEffect, useCallback } from "react";

const PlayerContext = createContext();

// eslint-disable-next-line react/prop-types
export const PlayerProvider = ({ children }) => {
  const [currentSong, setCurrentSong] = useState(null);
  const [isPlaying, setIsPlaying] = useState(false);
  const [duration, setDuration] = useState(0);
  const [currentTime, setCurrentTime] = useState(0);
  const [loop, setLoop] = useState(false);

  const audioRef = useRef(new Audio());

  const playSong = useCallback(
    async (song) => {
      if (currentSong?.id !== song.id) {
        audioRef.current.src = song.media_url;
        setCurrentSong(song);

        await new Promise((resolve) => {
          audioRef.current.oncanplaythrough = resolve;
        });
      }

      try {
        await audioRef.current.play();
        setIsPlaying(true);
      } catch (error) {
        console.error(error);
      }
    },
    [currentSong]
  );

  const pauseSong = useCallback(() => {
    audioRef.current.pause();
    setIsPlaying(false);
  }, []);

  const togglePlayPause = useCallback(() => {
    isPlaying ? pauseSong() : playSong(currentSong);
  }, [isPlaying, pauseSong, playSong, currentSong]);

  const seek = useCallback((time) => {
    audioRef.current.currentTime = time;
    setCurrentTime(time);
  }, []);

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
    setLoop,
    playSong,
    pauseSong,
    togglePlayPause,
    seek,
  };

  return (
    <PlayerContext.Provider value={value}>{children}</PlayerContext.Provider>
  );
};

export default PlayerContext;
