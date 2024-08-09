import { Link } from "react-router-dom";
import PropTypes from "prop-types";

const SongCard = ({ id, title, image, song, primary_artists }) => (
  <Link to={`/song/${id}`} className="cursor-pointer" title={title}>
    <div className="card rounded-md h-fit grid gap-2 min-w-[200px] max-w-[200px]">
      <div>
        <img
          className="transition hover:opacity-75 rounded-md w-full h-[200px] bg-secondary aspect-square object-cover"
          src={image}
          alt={title}
        />
      </div>
      <div className="grid place-content-center text-center">
        <h1 className="text-sm text-ellipsis overflow-hidden max-w-[200px] font-bold">
          {song}
        </h1>
        <p className="text-xs text-ellipsis overflow-hidden -mt-[2px] max-w-[200px] mx-auto">
          {primary_artists}
        </p>
      </div>
    </div>
  </Link>
);

SongCard.propTypes = {
  id: PropTypes.string.isRequired,
  title: PropTypes.string,
  image: PropTypes.string,
  song: PropTypes.string.isRequired,
  primary_artists: PropTypes.string.isRequired,
};

export default SongCard;
