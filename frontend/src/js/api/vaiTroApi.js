import { apiFetch } from "./baseApi.js";
import { API_BASE_URL } from "../config.js";
import { getAccessToken } from "../utils/token.js";

/**
 * Get all roles
 * @returns {Promise<Object>} Roles data
 */
export async function getAllRoles() {
  try {
    console.log('Calling getAllRoles API...');
    const token = getAccessToken();
    
    if (!token) {
      throw new Error('Bạn cần đăng nhập để lấy danh sách vai trò');
    }

    const url = `${API_BASE_URL}v1/vaitros`;
    console.log('Fetching from URL:', url);
    
    const response = await fetch(url, {
      method: 'GET',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });
    
    console.log('Response status:', response.status, response.statusText);
    
    if (!response.ok) {
      // Try to get error message
      let errorMessage = `HTTP ${response.status}: ${response.statusText}`;
      try {
        const errorText = await response.text();
        console.log('Error response text:', errorText);
        if (errorText) {
          try {
            const errorData = JSON.parse(errorText);
            errorMessage = errorData.message || errorMessage;
          } catch (e) {
            errorMessage = errorText || errorMessage;
          }
        }
      } catch (e) {
        console.error('Error parsing error response:', e);
      }
      throw new Error(errorMessage);
    }
    
    const responseText = await response.text();
    console.log('Response text:', responseText);
    
    if (!responseText || responseText.trim() === '') {
      throw new Error('Empty response from server');
    }
    
    const data = JSON.parse(responseText);
    console.log('Parsed response data:', data);
    return data;
  } catch (error) {
    console.error('Error in getAllRoles:', error);
    console.error('Error stack:', error.stack);
    throw error;
  }
}

