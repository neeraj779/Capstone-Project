document.addEventListener("DOMContentLoaded", async () => {
  const params = new URLSearchParams(window.location.search);
  const songId = params.get("id");
  const audio = document.getElementById("audio");
  const playPauseBtn = document.getElementById("play-pause-btn");
  const playIcon = document.getElementById("play-icon");
  const pauseIcon = document.getElementById("pause-icon");
  const slider = document.getElementById("slider");
  const currentTimeEl = document.getElementById("current-time");
  const durationEl = document.getElementById("duration");
  const loopBtn = document.getElementById("loop-btn");
  const backwardBtn = document.getElementById("backward-btn");
  const forwardBtn = document.getElementById("forward-btn");
  const downloadBtn = document.getElementById("download-btn");
  const lyricsContainer = document.getElementById("lyrics-container");

  let isPlaying = false;

  async function getSong() {
    try {
      const data = await CRUDService.fetchAll(
        `SongsData/GetSongById?id=${songId}&lyrics=true`,
        true
      );
      if (!data) throw new Error("Song not found");
      document.title = data.song;
      audio.src = data.media_url;
      document.getElementById("song-title").textContent = data.song;
      document.getElementById("song-artist").textContent =
        data.singers || data.primary_artists || "Unknown Artist";
      document.getElementById("song-image").src = data.image;
      document.getElementById("song-lyrics").innerHTML =
        data.lyrics || lyricsContainer.classList.add("hidden");
      audio.addEventListener("loadedmetadata", () => {
        slider.max = audio.duration;
        durationEl.textContent = formatTime(audio.duration);
        updateMediaSession(data);
        audio
          .play()
          .then(() => {
            isPlaying = true;
            updatePlayPauseUI();
            updateMediaSessionPlaybackState();
          })
          .catch((err) => {
            console.error("Failed to play audio:", err);
          });
      });
      document.getElementById("skeleton-loader").classList.add("hidden");
      document.getElementById("content").classList.remove("hidden");
      logSongHistory(songId);
    } catch (error) {
      location.href = "index.html";
    }
  }

  function formatTime(seconds) {
    const minutes = String(Math.floor(seconds / 60)).padStart(2, "0");
    const secs = String(Math.floor(seconds % 60)).padStart(2, "0");
    return `${minutes}:${secs}`;
  }

  function togglePlayPause() {
    if (audio.paused) {
      audio.play();
    } else {
      audio.pause();
    }
    updatePlayPauseUI();
    updateMediaSessionPlaybackState();
  }

  function updatePlayPauseUI() {
    if (audio.paused) {
      playIcon.classList.remove("hidden");
      pauseIcon.classList.add("hidden");
    } else {
      playIcon.classList.add("hidden");
      pauseIcon.classList.remove("hidden");
    }
    isPlaying = !audio.paused;
  }

  function handleTimeUpdate() {
    slider.value = audio.currentTime;
    currentTimeEl.textContent = formatTime(audio.currentTime);
  }

  function handleSeek() {
    audio.currentTime = slider.value;
    updateMediaSessionPositionState();
  }

  function toggleLoop() {
    audio.loop = !audio.loop;
    loopBtn.classList.toggle("bg-blue-700", audio.loop);
  }

  function seek(seconds) {
    audio.currentTime += seconds;
    updateMediaSessionPositionState();
  }

  async function downloadSong() {
    try {
      const response = await fetch(audio.src);
      if (!response.ok) throw new Error("Failed to download song");
      const blob = await response.blob();
      const url = URL.createObjectURL(blob);
      const a = document.createElement("a");
      a.href = url;
      a.download = `${document.getElementById("song-title").textContent}.mp3`;
      a.click();
      URL.revokeObjectURL(url);
      message.showToast("Song downloaded successfully");
    } catch (error) {
      console.error(error);
    }
  }

  async function logSongHistory(songId) {
    try {
      await CRUDService.create("PlayHistory/LogSongHistory", songId);
    } catch (error) {
      console.error(error);
    }
  }

  function updateMediaSession(data) {
    if ("mediaSession" in navigator) {
      navigator.mediaSession.metadata = new MediaMetadata({
        title: data.song,
        artist: data.singers || data.primary_artists || "Unknown Artist",
        artwork: [{ src: data.image, sizes: "500x500", type: "image/png" }],
      });

      navigator.mediaSession.setActionHandler("play", () => {
        togglePlayPause();
      });
      navigator.mediaSession.setActionHandler("pause", () => {
        togglePlayPause();
      });
      navigator.mediaSession.setActionHandler("seekbackward", () => {
        seek(-10);
      });
      navigator.mediaSession.setActionHandler("seekforward", () => {
        seek(10);
      });
      navigator.mediaSession.setActionHandler("previoustrack", () => {
        seek(-10);
      });
      navigator.mediaSession.setActionHandler("nexttrack", () => {
        seek(10);
      });
    }
  }

  function updateMediaSessionPlaybackState() {
    if ("mediaSession" in navigator) {
      navigator.mediaSession.playbackState = audio.paused
        ? "paused"
        : "playing";
    }
  }

  function updateMediaSessionPositionState() {
    if ("mediaSession" in navigator) {
      navigator.mediaSession.setPositionState({
        duration: audio.duration,
        playbackRate: audio.playbackRate,
        position: audio.currentTime,
      });
    }
  }

  audio.addEventListener("timeupdate", handleTimeUpdate);
  audio.addEventListener("ended", () => {
    isPlaying = false;
    updatePlayPauseUI();
    updateMediaSessionPlaybackState();
  });
  audio.addEventListener("pause", () => {
    isPlaying = false;
    updatePlayPauseUI();
    updateMediaSessionPlaybackState();
  });
  audio.addEventListener("play", () => {
    isPlaying = true;
    updatePlayPauseUI();
    updateMediaSessionPlaybackState();
  });
  slider.addEventListener("input", handleSeek);
  playPauseBtn.addEventListener("click", togglePlayPause);
  loopBtn.addEventListener("click", toggleLoop);
  backwardBtn.addEventListener("click", () => seek(-10));
  forwardBtn.addEventListener("click", () => seek(10));
  downloadBtn.addEventListener("click", downloadSong);

  await checkIfLiked(songId);
  document
    .getElementById("like-checkbox")
    .addEventListener("change", toggleLike);
  await getSong();
});
