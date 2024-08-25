import { useSearchParams } from "react-router-dom";
import useSearchData from "../hooks/useSearchData";
import usePlaylistSongs from "../hooks/usePlaylistSongs";
import SongCard from "../components/SongCard";
import ArtistButton from "../components/ArtistButton";
import SearchBar from "../components/SearchBar";
import SongSkeleton from "../components/SongSkeleton";
import ArtistSkeleton from "../components/ArtistSkeleton";
import ErrorMessage from "../components/Error/ErrorMessage";

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
        <h1 className="text-lg font-bold">{title}</h1>
        <p className="mb-2 text-xs">{subtitle}</p>
      </div>
      <div className="flex items-center gap-6 pb-4 mb-3 overflow-x-auto hide-scrollbar whitespace-nowrap">
        {isLoading ? loadingComponent : items.map(renderItem)}
      </div>
    </div>
  );

  return (
    <div className="text-white bg-gray-900">
      <div className="container px-6 py-10 mx-auto md:px-20">
        <div className="block mb-6 md:hidden">
          <SearchBar />
        </div>

        <div id="content">
          {error ? (
            <ErrorMessage statusCode={error.status} />
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
