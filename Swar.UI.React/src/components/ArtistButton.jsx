import PropTypes from "prop-types";
import { Link } from "react-router-dom";

const ArtistButton = ({ artist }) => {
  return (
    <Link
      to={`/search?q=${encodeURIComponent(artist)}`}
      className="bg-gray-700 text-white text-xs font-semibold py-3 px-4 rounded-full hover:bg-gray-600 transition duration-300"
    >
      {artist}
    </Link>
  );
};

ArtistButton.propTypes = {
  artist: PropTypes.string.isRequired,
};

export default ArtistButton;
