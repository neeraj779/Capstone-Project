import { FaDownload } from "react-icons/fa6";
import SongCard from "../components/SongCard";
import usePlaylistSongs from "../hooks/usePlaylistSongs";
import useRecentlyPlayedSongs from "../hooks/useRecentlyPlayedSongs";
import SearchBar from "../components/SearchBar";
import SongSkeleton from "../components/SongSkeleton";

const categoryInfo = {
  history: {
    title: "🚀 Recently played",
    description: "Songs you've listened to recently",
  },
  trending: {
    title: "🔥 Trending",
    description: "Trending songs in India",
  },
  relaxing: {
    title: "🎧 Relaxing",
    description: "Top relaxing songs for peace",
  },
  romance: {
    title: "💞 Romance",
    description: "Top romantic songs for love",
  },
  lofi: {
    title: "💤 Lofi",
    description: "Top lofi songs for relaxation",
  },
};

const Home = () => {
  const { playlistSongs, loading: playlistLoading } = usePlaylistSongs();
  const { recentlyPlayed, loading: recentlyPlayedLoading } =
    useRecentlyPlayedSongs();

  const renderSongs = (category) => (
    <div className="hide-scrollbar mb-3 pb-4 flex gap-6 items-center overflow-x-auto whitespace-nowrap">
      {playlistLoading && <SongSkeleton cards={5} />}
      {playlistSongs[category].map((song) => (
        <SongCard
          key={song.id}
          id={song.id}
          title={song.title}
          image={song.image}
          song={song.song}
          primary_artists={song.primary_artists}
        />
      ))}
    </div>
  );

  return (
    <div>
      <div className="container mx-auto px-6 md:px-20 py-10">
        <div className="block md:hidden mb-6">
          <SearchBar />
        </div>

        <div id="content">
          {Object.keys(categoryInfo).map((category) => (
            <div key={category} className="-mb-[3px]">
              {category === "history" && recentlyPlayed.length > 0 ? (
                <>
                  <h1 className="font-bold text-lg">
                    {categoryInfo[category].title}
                  </h1>
                  <p className="text-xs mb-4">
                    {categoryInfo[category].description}
                  </p>
                  <div className="hide-scrollbar mb-3 pb-4 flex gap-6 items-center overflow-x-auto whitespace-nowrap">
                    {recentlyPlayedLoading && <SongSkeleton cards={5} />}
                    {recentlyPlayed.map((song) => (
                      <SongCard
                        key={song.id}
                        id={song.id}
                        title={song.title}
                        image={song.image}
                        song={song.song}
                        primary_artists={song.primary_artists}
                      />
                    ))}
                  </div>
                </>
              ) : category !== "history" ? (
                <>
                  <h1 className="font-bold text-lg">
                    {categoryInfo[category].title}
                  </h1>
                  <p className="text-xs mb-4">
                    {categoryInfo[category].description}
                  </p>
                  {renderSongs(category)}
                </>
              ) : null}
            </div>
          ))}
        </div>

        <button
          id="install-button"
          className="fixed bottom-6 right-6 bg-gray-700 hover:bg-gray-600 text-white p-4 rounded-lg focus:outline-none focus:ring-2 focus:ring-gray-500 shadow-lg z-50 hidden"
        >
          <FaDownload />
        </button>
      </div>
    </div>
  );
};

export default Home;
