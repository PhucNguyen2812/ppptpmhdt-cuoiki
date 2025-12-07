import { apiFetch } from "./baseApi.js";
import { API_BASE_URL } from "../config.js";

/**
 * Tạo đánh giá mới cho khóa học
 * @param {number} courseId - ID khóa học
 * @param {number} diemDanhGia - Điểm đánh giá (1-5)
 * @param {string|null} binhLuan - Bình luận (tùy chọn)
 * @returns {Promise<Object>} Đánh giá đã tạo
 */
export async function createReview(courseId, diemDanhGia, binhLuan = null) {
  const response = await apiFetch("/api/v1/reviews", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({
      idKhoaHoc: courseId,
      diemDanhGia: diemDanhGia,
      binhLuan: binhLuan || null,
    }),
  });

  if (!response.ok) {
    const errorData = await response.json().catch(() => ({}));
    throw new Error(errorData.message || "Lỗi khi tạo đánh giá");
  }

  const data = await response.json();
  return data.data;
}

/**
 * Cập nhật đánh giá
 * @param {number} reviewId - ID đánh giá
 * @param {number} diemDanhGia - Điểm đánh giá (1-5)
 * @param {string|null} binhLuan - Bình luận (tùy chọn)
 * @returns {Promise<Object>} Đánh giá đã cập nhật
 */
export async function updateReview(reviewId, diemDanhGia, binhLuan = null) {
  const response = await apiFetch(`/api/v1/reviews/${reviewId}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({
      diemDanhGia: diemDanhGia,
      binhLuan: binhLuan || null,
    }),
  });

  if (!response.ok) {
    const errorData = await response.json().catch(() => ({}));
    throw new Error(errorData.message || "Lỗi khi cập nhật đánh giá");
  }

  const data = await response.json();
  return data.data;
}

/**
 * Xóa đánh giá
 * @param {number} reviewId - ID đánh giá
 * @returns {Promise<void>}
 */
export async function deleteReview(reviewId) {
  const response = await apiFetch(`/api/v1/reviews/${reviewId}`, {
    method: "DELETE",
  });

  if (!response.ok) {
    const errorData = await response.json().catch(() => ({}));
    throw new Error(errorData.message || "Lỗi khi xóa đánh giá");
  }

  const data = await response.json();
  return data;
}

/**
 * Lấy đánh giá của mình cho một khóa học
 * @param {number} courseId - ID khóa học
 * @returns {Promise<Object|null>} Đánh giá của mình hoặc null nếu chưa đánh giá
 */
export async function getMyReview(courseId) {
  const response = await apiFetch(`/api/v1/reviews/my-review/${courseId}`);

  if (!response.ok) {
    const errorData = await response.json().catch(() => ({}));
    throw new Error(errorData.message || "Lỗi khi lấy đánh giá");
  }

  const data = await response.json();
  return data.data; // Có thể là null nếu chưa đánh giá
}

/**
 * Lấy danh sách đánh giá của khóa học
 * @param {number} courseId - ID khóa học
 * @param {number|null} diemDanhGia - Lọc theo điểm đánh giá (1-5, optional)
 * @param {number} pageNumber - Số trang (default: 1)
 * @param {number} pageSize - Số items mỗi trang (default: 10)
 * @returns {Promise<Object>} Danh sách đánh giá với pagination
 */
export async function getReviewsByCourseId(
  courseId,
  diemDanhGia = null,
  pageNumber = 1,
  pageSize = 10
) {
  const params = new URLSearchParams({
    pageNumber: pageNumber.toString(),
    pageSize: pageSize.toString(),
  });

  if (diemDanhGia) {
    params.append("diemDanhGia", diemDanhGia.toString());
  }

  const response = await apiFetch(
    `/api/v1/reviews/course/${courseId}?${params.toString()}`
  );

  if (!response.ok) {
    const errorData = await response.json().catch(() => ({}));
    throw new Error(errorData.message || "Lỗi khi lấy danh sách đánh giá");
  }

  const data = await response.json();
  return data.data;
}

/**
 * Lấy tổng hợp đánh giá của khóa học
 * @param {number} courseId - ID khóa học
 * @returns {Promise<Object>} Tổng hợp đánh giá (điểm TB, số lượng, phân bố)
 */
export async function getCourseReviewSummary(courseId) {
  const response = await apiFetch(`/api/v1/reviews/course/${courseId}/summary`);

  if (!response.ok) {
    const errorData = await response.json().catch(() => ({}));
    throw new Error(errorData.message || "Lỗi khi lấy tổng hợp đánh giá");
  }

  const data = await response.json();
  return data.data;
}




