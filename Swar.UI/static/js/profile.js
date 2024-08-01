document.addEventListener("DOMContentLoaded", () => {
  const playlistsContainer = document.getElementById("playlists-container");
  const recentActivityContainer = document.getElementById(
    "recent-activity-container"
  );

  let profileData = {};

  initEventListeners();

  loadProfileData();
  loadPlaylists();
  loadRecentActivity();

  function initEventListeners() {
    document
      .getElementById("change-password")
      .addEventListener("click", changePassword);
    document
      .getElementById("delete-account")
      .addEventListener("click", deleteAccount);
    document
      .getElementById("edit-profile")
      .addEventListener("click", editProfile);
  }

  async function loadProfileData() {
    try {
      profileData = await CRUDService.fetchAll("users/me");
      updateProfileUI(profileData);
    } catch (error) {
      console.error("Error loading profile data:", error);
    }
  }

  function updateProfileUI(data) {
    if (data) {
      let profileImgSrc = "./assets/img/neutral_avatar.png";

      if (data.gender === "Male")
        profileImgSrc = "./assets/img/male_avatar.svg";
      else if (data.gender === "Female")
        profileImgSrc = "./assets/img/female_avatar.svg";

      document.getElementById("profile-picture").src = profileImgSrc;
      document.getElementById("profile-status").textContent = data.status;
      document.getElementById("user-name").textContent = data.name;
      document.getElementById("user-email").textContent = data.email;
      document.getElementById("user-doj").textContent = `Joined On: ${new Date(
        data.registrationDate
      ).toLocaleDateString()}`;
      document.getElementById(
        "user-gender"
      ).textContent = `Gender: ${data.gender}`;
    }
  }

  function truncateText(text, maxLength = 15) {
    return text.length <= maxLength
      ? text
      : `${text.substring(0, maxLength)}...`;
  }

  async function loadPlaylists() {
    try {
      const playlists = await CRUDService.fetchAll(
        "Playlist/GetAllPlaylistsByUserId"
      );
      updatePlaylistsUI(playlists);
    } catch (error) {
      console.error("Error loading playlists:", error);
    }
  }

  function updatePlaylistsUI(playlists) {
    if (playlists) {
      playlistsContainer.innerHTML = playlists
        .map(
          (playlist) => `
          <a href="playlist.html?id=${playlist.playlistId}&name=${
            playlist.playlistName
          }" class="bg-gray-700 p-4 rounded-lg flex items-center">
            <img src="./assets/img/playlist.png" alt="${
              playlist.playlistName
            } Image" class="w-16 h-16 rounded-md mr-4"/>
            <div>
              <h3 class="text-lg font-semibold">${playlist.playlistName}</h3>
              <p class="text-gray-400">${truncateText(playlist.description)}</p>
            </div>
          </a>
        `
        )
        .join("");
    }
  }

  async function fetchSong(songId) {
    const SONG_CACHE_NAME = "songCache";
    try {
      const cache = await caches.open(SONG_CACHE_NAME);
      const cachedResponse = await cache.match(songId);
      if (cachedResponse) return await cachedResponse.json();

      const songData = await CRUDService.fetchAll(
        `SongsData/GetSongById?id=${songId}&lyrics=false`
      );
      await cache.put(songId, new Response(JSON.stringify(songData)));
      return songData;
    } catch (error) {
      console.error("Error fetching song:", error);
      return null;
    }
  }

  async function fetchRecentlyPlayedSongId() {
    try {
      return await CRUDService.fetchAll(
        "PlayHistory/GetSongHistoryByUser?=true"
      );
    } catch (error) {
      console.error("Error fetching recently played songs:", error);
      return { songs: [] };
    }
  }

  async function getHistorySongsData() {
    try {
      const { songs } = await fetchRecentlyPlayedSongId();
      if (!songs || songs.length === 0) return [];

      const fetchPromises = songs
        .slice(0, 10)
        .map((songId) => fetchSong(songId));
      const data = await Promise.all(fetchPromises);

      return data.filter((songData) => songData !== null);
    } catch (error) {
      console.error("Error fetching song data:", error);
      return [];
    }
  }

  async function loadRecentActivity() {
    try {
      const recentActivity = await getHistorySongsData();
      updateRecentActivityUI(recentActivity);
    } catch (error) {
      console.error("Error loading recent activity:", error);
    }
  }

  function updateRecentActivityUI(recentActivity) {
    if (recentActivity) {
      recentActivityContainer.innerHTML = recentActivity
        .map(
          (activity) => `
          <a href="songPlayer.html?id=${
            activity.id
          }" class="bg-gray-700 p-4 rounded-lg flex items-center">
            <img src="${activity.image || "./assets/img/playlist.png"}" alt="${
            activity.song
          } Image" class="w-16 h-16 rounded-md mr-4"/>
            <div>
              <h3 class="text-lg font-semibold">${activity.song}</h3>
              <p class="text-gray-400">${truncateText(
                activity.primary_artists
              )}</p>
            </div>
          </a>
        `
        )
        .join("");
    }
  }

  function showAlert({
    title,
    html,
    confirmButtonText,
    preConfirm,
    onConfirm,
  }) {
    swal
      .fire({
        title,
        html,
        showCancelButton: true,
        confirmButtonText,
        allowOutsideClick: false,
        showLoaderOnConfirm: true,
        background: "#1f2937",
        color: "#fff",
        customClass: {
          confirmButton:
            "bg-gradient-to-r from-indigo-500 via-purple-500 to-pink-500 text-white font-semibold py-2 px-6 rounded-full shadow-lg hover:shadow-xl transform hover:scale-110 transition-all duration-300 ease-in-out",
          cancelButton:
            "bg-gray-600 text-white font-semibold py-2 px-6 rounded-full shadow-lg hover:shadow-xl transform hover:scale-110 transition-all duration-300 ease-in-out",
        },
        preConfirm,
      })
      .then((result) => {
        if (result.isConfirmed) onConfirm();
      });
  }

  async function changePassword() {
    showAlert({
      title: "Update Password",
      html: `
        <form id="update-password-form">
          <div class="mb-4">
            <label for="old-password" class="block text-sm font-medium text-gray-300">Enter your old password</label>
            <input type="password" id="old-password" class="mt-1 p-2 w-full bg-gray-700 border border-gray-600 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 text-white" required>
          </div>
          <div class="mb-4">
            <label for="new-password" class="block text-sm font-medium text-gray-300">Enter your new password</label> 
            <input type="password" id="new-password" class="mt-1 p-2 w-full bg-gray-700 border border-gray-600 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 text-white" required>
          </div>
        </form>
      `,
      confirmButtonText: "Update",
      preConfirm: async () => {
        const oldPassword = document.getElementById("old-password").value;
        const newPassword = document.getElementById("new-password").value;

        if (!oldPassword || !newPassword) {
          Swal.showValidationMessage("Please fill in all the fields.");
          return false;
        }

        if (oldPassword === newPassword) {
          Swal.showValidationMessage(
            "New password cannot be the same as the old password."
          );
          return false;
        }

        try {
          await CRUDService.patch("users/me/password", {
            oldPassword,
            newPassword,
          });
        } catch (error) {
          Swal.showValidationMessage(
            `Error: ${error.message || "Something went wrong!"}`
          );
          return false;
        }
      },
      onConfirm: () => {
        message.showSuccessToast(
          "Password Updated",
          "Your password has been updated successfully."
        );
      },
    });
  }

  async function deleteAccount() {
    showAlert({
      title: "Delete Account",
      text: "Are you sure you want to delete your account? This action cannot be undone.",
      icon: "warning",
      confirmButtonText: "Delete",
      preConfirm: async () => {
        try {
          await CRUDService.remove("users", "me");
        } catch (error) {
          Swal.showValidationMessage(
            `Error: ${error.message || "Unable to delete account!"}`
          );
          return false;
        }
      },
      onConfirm: () => {
        localStorage.clear();
        swal.fire({
          title: "Account Deleted",
          text: "Your account has been deleted successfully. You will be redirected to the login page shortly.",
          icon: "success",
          showConfirmButton: false,
          showCancelButton: false,
          allowOutsideClick: false,
          timer: 4000,
          timerProgressBar: true,
          background: "#1f2937",
          color: "#fff",
        });

        setTimeout(() => {
          window.location.href = "index.html";
        }, 2500);
      },
    });
  }

  async function editProfile() {
    showAlert({
      title: "Edit Profile",
      html: `
        <form id="edit-profile-form">
          <div class="mb-4">
            <label for="name" class="block text-sm font-medium text-gray-300">Name</label>
            <input type="text" id="name" value="${
              profileData.name || ""
            }" class="mt-1 p-2 w-full bg-gray-700 border border-gray-600 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 text-white" required>
          </div>
          <div class="mb-4">
            <label for="email" class="block text-sm font-medium text-gray-300">Email</label>
            <input type="email" id="email" value="${
              profileData.email || ""
            }" class="mt-1 p-2 w-full bg-gray-700 border border-gray-600 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 text-white" required>
          </div>
          <div class="mb-4">
            <label for="gender" class="block text-sm font-medium text-gray-300">Gender</label>
            <select id="gender" class="mt-1 p-2 w-full bg-gray-700 border border-gray-600 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 text-white" required>
              <option value="">Select Gender</option>
              <option value="Male" ${
                profileData.gender === "Male" ? "selected" : ""
              }>Male</option>
              <option value="Female" ${
                profileData.gender === "Female" ? "selected" : ""
              }>Female</option>
              <option value="Other" ${
                profileData.gender === "Other" ? "selected" : ""
              }>Other</option>
            </select>
          </div>
        </form>
      `,
      confirmButtonText: "Save Changes",
      preConfirm: async () => {
        const name = document.getElementById("name").value;
        const email = document.getElementById("email").value;
        const gender = document.getElementById("gender").value;

        if (!name || !email || !gender) {
          Swal.showValidationMessage("Please fill in all required fields.");
          return false;
        }

        try {
          await CRUDService.patch("users/me", { name, email, gender });
          return true;
        } catch (error) {
          Swal.showValidationMessage(
            `Error: ${error.message || "Something went wrong!"}`
          );
          return false;
        }
      },
      onConfirm: () => {
        message.showSuccessToast(
          "Profile Updated",
          "Your profile has been updated successfully."
        );
        loadProfileData();
      },
    });
  }
});
