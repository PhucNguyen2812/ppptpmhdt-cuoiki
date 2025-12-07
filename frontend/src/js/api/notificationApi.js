import { apiFetch } from "./baseApi.js";
import { API_BASE_URL } from "../config.js";

/**
 * Lấy danh sách thông báo
 * @param {string|null} trangThai - "Chưa đọc" hoặc "Đã đọc" hoặc null
 * @param {number} pageNumber - Số trang (default: 1)
 * @param {number} pageSize - Số items mỗi trang (default: 10)
 * @returns {Promise<Object>} Danh sách thông báo với pagination
 */
export async function getNotifications(trangThai = null, pageNumber = 1, pageSize = 10) {
  const params = new URLSearchParams({
    pageNumber: pageNumber.toString(),
    pageSize: pageSize.toString(),
  });
  
  if (trangThai) {
    params.append("trangThai", trangThai);
  }

  const response = await apiFetch(`/api/v1/notifications?${params.toString()}`);
  
  if (!response.ok) {
    const errorData = await response.json().catch(() => ({}));
    throw new Error(errorData.message || "Lỗi khi lấy danh sách thông báo");
  }

  const data = await response.json();
  return data.data;
}

/**
 * Lấy số lượng thông báo chưa đọc
 * @returns {Promise<number>} Số lượng thông báo chưa đọc
 */
export async function getUnreadCount() {
  const response = await apiFetch("/api/v1/notifications/unread-count");
  
  if (!response.ok) {
    const errorData = await response.json().catch(() => ({}));
    throw new Error(errorData.message || "Lỗi khi lấy số thông báo chưa đọc");
  }

  const data = await response.json();
  return data.data;
}

/**
 * Đánh dấu thông báo đã đọc
 * @param {number} notificationId - ID thông báo
 * @returns {Promise<void>}
 */
export async function markAsRead(notificationId) {
  const response = await apiFetch(`/api/v1/notifications/${notificationId}/read`, {
    method: "PUT",
  });
  
  if (!response.ok) {
    const errorData = await response.json().catch(() => ({}));
    throw new Error(errorData.message || "Lỗi khi đánh dấu đọc");
  }

  const data = await response.json();
  return data;
}

/**
 * Đánh dấu tất cả thông báo đã đọc
 * @returns {Promise<void>}
 */
export async function markAllAsRead() {
  const response = await apiFetch("/api/v1/notifications/read-all", {
    method: "PUT",
  });
  
  if (!response.ok) {
    const errorData = await response.json().catch(() => ({}));
    throw new Error(errorData.message || "Lỗi khi đánh dấu tất cả đã đọc");
  }

  const data = await response.json();
  return data;
}




