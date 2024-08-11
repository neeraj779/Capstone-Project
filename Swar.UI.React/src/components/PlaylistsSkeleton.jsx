import Skeleton from "react-loading-skeleton";
import "react-loading-skeleton/dist/skeleton.css";

const PlaylistsSkeleton = () => {
  return (
    <>
      <Skeleton width={150} className="mb-3" />
      <div>
        <div className="bg-gray-700 p-6 rounded-lg  flex items-center gap-4">
          <Skeleton circle={true} height={48} width={48} />
          <div className="flex-1">
            <Skeleton width={120} />
            <Skeleton width={180} count={2} />
          </div>
        </div>
      </div>
    </>
  );
};

export default PlaylistsSkeleton;
