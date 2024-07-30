async function fetchSongs() {
  const cacheName = "cached-songs";
  const baseUrl = "https://songserviceapi.azurewebsites.net/api/v1/";
  const endpoint = "SongsData/GetPlaylistById?listId=1220338282&lyrics=false";

  const cache = await caches.open(cacheName);

  const cachedResponse = await cache.match(baseUrl + endpoint);
  if (cachedResponse) {
    const data = await cachedResponse.json();
    return processSongData(data);
  }

  try {
    const data = await CRUDService.fetchAll(endpoint, true);
    const clonedResponse = new Response(JSON.stringify(data));
    await cache.put(baseUrl + endpoint, clonedResponse);

    return processSongData(data);
  } catch (error) {
    console.error("Error fetching song data:", error);
    return {
      history: [],
      trending: [],
      relaxing: [],
      romance: [],
      lofi: [],
    };
  }
}

async function processSongData(data) {
  const { content_list, songs } = data;
  const songMap = new Map(songs.map((song) => [song.id, song]));
  const recentlyPlayedSongIds = await getHistorySongsData();
  const res = {
    history: recentlyPlayedSongIds || [],
    trending: content_list.slice(0, 5).map((id) => songMap.get(id) || {}),
    relaxing: content_list.slice(5, 10).map((id) => songMap.get(id) || {}),
    romance: content_list.slice(10, 15).map((id) => songMap.get(id) || {}),
    lofi: content_list.slice(15, 20).map((id) => songMap.get(id) || {}),
  };

  document.getElementById("skeleton-loader").classList.add("hidden");
  document.getElementById("content").classList.remove("hidden");

  return res;
}

function createSongCard({
  id,
  title = "Unknown Title",
  image = "default-image.jpg",
  song = "Unknown Song",
  primary_artists = "Unknown Artist",
}) {
  return `
        <a href="songPlayer.html?id=${id}" class="cursor-pointer" title="${title}">
            <div class="card rounded-md h-fit grid gap-2">
                <div>
                    <img class="transition hover:opacity-75 rounded-md w-full h-[200px] bg-secondary aspect-square object-cover" src="${image}" alt="${title}"/>
                </div>
                <div class="grid place-content-center text-center">
                    <h1 class="text-sm text-ellipsis overflow-hidden max-w-[200px] font-bold">${song}</h1>
                    <p class="text-xs text-ellipsis overflow-hidden -mt-[2px] max-w-[200px] mx-auto">${primary_artists}</p>
                </div>
            </div>
        </a>
    `;
}

async function renderSongs() {
  const categories = await fetchSongs();
  const containerIds = ["history", "trending", "relaxing", "romance", "lofi"];

  if (categories.history.length === 0) {
    document.getElementById("history-header").classList.add("hidden");
    document.getElementById("history").classList.add("hidden");
  }

  const containers = containerIds.reduce((acc, id) => {
    acc[id] = document.getElementById(id);
    return acc;
  }, {});

  containerIds.forEach((id) => {
    if (containers[id]) {
      containers[id].innerHTML = categories[id].map(createSongCard).join("");
    }
  });
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

async function fetchRecentlyPlayedSongId() {
  try {
    let response = await CRUDService.fetchAll(
      `PlayHistory/GetSongHistoryByUser?=true`,
      false
    );
    return response;
  } catch (error) {
    console.error("Error fetching songs:", error);
  }
}

async function getHistorySongsData() {
  try {
    const { songs } = await fetchRecentlyPlayedSongId();

    if (!songs || songs.length === 0) return [];

    const fetchPromises = songs.map((songId) => fetchSong(songId));
    const data = await Promise.all(fetchPromises);

    return data.filter((songData) => songData != null);
  } catch (error) {
    console.error("Error fetching song data:", error);
    return [];
  }
}

window.addEventListener("load", renderSongs);

document.getElementById("hamburger")?.addEventListener("click", () => {
  document.getElementById("mobile-menu")?.classList.toggle("hidden");
});
