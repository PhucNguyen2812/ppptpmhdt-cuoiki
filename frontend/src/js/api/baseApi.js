import { getAccessToken, saveAccessToken, removeAccessToken } from "../utils/token.js";
import { API_BASE_URL } from "../config.js";

export async function apiFetch(url, options = {}) {
  let token = getAccessToken();

  const headers = {
    "Content-Type": "application/json",
    ...(token && { Authorization: `Bearer ${token}` }),
  };

  // Ensure URL is properly formatted
  const fullUrl = `${API_BASE_URL}${url}`;
  console.log('API Fetch URL:', fullUrl);
  
  let response = await fetch(fullUrl, {
    ...options,
    headers,
    credentials: "include", // include cookies
  });

  // If unauthorized, try to refresh token
  if (response.status === 401) {
    const refreshed = await refreshToken();
    if (refreshed) {
      token = getAccessToken();
      response = await fetch(`${API_BASE_URL}${url}`, {
        ...options,
        headers: { ...headers, Authorization: `Bearer ${token}` },
        credentials: "include",
      });
    } else {
      removeAccessToken();
      // Determine correct path based on current location
      const currentPath = window.location.pathname;
      if (currentPath.includes('/pages/') || currentPath.includes('/admin/')) {
        window.location.href = "login.html";
      } else {
        window.location.href = "pages/login.html";
      }
    }
  }

  return response;
}

async function refreshToken() {
  try {
    const response = await fetch(`${API_BASE_URL}v1/auth/refresh-token`, {
      method: "POST",
      credentials: "include",
    });

    if (!response.ok) return false;

    const data = await response.json();
    saveAccessToken(data.accessToken);
    return true;
  } catch {
    return false;
  }
}
