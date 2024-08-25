import Skeleton from "react-loading-skeleton";
import "react-loading-skeleton/dist/skeleton.css";

const ProfileSkeleton = () => {
  return (
    <div className="flex flex-col items-center text-center">
      <div className="relative mb-8">
        <Skeleton circle={true} height={144} width={144} />
        <span className="absolute bottom-0 right-0 px-3 py-1 text-xs font-bold text-gray-300 bg-gray-600 rounded-full shadow-md">
          <Skeleton width={60} />
        </span>
      </div>
      <div className="w-full p-6 bg-gray-800 rounded-lg shadow-lg">
        <Skeleton width={200} count={6} />
      </div>
    </div>
  );
};

export default ProfileSkeleton;
