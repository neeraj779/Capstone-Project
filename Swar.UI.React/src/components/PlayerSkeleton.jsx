import Skeleton from "react-loading-skeleton";
import "react-loading-skeleton/dist/skeleton.css";

const skeletonCircleProps = { circle: true, height: 40, width: 40 };

const PlayerSkeleton = () => {
  return (
    <div className="grid gap-6 px-6 mt-5">
      <div className="block mx-6 mb-6 md:hidden">
        <Skeleton height={40} width="100%" borderRadius={12} />
      </div>

      <div className="grid gap-3 text-center place-content-center">
        <Skeleton height={208} width={208} />
        <Skeleton height={24} width="75%" />
        <Skeleton height={16} width="50%" />
      </div>

      <div className="w-full max-w-[400px] mx-auto flex items-center justify-between -mt-3 -mb-3">
        <Skeleton {...skeletonCircleProps} />
        <Skeleton {...skeletonCircleProps} />
      </div>

      <div className="w-full max-w-[400px] mx-auto">
        <Skeleton height={8} className="mb-2" />
      </div>

      <div className="flex items-center justify-center gap-4">
        <Skeleton {...skeletonCircleProps} />
        <div className="flex items-center justify-center gap-2">
          <Skeleton {...skeletonCircleProps} />
          <Skeleton {...skeletonCircleProps} />
          <Skeleton {...skeletonCircleProps} />
        </div>
        <Skeleton {...skeletonCircleProps} />
      </div>

      <div className="lyrics-container">
        <h2 className="mb-2 text-xl font-semibold">Lyrics</h2>
        <div className="space-y-2">
          <Skeleton height={16} count={5} />
        </div>
      </div>
    </div>
  );
};

export default PlayerSkeleton;
