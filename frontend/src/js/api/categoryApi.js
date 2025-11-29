import { apiFetch } from "./baseApi.js";
import { API_ENDPOINTS } from "../config.js";

/**
 * Get all active categories with pagination
 */
export async function getCategories(pageNumber = 1, pageSize = 100) {
  try {
    const response = await apiFetch(
      `${API_ENDPOINTS.CATEGORIES}?pageNumber=${pageNumber}&pageSize=${pageSize}`
    );
    
    if (!response.ok) {
      throw new Error('Failed to fetch categories');
    }
    
    return await response.json();
  } catch (error) {
    console.error('Error fetching categories:', error);
    throw error;
  }
}

/**
 * Get all categories without pagination
 */
export async function getAllCategories() {
  try {
    const response = await apiFetch(API_ENDPOINTS.CATEGORIES_ALL);
    
    if (!response.ok) {
      throw new Error('Failed to fetch all categories');
    }
    
    return await response.json();
  } catch (error) {
    console.error('Error fetching all categories:', error);
    throw error;
  }
}

/**
 * Get category by ID
 */
export async function getCategoryById(id) {
  try {
    const response = await apiFetch(`${API_ENDPOINTS.CATEGORIES}/${id}`);
    
    if (!response.ok) {
      throw new Error('Failed to fetch category');
    }
    
    return await response.json();
  } catch (error) {
    console.error('Error fetching category:', error);
    throw error;
  }
}