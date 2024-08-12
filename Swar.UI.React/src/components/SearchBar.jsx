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
          className="bg-gray-700 text-white placeholder-gray-400 rounded-full py-2 px-4 pl-10 w-full"
          placeholder="Search songs, artists..."
        />
        <FaSearch className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" />
      </form>
    </>
  );
};

export default MobileSearchBar;
