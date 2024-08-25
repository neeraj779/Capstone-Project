import { useNavigate } from "react-router-dom";
import { FaSearch } from "react-icons/fa";

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
          className="w-full px-4 py-2 pl-10 text-white placeholder-gray-400 bg-gray-700 rounded-full"
          placeholder="Search songs, artists..."
        />
        <FaSearch className="absolute text-gray-400 transform -translate-y-1/2 left-3 top-1/2" />
      </form>
    </>
  );
};

export default MobileSearchBar;
