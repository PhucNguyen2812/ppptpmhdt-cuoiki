import { getAccessToken } from "../utils/token.js";
import { API_BASE_URL } from "../config.js";

/**
 * Create instructor request with certificate file
 * @param {File} chungChiFile - Certificate file (image or document)
 * @param {string} thongTinBoSung - Additional information (optional)
 * @returns {Promise<Object>} Success response
 */
export async function createInstructorRequest(chungChiFile, thongTinBoSung = '') {
  const token = getAccessToken();
  if (!token) {
    throw new Error('Bạn cần đăng nhập để gửi yêu cầu');
  }

  const formData = new FormData();
  formData.append('chungChi', chungChiFile);
  if (thongTinBoSung) {
    formData.append('thongTinBoSung', thongTinBoSung);
  }

  const response = await fetch(`${API_BASE_URL}v1/instructor-requests`, {
    method: 'POST',
    headers: {
      'Authorization': `Bearer ${token}`
      // Don't set Content-Type, browser will set it with boundary for FormData
    },
    body: formData
  });

  if (!response.ok) {
    const error = await response.json();
    throw new Error(error.message || 'Gửi yêu cầu thất bại');
  }

  return await response.json();
}

/**
 * Get my instructor request
 * @returns {Promise<Object|null>} Request data or null if no request
 */
export async function getMyInstructorRequest() {
  const token = getAccessToken();
  if (!token) {
    throw new Error('Bạn cần đăng nhập');
  }

  const response = await fetch(`${API_BASE_URL}v1/instructor-requests/my-request`, {
    method: 'GET',
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    }
  });

  if (!response.ok) {
    const error = await response.json();
    throw new Error(error.message || 'Lấy thông tin yêu cầu thất bại');
  }

  const result = await response.json();
  return result.data; // Return null if no request, or the request object
}

/**
 * Get instructor requests (for KIEMDUYETVIEN/ADMIN)
 * @param {Object} params - Query parameters (trangThai, pageNumber, pageSize)
 * @returns {Promise<Object>} Instructor requests data
 */
export async function getInstructorRequests(params = {}) {
  const token = getAccessToken();
  if (!token) {
    throw new Error('Bạn cần đăng nhập');
  }

  const queryString = new URLSearchParams(params).toString();
  const url = `${API_BASE_URL}v1/instructor-requests${queryString ? '?' + queryString : ''}`;

  const response = await fetch(url, {
    method: 'GET',
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    }
  });

  if (!response.ok) {
    const error = await response.json();
    throw new Error(error.message || 'Lấy danh sách yêu cầu đăng ký giảng viên thất bại');
  }

  return await response.json();
}

/**
 * Get instructor request by ID (for KIEMDUYETVIEN/ADMIN)
 * @param {number} id - Request ID
 * @returns {Promise<Object>} Request detail
 */
export async function getInstructorRequestById(id) {
  const token = getAccessToken();
  if (!token) {
    throw new Error('Bạn cần đăng nhập');
  }

  const response = await fetch(`${API_BASE_URL}v1/instructor-requests/${id}`, {
    method: 'GET',
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    }
  });

  if (!response.ok) {
    const error = await response.json();
    throw new Error(error.message || 'Lấy chi tiết yêu cầu thất bại');
  }

  return await response.json();
}

/**
 * Approve instructor request
 * @param {number} id - Request ID
 * @returns {Promise<Object>} Success response
 */
export async function approveInstructorRequest(id) {
  const token = getAccessToken();
  if (!token) {
    throw new Error('Bạn cần đăng nhập');
  }

  const response = await fetch(`${API_BASE_URL}v1/instructor-requests/${id}/approve`, {
    method: 'POST',
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    }
  });

  if (!response.ok) {
    const error = await response.json();
    throw new Error(error.message || 'Duyệt yêu cầu thất bại');
  }

  return await response.json();
}

/**
 * Reject instructor request
 * @param {number} id - Request ID
 * @param {string} lyDoTuChoi - Reason for rejection (required)
 * @returns {Promise<Object>} Success response
 */
export async function rejectInstructorRequest(id, lyDoTuChoi) {
  const token = getAccessToken();
  if (!token) {
    throw new Error('Bạn cần đăng nhập');
  }

  if (!lyDoTuChoi || !lyDoTuChoi.trim()) {
    throw new Error('Lý do từ chối là bắt buộc');
  }

  const response = await fetch(`${API_BASE_URL}v1/instructor-requests/${id}/reject`, {
    method: 'POST',
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      lyDoTuChoi: lyDoTuChoi.trim()
    })
  });

  if (!response.ok) {
    const error = await response.json();
    throw new Error(error.message || 'Từ chối yêu cầu thất bại');
  }

  return await response.json();
}


