import SongCard from "../components/SongCard";
import usePlaylistSongs from "../hooks/usePlaylistSongs";
import useRecentlyPlayedSongs from "../hooks/useRecentlyPlayedSongs";
import SearchBar from "../components/SearchBar";
import SongSkeleton from "../components/SongSkeleton";
import Footer from "../components/Footer";

const categoryInfo = {
  history: {
    title: "🚀 Recently Played",
    description: "Songs you’ve recently enjoyed.",
  },
  trending: {
    title: "🔥 Trending Now",
    description: "Hottest tracks in India.",
  },
  relaxing: {
    title: "🎧 Chill Vibes",
    description: "Soothing melodies for relaxation.",
  },
  romance: {
    title: "💞 Love Songs",
    description: "Romantic tracks for date nights.",
  },
  lofi: {
    title: "💤 Lofi Beats",
    description: "Best lofi tunes to relax.",
  },
};

const Home = () => {
  const { playlistSongs, loading: playlistLoading } = usePlaylistSongs();
  const { recentlyPlayed, loading: recentlyPlayedLoading } =
    useRecentlyPlayedSongs();

  const renderSongs = (category) => (
    <div className="flex items-center gap-6 pb-4 mb-3 overflow-x-auto hide-scrollbar whitespace-nowrap">
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
      <div className="container px-6 py-10 mx-auto md:px-20">
        <div className="block mb-6 md:hidden">
          <SearchBar />
        </div>

        <div>
          {Object.keys(categoryInfo).map((category) => (
            <div key={category} className="-mb-[3px]">
              {category === "history" && (
                <>
                  {recentlyPlayedLoading ? (
                    <div className="flex items-center gap-6 pb-4 mb-3 overflow-x-auto hide-scrollbar whitespace-nowrap">
                      <SongSkeleton cards={5} />
                    </div>
                  ) : recentlyPlayed.length > 0 ? (
                    <>
                      <h1 className="text-lg font-bold">
                        {categoryInfo[category].title}
                      </h1>
                      <p className="mb-4 text-xs">
                        {categoryInfo[category].description}
                      </p>
                      <div className="flex items-center gap-6 pb-4 mb-3 overflow-x-auto hide-scrollbar whitespace-nowrap">
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
                  ) : null}
                </>
              )}
              {category !== "history" && (
                <>
                  <h1 className="text-lg font-bold">
                    {categoryInfo[category].title}
                  </h1>
                  <p className="mb-4 text-xs">
                    {categoryInfo[category].description}
                  </p>
                  {renderSongs(category)}
                </>
              )}
            </div>
          ))}
        </div>
        <Footer />
      </div>
    </div>
  );
};

export default Home;
