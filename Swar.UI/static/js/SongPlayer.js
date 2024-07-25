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

  let isPlaying = false;

  async function getSong() {
    try {
      const response = await fetch(
        `http://localhost:5292/api/SongsData/GetSongById?id=${songId}&lyrics=true`
      );
      if (!response.ok) throw new Error("Failed to fetch song data");
      const data = await response.json();
      audio.src = data.media_url;
      document.getElementById("song-title").textContent = data.song;
      document.getElementById("song-artist").textContent =
        data.singers || "Unknown";
      document.getElementById("song-image").src = data.image;
      document.getElementById("song-lyrics").innerHTML =
        data.lyrics || "Lyrics not available";
      audio.addEventListener("loadedmetadata", () => {
        slider.max = audio.duration;
        durationEl.textContent = formatTime(audio.duration);
      });
    } catch (error) {
      console.error(error);
    }
  }

  function formatTime(seconds) {
    const minutes = String(Math.floor(seconds / 60)).padStart(2, "0");
    const secs = String(Math.floor(seconds % 60)).padStart(2, "0");
    return `${minutes}:${secs}`;
  }

  function togglePlayPause() {
    if (isPlaying) {
      audio.pause();
      playIcon.classList.remove("hidden");
      pauseIcon.classList.add("hidden");
    } else {
      audio.play();
      playIcon.classList.add("hidden");
      pauseIcon.classList.remove("hidden");
    }
    isPlaying = !isPlaying;
  }

  function handleTimeUpdate() {
    slider.value = audio.currentTime;
    currentTimeEl.textContent = formatTime(audio.currentTime);
  }

  function handleSeek() {
    audio.currentTime = slider.value;
  }

  function toggleLoop() {
    audio.loop = !audio.loop;
    loopBtn.classList.toggle("bg-blue-700", audio.loop);
  }

  function seek(seconds) {
    audio.currentTime += seconds;
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
    } catch (error) {
      console.error(error);
    }
  }

  audio.addEventListener("timeupdate", handleTimeUpdate);
  audio.addEventListener("ended", () => {
    playing = false;
    togglePlayPause();
  });
  slider.addEventListener("input", handleSeek);
  playPauseBtn.addEventListener("click", togglePlayPause);
  loopBtn.addEventListener("click", toggleLoop);
  backwardBtn.addEventListener("click", () => seek(-10));
  forwardBtn.addEventListener("click", () => seek(10));
  downloadBtn.addEventListener("click", downloadSong);

  await getSong();
});
