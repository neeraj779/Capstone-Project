import Skeleton from "react-loading-skeleton";
import "react-loading-skeleton/dist/skeleton.css";

const ArtistSkeleton = ({ cards }) => {
  return Array(cards)
    .fill(0)
    .map((_, index) => (
      <div key={index} className="song-skeleton">
        <Skeleton height={27} width={75} borderRadius={12} />
      </div>
    ));
};

export default ArtistSkeleton;
