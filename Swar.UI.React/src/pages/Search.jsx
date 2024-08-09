import { useSearchParams } from "react-router-dom";
import useSearchData from "../hooks/useSearchData";
import usePlaylistSongs from "../hooks/usePlaylistSongs";
import SongCard from "../components/SongCard";
import ArtistButton from "../components/ArtistButton";
import MobileSearchBar from "../components/MobileSearchBar";
import SongSkeleton from "../components/SongSkeleton";
import ArtistSkeleton from "../components/ArtistSkeleton";
import ErrorMessage from "../components/ErrorMessage";

const Search = () => {
  const [searchParams] = useSearchParams();
  const query = searchParams.get("q") || "";

  const {
    searchResults,
    artists,
    loading: searchLoading,
    error,
  } = useSearchData(query);
  const { playlistSongs = {}, loading: playlistLoading } = usePlaylistSongs();

  const renderSection = (
    title,
    subtitle,
    isLoading,
    loadingComponent,
    items,
    renderItem
  ) => (
    <div className="grid gap-5">
      <div className="-mb-[3px]">
        <h1 className="font-bold text-lg">{title}</h1>
        <p className="text-xs mb-2">{subtitle}</p>
      </div>
      <div className="hide-scrollbar mb-3 pb-4 flex gap-6 items-center overflow-x-auto whitespace-nowrap">
        {isLoading ? loadingComponent : items.map(renderItem)}
      </div>
    </div>
  );

  return (
    <div className="bg-gray-900 text-white">
      <div className="container mx-auto px-6 md:px-20 py-10">
        <MobileSearchBar />

        <div id="content">
          {error ? (
            <ErrorMessage message={error.message} />
          ) : (
            <>
              {renderSection(
                "🔎 Search Results",
                `Search results for "${query}"`,
                searchLoading,
                <SongSkeleton cards={5} />,
                searchResults,
                (song) => (
                  <SongCard
                    key={song.id}
                    id={song.id}
                    title={song.title}
                    image={song.image}
                    song={song.song}
                    primary_artists={song.primary_artists}
                  />
                )
              )}

              {renderSection(
                "🎤 Artists",
                "Artists related to your search",
                searchLoading,
                <ArtistSkeleton cards={5} />,
                artists,
                (artist) => (
                  <ArtistButton key={artist} artist={artist} />
                )
              )}

              {renderSection(
                "🔥 Trending",
                "Trending songs in India",
                playlistLoading,
                <SongSkeleton cards={5} />,
                playlistSongs.trending,
                (song) => (
                  <SongCard
                    key={song.id}
                    id={song.id}
                    title={song.title}
                    image={song.image}
                    song={song.song}
                    primary_artists={song.primary_artists}
                  />
                )
              )}
            </>
          )}
        </div>
      </div>
    </div>
  );
};

export default Search;
