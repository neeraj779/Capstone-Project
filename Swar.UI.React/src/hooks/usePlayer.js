import { useContext } from "react";
import PlayerContext from "../contexts/PlayerContext";

const usePlayer = () => {
  const context = useContext(PlayerContext);
  if (context === null) {
    throw new Error("usePlayer must be used within a PlayerProvider");
  }
  return context;
};

export default usePlayer;
