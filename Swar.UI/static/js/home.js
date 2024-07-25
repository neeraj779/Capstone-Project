async function fetchSongs() {
  try {
    const response = await fetch(
      "http://localhost:5292/api/SongsData/GetPlaylistById?listId=1139074020&lyrics=false"
    );
    const data = await response.json();
    const { content_list, songs } = data;

    const songMap = new Map(songs.map((song) => [song.id, song]));

    return {
      trending: content_list.slice(0, 5).map((id) => songMap.get(id) || {}),
      relaxing: content_list.slice(5, 10).map((id) => songMap.get(id) || {}),
      romance: content_list.slice(10, 15).map((id) => songMap.get(id) || {}),
      lofi: content_list.slice(15, 20).map((id) => songMap.get(id) || {}),
    };
  } catch (error) {
    console.error("Error fetching song data:", error);
    return {
      trending: [],
      relaxing: [],
      romance: [],
      lofi: [],
    };
  }
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
  const containerIds = ["trending", "relaxing", "romance", "lofi"];

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

window.addEventListener("load", renderSongs);

document.getElementById("hamburger")?.addEventListener("click", () => {
  document.getElementById("mobile-menu")?.classList.toggle("hidden");
});
