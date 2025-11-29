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
    const response = await apiFetch(
      `${API_ENDPOINTS.COURSES_BY_INSTRUCTOR}/${instructorId}?pageNumber=${pageNumber}&pageSize=${pageSize}`
    );
    
    if (!response.ok) {
      throw new Error('Failed to fetch instructor courses');
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