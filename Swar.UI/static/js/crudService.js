const CRUDService = (function () {
  const API_BASE_URL = "https://swarapi.azurewebsites.net/api/v1";
  const SONG_SERVICE_BASE_URL =
    "https://songserviceapi.azurewebsites.net/api/v1";
  const accessToken = localStorage.getItem("accessToken");

  const HTTP_METHODS = {
    GET: "GET",
    POST: "POST",
    PUT: "PUT",
    DELETE: "DELETE",
  };

  async function makeRequest(
    endpoint,
    method,
    data = null,
    isSongService = false
  ) {
    const baseUrl = isSongService ? SONG_SERVICE_BASE_URL : API_BASE_URL;
    const url = `${baseUrl}/${endpoint}`;

    const options = {
      method,
      headers: {
        Authorization: `Bearer ${accessToken}`,
        "Content-Type": "application/json",
      },
    };

    if (data) {
      options.body = JSON.stringify(data);
    }

    try {
      const response = await fetch(url, options);
      if (!response.ok) {
        throw new Error(result.message || response.statusText);
      }
      const result = await response.json();
      return result;
    } catch (error) {
      console.error(`Error with ${method} request to ${url}:`, error);
      throw error;
    }
  }

  async function fetchAll(endpoint, isSongService = false) {
    return makeRequest(endpoint, HTTP_METHODS.GET, null, isSongService);
  }

  async function fetchById(endpoint, id, isSongService = false) {
    return makeRequest(
      `${endpoint}/${id}`,
      HTTP_METHODS.GET,
      null,
      isSongService
    );
  }

  async function create(endpoint, data, isSongService = false) {
    return makeRequest(endpoint, HTTP_METHODS.POST, data, isSongService);
  }

  async function update(endpoint, id, data, isSongService = false) {
    return makeRequest(
      `${endpoint}/${id}`,
      HTTP_METHODS.PUT,
      data,
      isSongService
    );
  }

  async function remove(endpoint, id, isSongService = false) {
    return makeRequest(
      `${endpoint}/${id}`,
      HTTP_METHODS.DELETE,
      null,
      isSongService
    );
  }

  return {
    fetchAll,
    fetchById,
    create,
    update,
    remove,
  };
})();
