import { useEffect, useState } from "react";
import useRecentlyPlayedSongs from "../hooks/useRecentlyPlayedSongs";
import { Avatar, Button } from "@nextui-org/react";
import { CustomScroll } from "react-custom-scroll";
import toast from "react-hot-toast";
import useApiClient from "../hooks/useApiClient";
import UpdatePasswordModal from "../components/modals/UpdatePasswordModal";

import playlistSvg from "../assets/img/playlist.svg";
import maleAvatar from "../assets/img/male-avatar.svg";
import femaleAvatar from "../assets/img/female-avatar.svg";
import neutralAvatar from "../assets/img/neutral-avatar.svg";
import ProfileSkeleton from "../components/ProfileSkeleton";
import PlaylistsSkeleton from "../components/PlaylistsSkeleton";
import { LuBadgeCheck } from "react-icons/lu";

const Profile = () => {
  const swarApiClient = useApiClient();
  const [data, setData] = useState({
    profile: null,
    playlists: [],
    loadingProfile: true,
    loadingPlaylists: true,
  });
  const { recentlyPlayed, loading: recentlyPlayedLoading } =
    useRecentlyPlayedSongs();
  const [isUpdatePasswordOpen, setIsUpdatePasswordOpen] = useState(false);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [profileResponse, playlistsResponse] = await Promise.all([
          swarApiClient.get("users/me"),
          swarApiClient.get("Playlist/GetAllPlaylistsByUserId"),
        ]);

        setData({
          profile: profileResponse.data,
          playlists: playlistsResponse.data,
          loadingProfile: false,
          loadingPlaylists: false,
        });
      } catch {
        toast.error("Failed to fetch profile data");
        setData((prev) => ({
          ...prev,
          loadingProfile: false,
          loadingPlaylists: false,
        }));
      }
    };

    fetchData();
  }, [swarApiClient]);

  const getProfileImage = (gender) => {
    switch (gender) {
      case "Male":
        return maleAvatar;
      case "Female":
        return femaleAvatar;
      default:
        return neutralAvatar;
    }
  };

  const { profile, playlists, loadingProfile, loadingPlaylists } = data;

  return (
    <div className="mx-auto px-6 py-12">
      <div className="flex flex-col md:flex-row gap-8">
        {/* Profile Section */}
        <div className="w-full md:w-1/2 lg:w-1/3 bg-gradient-to-br from-gray-900 to-gray-800 p-8 rounded-2xl shadow-2xl border border-gray-700">
          {loadingProfile ? (
            <ProfileSkeleton />
          ) : (
            <div className="flex flex-col items-center text-center">
              <div className="relative mb-8">
                <Avatar
                  src={
                    profile ? getProfileImage(profile.gender) : neutralAvatar
                  }
                  className="w-36 h-36 rounded-full border-4 border-gradient-to-r from-blue-500 to-teal-400 shadow-xl"
                />
                <span className="absolute bottom-0 right-0 bg-green-500 text-white text-xs font-bold rounded-full px-3 py-1 shadow-md">
                  <LuBadgeCheck className="w-5 h-5" />
                </span>
              </div>
              <div className="bg-gray-800 p-6 rounded-lg shadow-lg w-full">
                <h2 className="text-4xl font-extrabold text-white mb-2">
                  {profile?.name || "N/A"}
                </h2>
                <p className="text-gray-300 text-base mb-2">
                  {profile?.email || "N/A"}
                </p>
                <p className="text-gray-400 text-sm mb-2">
                  Member since:{" "}
                  {profile?.registrationDate
                    ? new Date(profile.registrationDate).toLocaleDateString()
                    : "N/A"}
                </p>
                <p className="text-gray-400 text-sm mb-6">
                  Gender: {profile?.gender || "N/A"}
                </p>
                <div className="flex flex-col space-y-4">
                  <Button color="primary" variant="shadow">
                    Edit Profile
                  </Button>
                  <Button
                    onClick={() => setIsUpdatePasswordOpen(true)}
                    color="secondary"
                    variant="shadow"
                  >
                    Change Password
                  </Button>
                  <UpdatePasswordModal
                    isOpen={isUpdatePasswordOpen}
                    onOpenChange={() => setIsUpdatePasswordOpen(false)}
                  />
                  <Button color="danger" variant="shadow">
                    Delete Account
                  </Button>
                </div>
              </div>
            </div>
          )}
        </div>

        {/* Content Section */}
        <div className="w-full md:w-2/3">
          {/* Playlists Section */}
          <div className="bg-gradient-to-br from-gray-800 to-gray-900 p-8 rounded-2xl shadow-2xl border border-gray-700 mb-8">
            {loadingPlaylists ? (
              <PlaylistsSkeleton />
            ) : (
              <>
                <h2 className="text-2xl font-extrabold text-white mb-6">
                  Your Playlists
                </h2>
                <CustomScroll>
                  <div className="grid grid-cols-1 lg:grid-cols-3 gap-4 mr-4">
                    {playlists.length > 0 ? (
                      playlists.map((playlist) => (
                        <div
                          key={playlist.playlistId}
                          className="bg-gray-700 p-6 rounded-lg shadow-lg hover:shadow-xl transition-shadow duration-300 flex items-center gap-4"
                        >
                          <img
                            src={playlistSvg}
                            alt="Playlist Thumbnail"
                            className="w-12 h-12 rounded-full"
                          />
                          <div className="flex-1 overflow-hidden">
                            <h3 className="text-xl font-semibold text-white truncate overflow-hidden whitespace-nowrap">
                              {playlist.playlistName}
                            </h3>
                            <p className="text-gray-300 text-sm truncate overflow-hidden whitespace-nowrap">
                              {playlist.description}
                            </p>
                          </div>
                        </div>
                      ))
                    ) : (
                      <div>No playlists available</div>
                    )}
                  </div>
                </CustomScroll>
              </>
            )}
          </div>

          {/* Recent Activity Section */}
          <div className="bg-gradient-to-br from-gray-800 to-gray-900 p-8 rounded-2xl shadow-2xl border border-gray-700">
            {recentlyPlayedLoading ? (
              <PlaylistsSkeleton />
            ) : (
              <>
                <h2 className="text-2xl font-extrabold text-white mb-6">
                  Recent Activity
                </h2>
                <CustomScroll>
                  <div className="space-y-4 max-h-[calc(4*5rem)]">
                    {recentlyPlayed.length > 0 ? (
                      recentlyPlayed.map((activity) => (
                        <div
                          key={activity.id}
                          className="bg-gray-700 p-4 rounded-lg shadow-lg hover:shadow-xl transition-shadow duration-300 flex items-center gap-4 min-w-[250px] mr-4"
                        >
                          <img
                            src={activity.image}
                            alt={activity.song}
                            className="w-12 h-12 rounded-full"
                          />
                          <div>
                            <h3 className="text-xl font-semibold text-white">
                              {activity.song}
                            </h3>
                            {activity.primary_artists}
                          </div>
                        </div>
                      ))
                    ) : (
                      <div>No recent activity available</div>
                    )}
                  </div>
                </CustomScroll>
              </>
            )}
          </div>
        </div>
      </div>
    </div>
  );
};

export default Profile;
