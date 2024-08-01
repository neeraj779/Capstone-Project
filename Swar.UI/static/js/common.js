async function toggleLike(event) {
  const isChecked = event.target.checked;
  const songId = event.target.getAttribute("data-song-id");
  try {
    if (isChecked) {
      await CRUDService.create("LikedSongs/LikeSong", songId);
    } else {
      await CRUDService.remove("LikedSongs/UnlikeSong", songId);
    }
  } catch (error) {
    console.error("Error toggling like status:", error);
  }
}

async function checkIfLiked(songId) {
  const checkbox = document.getElementById("like-checkbox");
  const playlistBtn = document.getElementById("add-to-playlist-btn");
  checkbox.setAttribute("data-song-id", songId);
  playlistBtn.setAttribute("data-song-id", songId);
  try {
    const isLiked = await CRUDService.fetchById(
      "LikedSongs/IsSongLikedByUser",
      songId
    );
    checkbox.checked = isLiked;
  } catch (error) {
    console.error("Error checking like status:", error);
  }
}

async function createPlaylistOptions() {
  const playlists = await CRUDService.fetchAll(
    "Playlist/GetAllPlaylistsByUserId"
  );

  if (!playlists || playlists.status === 404) {
    Swal.fire({
      title: "Opps! 😶",
      text: "You don't have any playlists yet. Create one to add songs.",
      icon: "question",
      showCancelButton: true,
      confirmButtonText: "Create Playlist",
      background: "#1f2937",
      color: "#fff",
    }).then((result) => {
      if (result.isConfirmed) {
        addNewPlaylist();
      }
    });
    return -1;
  }

  const playlistOptions = playlists
    .map(
      (playlist) =>
        `<option value="${playlist.playlistId}">${playlist.playlistName}</option>`
    )
    .join("");

  return `
    <div class="mb-4">
    <select
        id="playlist-options"
        class="mt-1 p-2 w-full bg-gray-700 border border-gray-600 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 text-white">
        ${playlistOptions}
    </select>
    </div>
  `;
}

async function addToPlaylist() {
  let playlistOptions = await createPlaylistOptions();
  if (playlistOptions === -1) return;
  swal.fire({
    title: "Add to Playlist",
    html: playlistOptions,
    showCancelButton: true,
    confirmButtonText: "Add",
    showLoaderOnConfirm: true,
    background: "#1f2937",
    color: "#fff",
    customClass: {
      confirmButton:
        "bg-gradient-to-r from-indigo-500 via-purple-500 to-pink-500 text-white font-semibold py-2 px-6 rounded-full shadow-lg hover:shadow-xl transform hover:scale-110 transition-all duration-300 ease-in-out",
      cancelButton:
        "bg-gray-600 text-white font-semibold py-2 px-6 rounded-full shadow-lg hover:shadow-xl transform hover:scale-110 transition-all duration-300 ease-in-out",
    },
    preConfirm: async () => {
      const playlistId = document.getElementById("playlist-options").value;
      const songId = document
        .getElementById("add-to-playlist-btn")
        .getAttribute("data-song-id");
      try {
        await CRUDService.create("PlaylistSongs/AddSongToPlaylist", {
          playlistId,
          songId,
        });

        message.showSuccessToast(
          "🎉 Song Added",
          "Song has been added to your Playlist."
        );
      } catch (error) {
        message.showAlert("info", "Error", error.message);
      }
    },
  });
}

async function createPlaylist(data) {
  try {
    const result = await CRUDService.create("Playlist/CreatePlaylist", data);
    message.showSuccessToast(
      "🎉 Playlist Created",
      "Playlist has been added to your library."
    );
  } catch (error) {
    message.showAlert("error", "Error", error.message);
  }
}

function addNewPlaylist() {
  Swal.fire({
    title: "Create a new playlist",
    html: `
      <form id="create-playlist-form">
        <div class="mb-4">
          <label for="playlist-name" class="block text-sm font-medium text-gray-300">Playlist Name</label>
          <input type="text" id="playlist-name" onblur="validateText('playlist-name')" class="mt-1 p-2 w-full bg-gray-700 border border-gray-600 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 text-white" placeholder="Enter playlist name">
          <div class="error" id="playlist-nameError"></div>
        </div>
        <div class="mb-4">
          <label for="description" class="block text-sm font-medium text-gray-300">Description</label>
          <input type="text" id="description" onblur="validateText('description')" class="mt-1 p-2 w-full bg-gray-700 border border-gray-600 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 text-white" placeholder="Enter description">
          <div class="error" id="descriptionError"></div>
        </div>
        <div class="mb-4 flex items-center">
          <input type="checkbox" id="is-private" class="form-checkbox h-4 w-4 text-indigo-500 transition duration-150 ease-in-out">
          <label for="is-private" class="ml-2 block text-sm text-gray-300">Make private</label>
        </div>
      </form>
    `,
    showCancelButton: true,
    confirmButtonText: "Create",
    background: "#1f2937",
    color: "#fff",
    customClass: {
      confirmButton:
        "bg-gradient-to-r from-indigo-500 via-purple-500 to-pink-500 text-white font-semibold py-2 px-6 rounded-full shadow-lg hover:shadow-xl transform hover:scale-110 transition-all duration-300 ease-in-out",
      cancelButton:
        "bg-gray-600 text-white font-semibold py-2 px-6 rounded-full shadow-lg hover:shadow-xl transform hover:scale-110 transition-all duration-300 ease-in-out",
    },
    preConfirm: () => {
      const playlistName = document.getElementById("playlist-name").value;
      const description = document.getElementById("description").value;
      const isPrivate = document.getElementById("is-private").checked;

      if (!validateForm("playlist")) {
        Swal.showValidationMessage(
          "Please fix the above errors first before submitting."
        );
      }

      return { playlistName, description, isPrivate };
    },
  }).then((result) => {
    if (result.isConfirmed) {
      const { playlistName, description, isPrivate } = result.value;
      const data = { playlistName, description, isPrivate };
      createPlaylist(data);
    }
  });
}
