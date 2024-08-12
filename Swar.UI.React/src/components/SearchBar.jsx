import { useNavigate } from "react-router-dom";
import { FcSearch } from "react-icons/fc";

const MobileSearchBar = () => {
  const navigate = useNavigate();

  const handleSubmit = (e) => {
    e.preventDefault();
    const query = e.target.searchInput.value;
    navigate(`/search?q=${encodeURIComponent(query)}`);
  };

  return (
    <>
      <form className="relative" onSubmit={handleSubmit}>
        <input
          type="text"
          name="searchInput"
          className="bg-gray-700 text-white placeholder-gray-400 rounded-full py-2 px-4 pl-10 w-full"
          placeholder="Search songs, artists..."
        />
        <FcSearch />
      </form>
    </>
  );
};

export default MobileSearchBar;
