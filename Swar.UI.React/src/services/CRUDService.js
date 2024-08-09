const API_BASE_URL = "https://swarapi.azurewebsites.net/api/v1";
const SONG_SERVICE_BASE_URL = "https://songserviceapi.azurewebsites.net/api/v1";

const HTTP_METHODS = {
  GET: "GET",
  POST: "POST",
  PUT: "PUT",
  PATCH: "PATCH",
  DELETE: "DELETE",
};

const getAccessToken = () => localStorage.getItem("accessToken");

const buildUrl = (endpoint, isSongService) =>
  `${isSongService ? SONG_SERVICE_BASE_URL : API_BASE_URL}/${endpoint}`;

const buildHeaders = () => ({
  Authorization: `Bearer ${getAccessToken()}`,
  "Content-Type": "application/json",
});

const isJsonOrText = (contentType) =>
  contentType &&
  (contentType.includes("application/json") ||
    contentType.includes("text/plain"));

const handleResponse = async (response) => {
  const contentType = response.headers.get("Content-Type");
  const result = isJsonOrText(contentType) ? await response.json() : null;

  if (!response.ok) {
    const error = new Error(result?.message || response.statusText);
    error.status = result?.status || response.status;
    throw error;
  }
  return result;
};

const makeRequest = async (
  endpoint,
  method,
  data = null,
  isSongService = false
) => {
  const url = buildUrl(endpoint, isSongService);
  const options = {
    method,
    headers: buildHeaders(),
    ...(data && { body: JSON.stringify(data) }),
  };

  const response = await fetch(url, options);
  return handleResponse(response);
};

export const fetchAll = (endpoint, isSongService = false) =>
  makeRequest(endpoint, HTTP_METHODS.GET, null, isSongService);

export const fetchById = (endpoint, id, isSongService = false) =>
  makeRequest(`${endpoint}/${id}`, HTTP_METHODS.GET, null, isSongService);

export const create = (endpoint, data, isSongService = false) =>
  makeRequest(endpoint, HTTP_METHODS.POST, data, isSongService);

export const update = (endpoint, id, data, isSongService = false) =>
  makeRequest(`${endpoint}/${id}`, HTTP_METHODS.PUT, data, isSongService);

export const patch = (endpoint, data, isSongService = false) =>
  makeRequest(endpoint, HTTP_METHODS.PATCH, data, isSongService);

export const remove = (endpoint, id, isSongService = false) =>
  makeRequest(`${endpoint}/${id}`, HTTP_METHODS.DELETE, null, isSongService);
