const CACHE_NAME = "cached-songs";
const SONG_CACHE_NAME = "songCache";
const BASE_URL = "https://songserviceapi.azurewebsites.net/api/v1/";
const ENDPOINT = "SongsData/GetPlaylistById?listId=1220338282&lyrics=false";

async function fetchSongs() {
  const cache = await caches.open(CACHE_NAME);

  const cachedResponse = await cache.match(BASE_URL + ENDPOINT);
  if (cachedResponse) {
    const data = await cachedResponse.json();
    return processSongData(data);
  }

  try {
    const data = await CRUDService.fetchAll(ENDPOINT, true);
    const clonedResponse = new Response(JSON.stringify(data));
    await cache.put(BASE_URL + ENDPOINT, clonedResponse);

    return processSongData(data);
  } catch (error) {
    console.error("Error fetching song data:", error);
    return getDefaultCategories();
  }
}

async function processSongData(data) {
  const { content_list, songs } = data;
  const songMap = new Map(songs.map((song) => [song.id, song]));

  const recentlyPlayedSongIdsPromise = getHistorySongsData();

  const categories = {
    trending: getCategorySongs(content_list, songMap, 0, 5),
    relaxing: getCategorySongs(content_list, songMap, 5, 10),
    romance: getCategorySongs(content_list, songMap, 10, 15),
    lofi: getCategorySongs(content_list, songMap, 15, 20),
  };

  const recentlyPlayedSongIds = await recentlyPlayedSongIdsPromise;
  categories.history = recentlyPlayedSongIds;

  toggleVisibility("skeleton-loader", false);
  toggleVisibility("content", true);

  return categories;
}

function getCategorySongs(contentList, songMap, start, end) {
  return contentList.slice(start, end).map((id) => songMap.get(id) || {});
}

function getDefaultCategories() {
  return {
    history: [],
    trending: [],
    relaxing: [],
    romance: [],
    lofi: [],
  };
}

function toggleVisibility(elementId, isVisible) {
  const element = document.getElementById(elementId);
  if (element) {
    element.classList.toggle("hidden", !isVisible);
  }
}

function createSongCard({
  id,
  title = "Unknown Title",
  image = "./assets/img/songLogo.avif",
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
    toggleVisibility("history-header", false);
    toggleVisibility("history", false);
  }

  containerIds.forEach((id) => {
    const container = document.getElementById(id);
    if (container) {
      container.innerHTML = categories[id].map(createSongCard).join("");
    }
  });
}

async function fetchSong(songId) {
  const cache = await caches.open(SONG_CACHE_NAME);

  const cachedResponse = await cache.match(songId);
  if (cachedResponse) {
    return await cachedResponse.json();
  }

  try {
    const songData = await CRUDService.fetchAll(
      `SongsData/GetSongById?id=${songId}&lyrics=false`,
      true
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
      `PlayHistory/GetSongHistoryByUser?=true`,
      false
    );
  } catch (error) {
    console.error("Error fetching songs:", error);
    return { songs: [] };
  }
}

async function getHistorySongsData() {
  try {
    const { songs } = await fetchRecentlyPlayedSongId();

    if (!songs || songs.length === 0) return [];

    const songsToFetch = songs.slice(0, 10);

    const fetchPromises = songsToFetch.map((songId) => fetchSong(songId));
    const data = await Promise.all(fetchPromises);

    return data.filter((songData) => songData != null);
  } catch (error) {
    console.error("Error fetching song data:", error);
    return [];
  }
}

window.addEventListener("load", renderSongs);

