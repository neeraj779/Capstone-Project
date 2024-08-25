import { Link } from "react-router-dom";
import PropTypes from "prop-types";
import { Image } from "@nextui-org/react";

const SongCard = ({ id, title, image, song, primary_artists }) => (
  <Link
    to="/player"
    state={{ songId: id }}
    className="cursor-pointer"
    title={title}
  >
    <div className="grid gap-2 min-w-[200px] max-w-[200px]">
      <div>
        <Image isZoomed width={240} src={image} alt={title} className="z-0" />
      </div>
      <div className="grid text-center place-content-center">
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
