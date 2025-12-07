import { apiFetch } from "./baseApi.js";
import { API_ENDPOINTS } from "../config.js";

/**
 * Get courses with filters, search, sort, pagination
 */
export async function getCourses(filters = {}) {
  try {
    const params = new URLSearchParams();
    
    // Pagination
    if (filters.pageNumber) params.append('pageNumber', filters.pageNumber);
    if (filters.pageSize) params.append('pageSize', filters.pageSize);
    
    // Search
    if (filters.search) params.append('search', filters.search);
    
    // Filters
    if (filters.categoryId) params.append('categoryId', filters.categoryId);
    if (filters.minRating) params.append('minRating', filters.minRating);
    if (filters.level) params.append('level', filters.level);
    
    // Sort
    if (filters.sortBy) params.append('sortBy', filters.sortBy);
    
    const url = `${API_ENDPOINTS.COURSES}?${params.toString()}`;
    const response = await apiFetch(url);
    
    if (!response.ok) {
      throw new Error('Failed to fetch courses');
    }
    
    return await response.json();
  } catch (error) {
    console.error('Error fetching courses:', error);
    throw error;
  }
}

/**
 * Get featured courses
 */
export async function getFeaturedCourses(take = 8) {
  try {
    const response = await apiFetch(`${API_ENDPOINTS.COURSES_FEATURED}?take=${take}`);
    
    if (!response.ok) {
      throw new Error('Failed to fetch featured courses');
    }
    
    return await response.json();
  } catch (error) {
    console.error('Error fetching featured courses:', error);
    throw error;
  }
}

/**
 * Get best-selling courses
 */
export async function getBestSellingCourses(take = 8) {
  try {
    const response = await apiFetch(`${API_ENDPOINTS.COURSES_BEST_SELLING}?take=${take}`);
    
    if (!response.ok) {
      throw new Error('Failed to fetch best-selling courses');
    }
    
    return await response.json();
  } catch (error) {
    console.error('Error fetching best-selling courses:', error);
    throw error;
  }
}

/**
 * Get newest courses
 */
export async function getNewestCourses(take = 8) {
  try {
    const response = await apiFetch(`${API_ENDPOINTS.COURSES_NEWEST}?take=${take}`);
    
    if (!response.ok) {
      throw new Error('Failed to fetch newest courses');
    }
    
    return await response.json();
  } catch (error) {
    console.error('Error fetching newest courses:', error);
    throw error;
  }
}

/**
 * Get course detail by ID
 */
export async function getCourseById(id) {
  try {
    const response = await apiFetch(`${API_ENDPOINTS.COURSE_DETAIL}/${id}`);
    
    if (!response.ok) {
      throw new Error('Failed to fetch course detail');
    }
    
    return await response.json();
  } catch (error) {
    console.error('Error fetching course detail:', error);
    throw error;
  }
}

/**
 * Get course curriculum
 */
export async function getCourseCurriculum(id) {
  try {
    const response = await apiFetch(`${API_ENDPOINTS.COURSE_CURRICULUM}/${id}/curriculum`);
    
    if (!response.ok) {
      throw new Error('Failed to fetch course curriculum');
    }
    
    return await response.json();
  } catch (error) {
    console.error('Error fetching course curriculum:', error);
    throw error;
  }
}

/**
 * Get courses by instructor
 */
export async function getCoursesByInstructor(instructorId, pageNumber = 1, pageSize = 12) {
  try {
    // Validate input
    if (!instructorId || instructorId <= 0) {
      throw new Error('ID giảng viên không hợp lệ');
    }

    if (pageNumber < 1) {
      throw new Error('Số trang phải lớn hơn 0');
    }

    if (pageSize < 1 || pageSize > 100) {
      throw new Error('Số items mỗi trang phải từ 1 đến 100');
    }

    const response = await apiFetch(
      `${API_ENDPOINTS.COURSES_BY_INSTRUCTOR}/${instructorId}?pageNumber=${pageNumber}&pageSize=${pageSize}`
    );
    
    if (!response.ok) {
      // Try to get error message from response
      let errorMessage = 'Không thể tải danh sách khóa học';
      try {
        const errorData = await response.json();
        errorMessage = errorData.message || errorData.data || errorMessage;
      } catch (e) {
        // If response is not JSON, use status text
        if (response.status === 404) {
          errorMessage = 'Không tìm thấy giảng viên';
        } else if (response.status === 400) {
          errorMessage = 'Dữ liệu không hợp lệ';
        } else if (response.status === 500) {
          errorMessage = 'Lỗi máy chủ. Vui lòng thử lại sau.';
        }
      }
      throw new Error(errorMessage);
    }
    
    return await response.json();
  } catch (error) {
    console.error('Error fetching instructor courses:', error);
    throw error;
  }
}

/**
 * Search courses
 */
export async function searchCourses(searchTerm, filters = {}) {
  return await getCourses({ ...filters, search: searchTerm });
}

/**
 * Create a new course (Instructor only)
 */
export async function createCourse(courseData) {
  try {
    const response = await apiFetch(API_ENDPOINTS.COURSES, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(courseData)
    });
    
    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Failed to create course');
    }
    
    return await response.json();
  } catch (error) {
    console.error('Error creating course:', error);
    throw error;
  }
}

/**
 * Update a course (Instructor only)
 */
export async function updateCourse(courseId, courseData) {
  try {
    const response = await apiFetch(`${API_ENDPOINTS.COURSES}/${courseId}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(courseData)
    });
    
    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Failed to update course');
    }
    
    return await response.json();
  } catch (error) {
    console.error('Error updating course:', error);
    throw error;
  }
}

/**
 * Delete a course (Instructor only)
 */
export async function deleteCourse(courseId) {
  try {
    const response = await apiFetch(`${API_ENDPOINTS.COURSES}/${courseId}`, {
      method: 'DELETE'
    });
    
    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Failed to delete course');
    }
    
    return await response.json();
  } catch (error) {
    console.error('Error deleting course:', error);
    throw error;
  }
}

/**
 * Hide a course (Instructor only)
 */
export async function hideCourse(courseId) {
  try {
    const response = await apiFetch(`${API_ENDPOINTS.COURSES}/${courseId}/hide`, {
      method: 'POST'
    });
    
    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Failed to hide course');
    }
    
    return await response.json();
  } catch (error) {
    console.error('Error hiding course:', error);
    throw error;
  }
}

/**
 * Unhide a course (Instructor only)
 */
export async function unhideCourse(courseId) {
  try {
    const response = await apiFetch(`${API_ENDPOINTS.COURSES}/${courseId}/unhide`, {
      method: 'POST'
    });
    
    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Failed to unhide course');
    }
    
    return await response.json();
  } catch (error) {
    console.error('Error unhiding course:', error);
    throw error;
  }
}

/**
 * Get course for edit (Instructor only)
 */
export async function getCourseForEdit(courseId) {
  try {
    const url = `${API_ENDPOINTS.COURSES}/${courseId}/for-edit`;
    console.log('Fetching course for edit from:', url);
    const response = await apiFetch(url);
    
    if (!response.ok) {
      let errorMessage = `HTTP ${response.status}: ${response.statusText}`;
      try {
        const errorData = await response.json();
        errorMessage = errorData.message || errorData.data || errorMessage;
      } catch (e) {
        // Response không phải JSON hoặc không có body
        if (response.status === 404) {
          errorMessage = 'Không tìm thấy khóa học hoặc bạn không có quyền chỉnh sửa khóa học này';
        } else if (response.status === 401) {
          errorMessage = 'Bạn cần đăng nhập lại';
        } else if (response.status === 403) {
          errorMessage = 'Bạn không có quyền chỉnh sửa khóa học này';
        }
      }
      throw new Error(errorMessage);
    }
    
    const result = await response.json();
    return result;
  } catch (error) {
    console.error('Error fetching course for edit:', error);
    throw error;
  }
}


/**
 * Update course with curriculum (Instructor only)
 */
export async function updateCourseWithCurriculum(courseId, courseData) {
  try {
    const response = await apiFetch(`${API_ENDPOINTS.COURSES}/${courseId}/with-curriculum`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(courseData)
    });
    
    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Failed to update course');
    }
    
    return await response.json();
  } catch (error) {
    console.error('Error updating course with curriculum:', error);
    throw error;
  }
}
