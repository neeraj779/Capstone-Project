const params = new URLSearchParams(window.location.search);
const platlistId = params.get("id");
const playlistName = params.get("name");

document.addEventListener("DOMContentLoaded", () => {
  document.getElementById("playlist-name").textContent = playlistName;
  fetchPlaylistSongs();
});

async function fetchPlaylistSongs() {
  try {
    let playlistSongs;
    if (platlistId === "liked") {
      let isLogged = await isLoggedin();
      if (!isLogged) {
        location.href = "login.html";
        return;
      }
      playlistSongs = await CRUDService.fetchAll("LikedSongs/GetLikedSongs");
    } else {
      playlistSongs = await CRUDService.fetchAll(
        `PlaylistSongs/GetAllSongsInUserPlaylist/${platlistId}`
      );
    }
    renderPlaylistSongs(playlistSongs || {});
    addDropdownEventListeners();
  } catch (error) {
    if (error.status === 404) {
      document.getElementById("error-message").classList.remove("hidden");
      document.getElementById("skeleton-loader").classList.add("hidden");
      return;
    }

    if (error.status === 403) {
      message.showAlert(
        "error",
        "Error",
        "You are not authorized to view this playlist. Redirecting to home page..."
      );
      setTimeout(() => {
        location.href = "index.html";
      }, 3000);
    } else location.href = "library.html";
  }
}

async function fetchSong(songId) {
  try {
    const response = await CRUDService.fetchAll(
      `SongsData/GetSongById?id=${songId}&lyrics=false`,
      true
    );
    return response;
  } catch (error) {
    console.error("Error fetching song:", error);
    return null;
  }
}

async function removeSong(element) {
  const songId = element.dataset.songid;
  let id = `${platlistId}/${songId}`;
  try {
    if (platlistId === "liked")
      await CRUDService.remove("LikedSongs/UnlikeSong", songId);
    else await CRUDService.remove("PlaylistSongs/RemoveSongFromPlaylist", id);
    message.showSuccessToast("Removed", "Song removed from playlist");
    document.getElementById(songId).remove();
  } catch (error) {
    message.showAlert("error", "Error", error.message);
  }
}

function createPlaylistSongsCard(playlistSong) {
  return `
      <div id = ${playlistSong.id} class="relative flex items-center rounded-lg p-1 hover:shadow-xl transition-shadow duration-300 hover:bg-gray-800">
        <a href="songPlayer.html?id=${playlistSong.id}">
          <img src="${playlistSong.image}" alt="Album Cover" class="ml-3 w-12 h-12 object-cover rounded-md border border-gray-700" />
        </a>
        <div class="ml-6 flex flex-col justify-center flex-grow">
          <div class="flex items-center">
            <a href="songPlayer.html?id=${playlistSong.id}">
              <h2 class="text-mb font-semibold mb-1">${playlistSong.song}</h2>
            </a>
          </div>
          <p class="text-gray-400 text-sm mb-2">${playlistSong.primary_artists}</p>
        </div>
        <div class="flex items-center space-x-10 mr-6 relative">
          <div class="dropdown-button-container">
            <button class="text-gray-400 hover:text-gray-600 focus:outline-none" aria-haspopup="true" aria-expanded="false">
              <i class="fas fa-ellipsis-v text-xl"></i>
            </button>
            <div class="dropdown-content">
              <a href="#" data-songId="${playlistSong.id}" onclick="removeSong(this)" class="text-red-500 hover:text-red-700 flex items-center px-4 py-2">
                <i class="fas fa-trash-alt mr-2"></i>Remove
              </a>
            </div>
          </div>
        </div>
      </div>`;
}

async function renderPlaylistSongs(playlistSongs) {
  const skeletonLoader = document.getElementById("skeleton-loader");
  const container = document.getElementById("playlistSongs-container");
  const errorContainer = document.getElementById("error-message");

  if (!playlistSongs.songs || playlistSongs.songs.length === 0) {
    errorContainer.classList.remove("hidden");
    skeletonLoader.classList.add("hidden");
    return;
  }

  container.innerHTML = "";
  errorContainer.classList.add("hidden");
  container.classList.remove("hidden");
  const fragment = document.createDocumentFragment();

  for (const songId of playlistSongs.songs) {
    const songData = await fetchSong(songId);
    if (songData) {
      const card = createPlaylistSongsCard(songData);
      const tempDiv = document.createElement("div");
      tempDiv.innerHTML = card;
      fragment.appendChild(tempDiv.firstElementChild);
    }
  }

  container.appendChild(fragment);
  skeletonLoader.classList.add("hidden");
}

function addDropdownEventListeners() {
  document.addEventListener("click", (event) => {
    const isDropdownButton = event.target.closest(".dropdown-button-container");
    if (isDropdownButton) {
      const dropdownContent =
        isDropdownButton.querySelector(".dropdown-content");
      const isVisible = dropdownContent.classList.contains("show");
      document
        .querySelectorAll(".dropdown-content")
        .forEach((content) => content.classList.remove("show"));
      dropdownContent.classList.toggle("show", !isVisible);
      isDropdownButton
        .querySelector("button")
        .setAttribute("aria-expanded", !isVisible);
    } else {
      document
        .querySelectorAll(".dropdown-content")
        .forEach((content) => content.classList.remove("show"));
      document
        .querySelectorAll(".dropdown-button-container button")
        .forEach((button) => button.setAttribute("aria-expanded", "false"));
    }
  });
}

function toggleLike(button) {
  button.classList.toggle("text-red-500");
}

async function isLoggedin() {
  const accessToken = localStorage.getItem("accessToken");
  try {
    const response = await fetch(
      "https://swarapi.azurewebsites.net/api/v1/Auth/VerifyToken",
      {
        method: "GET",
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
      }
    );

    return response.ok;
  } catch (error) {
    return false;
  }
}

document.getElementById("hamburger")?.addEventListener("click", () => {
  document.getElementById("mobile-menu")?.classList.toggle("hidden");
});
