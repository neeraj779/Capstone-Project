const CRUDService = (function () {
  const API_BASE_URL = "https://swarapi.azurewebsites.net/api/v1";
  const accessToken = localStorage.getItem("accessToken");

  async function fetchAll(endpoint) {
    try {
      const response = await fetch(`${API_BASE_URL}/${endpoint}`, {
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
      });
      if (!response.ok) throw new Error(response.statusText);
      return await response.json();
    } catch (error) {
      console.error(`Error fetching data from ${endpoint}:`, error);
      throw error;
    }
  }

  async function fetchById(endpoint, id) {
    try {
      const response = await fetch(`${API_BASE_URL}/${endpoint}/${id}`, {
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
      });
      if (!response.ok) throw new Error(response.statusText);
      return await response.json();
    } catch (error) {
      console.error(`Error fetching data from ${endpoint}/${id}:`, error);
      throw error;
    }
  }

  async function create(endpoint, data) {
    try {
      const response = await fetch(`${API_BASE_URL}/${endpoint}`, {
        method: "POST",
        headers: {
          Authorization: `Bearer ${accessToken}`,
          "Content-Type": "application/json",
        },
        body: JSON.stringify(data),
      });
      const result = await response.json();
      if (response.status !== 200) {
        throw new Error(result.message);
      }
      return result;
    } catch (error) {
      console.error(`Error creating data at ${endpoint}:`, error);
      throw error;
    }
  }

  async function update(endpoint, id, data) {
    try {
      const response = await fetch(`${API_BASE_URL}/${endpoint}/${id}`, {
        method: "PUT",
        headers: {
          Authorization: `Bearer ${accessToken}`,
          "Content-Type": "application/json",
        },
        body: JSON.stringify(data),
      });
      const result = await response.json();
      if (response.status !== 200) {
        throw new Error(result.message);
      }
      return result;
    } catch (error) {
      console.error(`Error updating data at ${endpoint}/${id}:`, error);
      throw error;
    }
  }

  async function remove(endpoint, id) {
    try {
      const response = await fetch(`${API_BASE_URL}/${endpoint}/${id}`, {
        method: "DELETE",
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
      });
      if (!response.ok) throw new Error(response.statusText);
      return await response.json();
    } catch (error) {
      console.error(`Error deleting data at ${endpoint}/${id}:`, error);
      throw error;
    }
  }

  return {
    fetchAll,
    fetchById,
    create,
    update,
    remove,
  };
})();
