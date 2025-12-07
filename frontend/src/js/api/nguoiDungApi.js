import { apiFetch } from "./baseApi.js";

/**
 * Get all users
 * @param {Object} params - Query parameters
 * @returns {Promise<Object>} Users data
 */
export async function getAllUsers(params = {}) {
  const queryString = new URLSearchParams(params).toString();
  const url = `v1/nguoidungs${queryString ? '?' + queryString : ''}`;
  
  const response = await apiFetch(url);
  
  if (!response.ok) {
    let errorMessage = 'An unexpected error occurred';
    try {
      const error = await response.json();
      errorMessage = error.message || error.error || `Failed to fetch users (${response.status})`;
    } catch (e) {
      // If response is not JSON, use status text
      errorMessage = response.statusText || `Failed to fetch users (${response.status})`;
    }
    throw new Error(errorMessage);
  }
  
  return await response.json();
}

/**
 * Get user by ID
 * @param {string} id - User ID
 * @returns {Promise<Object>} User data
 */
export async function getUserById(id) {
  const response = await apiFetch(`v1/nguoidungs/${id}`);
  
  if (!response.ok) {
    const error = await response.json();
    throw new Error(error.message || 'Failed to fetch user');
  }
  
  return await response.json();
}

/**
 * Create new user
 * @param {Object} userData - User data
 * @returns {Promise<Object>} Created user
 */
export async function createUser(userData) {
  const response = await apiFetch('v1/nguoidungs', {
    method: 'POST',
    body: JSON.stringify(userData)
  });
  
  if (!response.ok) {
    const error = await response.json();
    throw new Error(error.message || 'Failed to create user');
  }
  
  return await response.json();
}

/**
 * Update user
 * @param {string} id - User ID
 * @param {Object} userData - User data
 * @returns {Promise<Object>} Updated user
 */
export async function updateUser(id, userData) {
  const response = await apiFetch(`v1/nguoidungs/${id}`, {
    method: 'PUT',
    body: JSON.stringify(userData)
  });
  
  if (!response.ok) {
    let errorMessage = 'Failed to update user';
    try {
      const error = await response.json();
      errorMessage = error.message || error.error || `Failed to update user (${response.status})`;
    } catch (e) {
      errorMessage = response.statusText || `Failed to update user (${response.status})`;
    }
    throw new Error(errorMessage);
  }
  
  // Handle 204 No Content or empty response
  if (response.status === 204 || response.headers.get('content-length') === '0') {
    return { success: true, message: 'Cập nhật người dùng thành công' };
  }
  
  // Try to parse JSON, but handle empty response gracefully
  const text = await response.text();
  if (!text || text.trim() === '') {
    return { success: true, message: 'Cập nhật người dùng thành công' };
  }
  
  try {
    return JSON.parse(text);
  } catch (e) {
    // If JSON parsing fails, return success response
    return { success: true, message: 'Cập nhật người dùng thành công' };
  }
}

/**
 * Delete user
 * @param {string} id - User ID
 * @returns {Promise<void>}
 */
export async function deleteUser(id) {
  const response = await apiFetch(`v1/nguoidungs/${id}`, {
    method: 'DELETE'
  });
  
  if (!response.ok) {
    const error = await response.json();
    throw new Error(error.message || 'Failed to delete user');
  }
}

/**
 * Register as instructor
 * @deprecated This function is deprecated. Use createInstructorRequest from instructorRequestApi.js instead.
 * The new flow requires certificate upload and approval process.
 * @returns {Promise<Object>} Success response
 */
export async function registerAsInstructor() {
  throw new Error('Hàm này đã bị vô hiệu hóa. Vui lòng sử dụng trang đăng ký làm giảng viên với chứng chỉ.');
  
  // DEPRECATED: Old API that directly grants instructor role without certificate
  // This is no longer supported. Users must go through the approval process.
  // const response = await apiFetch('v1/nguoidungs/register-instructor', {
  //   method: 'POST'
  // });
  // 
  // if (!response.ok) {
  //   const error = await response.json();
  //   throw new Error(error.message || 'Failed to register as instructor');
  // }
  // 
  // return await response.json();
}

/**
 * Restore user (un-delete)
 * @param {string} id - User ID
 * @returns {Promise<Object>} Success response
 */
export async function restoreUser(id) {
  const response = await apiFetch(`v1/nguoidungs/${id}/restore`, {
    method: 'PUT'
  });
  
  if (!response.ok) {
    const error = await response.json();
    throw new Error(error.message || 'Failed to restore user');
  }
  
  return await response.json();
}

/**
 * Get user profile
 * @returns {Promise<Object>} User profile data
 */
export async function getProfile() {
  const response = await apiFetch('v1/nguoidungs/profile');
  
  if (!response.ok) {
    const error = await response.json();
    throw new Error(error.message || 'Failed to fetch profile');
  }
  
  return await response.json();
}