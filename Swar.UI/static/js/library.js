document.addEventListener("DOMContentLoaded", () => {
  fetchPlaylists();
});

async function fetchPlaylists() {
  try {
    const playlists = await CRUDService.fetchAll(
      "Playlist/GetAllPlaylistsByUserId"
    );
    renderPlaylists(playlists);
    addDropdownEventListeners();
  } catch (error) {
    console.error("Error fetching playlists:", error);
  }
}

async function createPlaylist(data) {
  try {
    const result = await CRUDService.create("Playlist/CreatePlaylist", data);
    message.showSuccessToast(
      "🎉 Playlist Created",
      "Playlist has been added to your library."
    );
    fetchPlaylists();
  } catch (error) {
    message.showAlert("error", "Error", error.message);
  }
}

async function deletePlaylist(anchor) {
  const playlistId = anchor.getAttribute("data-playlistid");
  const playlistName = anchor.getAttribute("data-playlistname");

  Swal.fire({
    icon: "warning",
    title: "Are you sure?",
    text: `You are about to delete the playlist ${playlistName}. This action cannot be undone.`,
    showCancelButton: true,
    confirmButtonText: "Delete",
    confirmButtonColor: "#e53e3e",
    cancelButtonText: "Cancel",
    background: "#1f2937",
    color: "#fff",
    customClass: {
      confirmButton:
        "bg-gradient-to-r from-red-500 to-yellow-500 text-white font-semibold py-2 px-6 rounded-full shadow-lg hover:shadow-xl transform hover:scale-110 transition-all duration-300 ease-in-out",
      cancelButton:
        "bg-gray-600 text-white font-semibold py-2 px-6 rounded-full shadow-lg hover:shadow-xl transform hover:scale-110 transition-all duration-300 ease-in-out",
    },
  }).then(async (result) => {
    if (result.isConfirmed) {
      try {
        await CRUDService.remove("Playlist/DeletePlaylist", playlistId);
        message.showSuccessToast(
          "🚀 Playlist Deleted",
          "Playlist has been removed."
        );
        fetchPlaylists();
      } catch (error) {
        message.showAlert("error", "Error", error.message);
      }
    }
  });
}

function createPlaylistCard(playlist) {
  return `
    <div
      class="relative flex items-center bg-gray-800 rounded-lg p-4 shadow-lg hover:shadow-xl transition-shadow duration-300"
    >
      <a href="playlist.html?id=${playlist.playlistId}&name=${
    playlist.playlistName
  }">
        <img
          src="./assets/img/playlist.png"
          alt="Album Cover"
          class="w-32 h-32 object-cover rounded-md border border-gray-700"
        />
      </a>
      <div class="ml-6 flex flex-col justify-center flex-grow">
        <div class="flex items-center">
          <a href="playlist.html?id=${playlist.playlistId}&name=${
    playlist.playlistName
  }">
            <h2 class="text-2xl font-semibold mb-1">${
              playlist.playlistName
            }</h2>
          </a>
        </div>
        <p class="text-gray-400 text-sm mb-2">
          ${playlist.description}
        </p>
        <p class="text-gray-500 text-sm">Creation Date: ${new Date(
          playlist.createdAt
        ).toLocaleDateString()}</p>
      </div>
      <div class="dropdown-button">
        <button
          class="text-gray-400 hover:text-gray-600 focus:outline-none"
          aria-haspopup="true"
          aria-expanded="false"
        >
          <i class="fas fa-ellipsis-v text-xl"></i>
        </button>
        <div class="dropdown-content">
          <a
            href="#"
            data-playlistid="${playlist.playlistId}"
            data-playlistname="${playlist.playlistName}"
            data-description="${playlist.description}"
            onclick="editPlaylist(this)"
            class="text-blue-500 hover:text-blue-700 flex items-center px-4 py-2"
          >
            <i class="fas fa-pencil-alt mr-2"></i>Edit Playlist
          </a>
          <a
            href="#"
            data-playlistid="${playlist.playlistId}"
            data-playlistname="${playlist.playlistName}"
            onclick="deletePlaylist(this)"
            class="text-red-500 hover:text-red-700 flex items-center px-4 py-2"
          >
            <i class="fas fa-trash-alt mr-2"></i>Delete Playlist
          </a>
           <a
            href="#"
            data-playlistid="${playlist.playlistId}"
            data-playlistname="${playlist.playlistName}"
            onclick="changeVisibility(this, !${playlist.isPrivate})"
            class="text-green-500 hover:text-green-700 flex items-center px-4 py-2"
          >
            <i class="fas ${
              playlist.isPrivate ? "fa-unlock" : "fa-lock"
            } mr-2"></i>${playlist.isPrivate ? "Make Public" : "Make Private"}
          </a>
        </div>
      </div>
    </div>
  `;
}

function renderPlaylists(playlists) {
  const container = document.getElementById("playlist-container");
  container.innerHTML = "";

  if (Array.isArray(playlists)) {
    playlists.forEach((playlist) => {
      const card = createPlaylistCard(playlist);
      container.innerHTML += card;
    });
  }
}

function addDropdownEventListeners() {
  document.querySelectorAll(".dropdown-button").forEach((button, index) => {
    const dropdownContent =
      document.querySelectorAll(".dropdown-content")[index];

    button.addEventListener("click", () => {
      const isVisible = dropdownContent.classList.contains("show");
      document
        .querySelectorAll(".dropdown-content")
        .forEach((content) => content.classList.remove("show"));
      dropdownContent.classList.toggle("show", !isVisible);
      button.setAttribute("aria-expanded", !isVisible);
    });

    document.addEventListener("click", (event) => {
      if (
        !button.contains(event.target) &&
        !dropdownContent.contains(event.target)
      ) {
        dropdownContent.classList.remove("show");
        button.setAttribute("aria-expanded", "false");
      }
    });
  });
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

function editPlaylist(anchor) {
  const playlistId = anchor.getAttribute("data-playlistid");
  const playlistName = anchor.getAttribute("data-playlistname");
  const description = anchor.getAttribute("data-description");
  swal
    .fire({
      title: "Edit Playlist",
      html: `
      <form id="edit-playlist-form">
        <div class="mb-4">
          <label for="playlist-name" class="block text-sm font-medium text-gray-300">Playlist Name</label>
          <input type="text" id="playlist-name" onblur="validateText('playlist-name')" class="mt-1 p-2 w-full bg-gray-700 border border-gray-600 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 text-white" value="${playlistName}">
          <div class="error" id="playlist-nameError"></div>
        </div>
        <div class="mb-4">
          <label for="description" class="block text-sm font-medium text-gray-300">Description</label>
          <input type="text" id="description" onblur="validateText('description')" class="mt-1 p-2 w-full bg-gray-700 border border-gray-600 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 text-white" value="${description}">
          <div class="error" id="descriptionError"></div>
        </div>
      </form>
    `,
      showCancelButton: true,
      confirmButtonText: "Save",
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

        if (!validateForm("playlist")) {
          Swal.showValidationMessage(
            "Please fix the above errors first before submitting."
          );
        }

        return { playlistName, description };
      },
    })
    .then((result) => {
      if (result.isConfirmed) {
        CRUDService.update("Playlist/UpdatePlaylist", playlistId, result.value)
          .then(() => {
            message.showSuccessToast(
              "🚀 Playlist Updated",
              "Playlist has been updated."
            );
            fetchPlaylists();
          })
          .catch((error) => {
            message.showAlert("error", "Error", error.message);
          });
      }
    });
}

async function changeVisibility(anchor, isPrivate) {
  const playlistId = anchor.getAttribute("data-playlistid");
  const playlistName = anchor.getAttribute("data-playlistname");

  if (isPrivate) {
    Swal.fire({
      icon: "info",
      title: "Make Playlist Private?",
      text: `Only you will be able to view the playlist ${playlistName}.`,
      showCancelButton: true,
      confirmButtonText: "Yes",
      confirmButtonColor: "#e53e3e",
      cancelButtonText: "Cancel",
      background: "#1f2937",
      color: "#fff",
      customClass: {
        confirmButton:
          "bg-gradient-to-r from-red-500 to-yellow-500 text-white font-semibold py-2 px-6 rounded-full shadow-lg hover:shadow-xl transform hover:scale-110 transition-all duration-300 ease-in-out",
        cancelButton:
          "bg-gray-600 text-white font-semibold py-2 px-6 rounded-full shadow-lg hover:shadow-xl transform hover:scale-110 transition-all duration-300 ease-in-out",
      },
    }).then(async (result) => {
      if (result.isConfirmed) {
        try {
          await CRUDService.update(
            "Playlist/UpdatePlaylistPrivacy",
            playlistId,
            { isPrivate }
          );
          message.showSuccessToast(
            "🚀 Playlist Updated",
            `${playlistName} Playlist has been made private.`
          );
          fetchPlaylists();
        } catch (error) {
          message.showAlert("error", "Error", error.message);
        }
      }
    });
  } else {
    try {
      await CRUDService.update("Playlist/UpdatePlaylistPrivacy", playlistId, {
        isPrivate,
      });
      message.showSuccessToast(
        "🚀 Playlist Updated",
        `${playlistName} Playlist has been made public.`
      );
      fetchPlaylists();
    } catch (error) {
      message.showAlert("error", "Error", error.message);
    }
  }
}

document.getElementById("hamburger")?.addEventListener("click", () => {
  document.getElementById("mobile-menu")?.classList.toggle("hidden");
});
