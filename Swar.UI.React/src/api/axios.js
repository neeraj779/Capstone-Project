import axios from "axios";

const SWAR_API_BASE_URL = import.meta.env.VITE_SWAR_BASE_URL;
const SONG_SERVICE_API_BASE_URL = import.meta.env.VITE_SONG_SERVICE_BASE_URL;

const apiClient = (isSongService = false) =>
  axios.create({
    baseURL: isSongService ? SONG_SERVICE_API_BASE_URL : SWAR_API_BASE_URL,
    headers: {
      "Content-Type": "application/json",
    },
  });

export default apiClient;
