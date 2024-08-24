import { createContext, useState, useRef, useEffect, useCallback } from "react";
import { useNavigate } from "react-router-dom";
import PropTypes from "prop-types";
import useApiClient from "../hooks/useApiClient";
import toast from "react-hot-toast";

const PlayerContext = createContext();

export const PlayerProvider = ({ children }) => {
  const swarApiClient = useApiClient();
  const songApiClient = useApiClient(true);
  const navigate = useNavigate();
  const [currentSong, setCurrentSong] = useState(null);
  const [isLoadingSong, setIsLoadingSong] = useState(true);
  const [isPlaying, setIsPlaying] = useState(false);
  const [duration, setDuration] = useState(0);
  const [currentTime, setCurrentTime] = useState(0);
  const [loop, setLoop] = useState(false);
  const [isEnded, setIsEnded] = useState(false);
  const [currentSongId, setCurrentSongId] = useState(null);
  const [suggestedSongs, setSuggestedSongs] = useState([]);
  const [suggestedSongIndex, setSuggestedSongIndex] = useState(-1);

  const audioRef = useRef(new Audio());

  const pauseSong = useCallback(() => {
    audioRef.current.pause();
    setIsPlaying(false);
  }, []);

  const playSong = useCallback(() => {
    audioRef.current.play().then(() => setIsPlaying(true));
  }, []);

  const seek = useCallback((time) => {
    audioRef.current.currentTime = time;
    setCurrentTime(audioRef.current.currentTime);
  }, []);

  const loadSong = useCallback(
    async (songId, isFromPlayer) => {
      if (currentSong?.id === songId) return;
      setIsLoadingSong(true);

      try {
        const { data: songData } = await songApiClient.get(
          `SongsData/GetSongById?id=${songId}&lyrics=true`
        );

        audioRef.current.src = songData.media_url;
        document.title = `${songData.song} | Swar`;
        setCurrentSong(songData);

        await new Promise(
          (resolve) => (audioRef.current.oncanplaythrough = resolve)
        );

        playSong();
        if (isFromPlayer) setCurrentSongId(songData.id);
        swarApiClient.post("PlayHistory/LogSongHistory", songData.id);

        if (
          isFromPlayer ||
          !suggestedSongs.length ||
          suggestedSongIndex === suggestedSongs.length - 2
        ) {
          try {
            const { data: suggestions } = await songApiClient.get(
              `SongsData/GetSongSuggestions?songId=${songId}`
            );

            if (suggestions.length) {
              if (isFromPlayer) {
                setSuggestedSongs(suggestions);
                setSuggestedSongIndex(-1);
              } else {
                setSuggestedSongs((prevSongs) => [
                  ...prevSongs,
                  ...suggestions,
                ]);
              }
            }
          } catch {
            setSuggestedSongs([]);
            setSuggestedSongIndex(-1);
          }
        }
      } catch {
        toast.error("Opps! We couldn't load the song.", {
          icon: "😥",
        });
        navigate("/", { replace: true });
      } finally {
        setIsLoadingSong(false);
      }
    },
    [
      currentSong?.id,
      playSong,
      swarApiClient,
      songApiClient,
      suggestedSongs.length,
      suggestedSongIndex,
      setSuggestedSongs,
      setSuggestedSongIndex,
      navigate,
    ]
  );

  const goToNextSong = useCallback(() => {
    if (isLoadingSong) return;
    if (suggestedSongs.length === 0) return;
    setSuggestedSongIndex((prevIndex) => {
      const nextIndex = (prevIndex + 1) % suggestedSongs.length;
      const nextSongId = suggestedSongs[nextIndex];
      if (nextSongId) {
        loadSong(nextSongId, false);
        return nextIndex;
      }
    });
  }, [isLoadingSong, suggestedSongs, loadSong]);

  const goToPreviousSong = useCallback(() => {
    if (isLoadingSong) return;
    if (suggestedSongs.length === 0) return;

    setSuggestedSongIndex((prevIndex) => {
      const newIndex =
        prevIndex > 0
          ? (prevIndex - 1 + suggestedSongs.length) % suggestedSongs.length
          : -1;

      const songId = newIndex >= 0 ? suggestedSongs[newIndex] : currentSongId;

      if (songId) loadSong(songId, false);
      return newIndex;
    });
  }, [currentSongId, isLoadingSong, loadSong, suggestedSongs]);

  useEffect(() => {
    if (isEnded && !loop) {
      goToNextSong();
      setIsEnded(false);
    }
  }, [goToNextSong, isEnded, loop]);

  const updateMediaSession = useCallback(
    (data) => {
      navigator.mediaSession.metadata = new MediaMetadata({
        title: data.song,
        artist: data.singers || data.primary_artists || "Unknown Artist",
        artwork: [{ src: data.image, sizes: "512x512", type: "image/png" }],
      });

      navigator.mediaSession.setActionHandler("play", playSong);
      navigator.mediaSession.setActionHandler("pause", pauseSong);
      navigator.mediaSession.setActionHandler(
        "previoustrack",
        goToPreviousSong
      );
      navigator.mediaSession.setActionHandler("nexttrack", goToNextSong);
    },
    [goToNextSong, goToPreviousSong, pauseSong, playSong]
  );

  useEffect(() => {
    if (currentSong && navigator.mediaSession) {
      updateMediaSession(currentSong);
    }
  }, [currentSong, updateMediaSession]);

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
    audio.addEventListener("play", () => setIsPlaying(true));
    audio.addEventListener("pause", () => setIsPlaying(false));
    audio.addEventListener("ended", handleEnded);

    return () => {
      audio.removeEventListener("timeupdate", handleTimeUpdate);
      audio.removeEventListener("loadedmetadata", handleLoadedMetadata);
      audio.removeEventListener("play", () => setIsPlaying(true));
      audio.removeEventListener("pause", () => setIsPlaying(false));
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
    goToNextSong,
    goToPreviousSong,
  };

  return (
    <PlayerContext.Provider value={value}>{children}</PlayerContext.Provider>
  );
};

PlayerProvider.propTypes = {
  children: PropTypes.node.isRequired,
};

export default PlayerContext;
