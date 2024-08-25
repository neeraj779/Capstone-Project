import PropTypes from "prop-types";
import { Link } from "react-router-dom";

const ArtistButton = ({ artist }) => {
  return (
    <Link
      to={`/search?q=${encodeURIComponent(artist)}`}
      className="px-4 py-3 text-xs font-semibold text-white transition duration-300 bg-gray-700 rounded-full hover:bg-gray-600"
    >
      {artist}
    </Link>
  );
};

ArtistButton.propTypes = {
  artist: PropTypes.string.isRequired,
};

export default ArtistButton;
