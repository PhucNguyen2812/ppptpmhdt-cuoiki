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
 * Get all active categories without pagination (PUBLIC endpoint)
 */
export async function getAllCategories() {
  try {
    // Use public endpoint instead of /all which requires ADMIN role
    const response = await apiFetch(API_ENDPOINTS.CATEGORIES);
    
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
 * Get all categories including inactive ones (ADMIN only)
 */
export async function getAllCategoriesAdmin(search = null) {
  try {
    let url = API_ENDPOINTS.CATEGORIES_ALL;
    if (search) {
      url += `?search=${encodeURIComponent(search)}`;
    }
    
    const response = await apiFetch(url);
    
    if (!response.ok) {
      throw new Error('Failed to fetch all categories');
    }
    
    return await response.json();
  } catch (error) {
    console.error('Error fetching all categories (admin):', error);
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

/**
 * Create a new category (ADMIN only)
 */
export async function createCategory(categoryData) {
  try {
    const response = await apiFetch(API_ENDPOINTS.CATEGORIES, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(categoryData)
    });

    if (!response.ok) {
      let errorMessage = 'Failed to create category';
      try {
        const errorData = await response.json();
        errorMessage = errorData.message || errorData.data || errorMessage;
      } catch (_) {
        // ignore parse errors
      }
      throw new Error(errorMessage);
    }

    return await response.json();
  } catch (error) {
    console.error('Error creating category:', error);
    throw error;
  }
}

/**
 * Update an existing category (ADMIN only)
 */
export async function updateCategory(categoryId, categoryData) {
  try {
    const response = await apiFetch(`${API_ENDPOINTS.CATEGORIES}/${categoryId}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(categoryData)
    });

    if (!response.ok) {
      let errorMessage = 'Failed to update category';
      try {
        const errorData = await response.json();
        errorMessage = errorData.message || errorData.data || errorMessage;
      } catch (_) {
        // ignore parse errors
      }
      throw new Error(errorMessage);
    }

    return await response.json();
  } catch (error) {
    console.error('Error updating category:', error);
    throw error;
  }
}

/**
 * Soft delete (hide) a category (ADMIN only)
 * Note: This will fail if category still has courses
 */
export async function deleteCategory(categoryId) {
  try {
    const response = await apiFetch(`${API_ENDPOINTS.CATEGORIES}/${categoryId}`, {
      method: 'DELETE'
    });

    if (!response.ok) {
      let errorMessage = 'Failed to delete category';
      try {
        const errorData = await response.json();
        errorMessage = errorData.message || errorData.data || errorMessage;
      } catch (_) {
        // ignore parse errors
      }
      throw new Error(errorMessage);
    }

    return await response.json();
  } catch (error) {
    console.error('Error deleting category:', error);
    throw error;
  }
}

/**
 * Restore a soft-deleted category (ADMIN only)
 */
export async function restoreCategory(categoryId) {
  try {
    const response = await apiFetch(`${API_ENDPOINTS.CATEGORIES}/${categoryId}/restore`, {
      method: 'PUT'
    });

    if (!response.ok) {
      let errorMessage = 'Failed to restore category';
      try {
        const errorData = await response.json();
        errorMessage = errorData.message || errorData.data || errorMessage;
      } catch (_) {
        // ignore parse errors
      }
      throw new Error(errorMessage);
    }

    return await response.json();
  } catch (error) {
    console.error('Error restoring category:', error);
    throw error;
  }
}