import Skeleton from "react-loading-skeleton";
import "react-loading-skeleton/dist/skeleton.css";

const ProfileSkeleton = () => {
  return (
    <div className="flex flex-col items-center text-center">
      <div className="relative mb-8">
        <Skeleton circle={true} height={144} width={144} />
        <span className="absolute bottom-0 right-0 bg-gray-600 text-gray-300 text-xs font-bold rounded-full px-3 py-1 shadow-md">
          <Skeleton width={60} />
        </span>
      </div>
      <div className="bg-gray-800 p-6 rounded-lg shadow-lg w-full">
        <Skeleton width={200} count={6} />
      </div>
    </div>
  );
};

export default ProfileSkeleton;
