import Skeleton from "react-loading-skeleton";
import "react-loading-skeleton/dist/skeleton.css";

const SongSkeleton = ({ cards }) => {
  return Array(cards)
    .fill(0)
    .map((_, index) => (
      <div key={index} className="song-skeleton">
        <div className="song-skeleton__image">
          <Skeleton height={200} width={200} />
        </div>
        <div className="flex flex-col items-center song-skeleton__details">
          <Skeleton height={10} width={180} />
          <Skeleton height={10} width={160} />
        </div>
      </div>
    ));
};

export default SongSkeleton;
