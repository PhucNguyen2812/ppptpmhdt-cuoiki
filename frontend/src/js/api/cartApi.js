import { apiFetch } from "./baseApi.js";
import { API_ENDPOINTS } from "../config.js";

/**
 * Get cart of current user
 */
export async function getCart() {
  try {
    const response = await apiFetch(API_ENDPOINTS.CART);
    
    if (!response.ok) {
      throw new Error('Failed to fetch cart');
    }
    
    return await response.json();
  } catch (error) {
    console.error('Error fetching cart:', error);
    throw error;
  }
}

/**
 * Get cart count (number of courses in cart)
 */
export async function getCartCount() {
  try {
    const response = await apiFetch(API_ENDPOINTS.CART_COUNT);
    
    if (!response.ok) {
      throw new Error('Failed to fetch cart count');
    }
    
    return await response.json();
  } catch (error) {
    console.error('Error fetching cart count:', error);
    throw error;
  }
}

/**
 * Add course to cart
 */
export async function addToCart(courseId) {
  try {
    const response = await apiFetch(API_ENDPOINTS.CART + '/items', {
      method: 'POST',
      body: JSON.stringify({ idKhoaHoc: courseId })
    });
    
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      throw new Error(errorData.message || 'Failed to add course to cart');
    }
    
    return await response.json();
  } catch (error) {
    console.error('Error adding to cart:', error);
    throw error;
  }
}

/**
 * Remove course from cart
 */
export async function removeFromCart(cartItemId) {
  try {
    const response = await apiFetch(`${API_ENDPOINTS.CART}/items/${cartItemId}`, {
      method: 'DELETE'
    });
    
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      throw new Error(errorData.message || 'Failed to remove course from cart');
    }
    
    return await response.json();
  } catch (error) {
    console.error('Error removing from cart:', error);
    throw error;
  }
}

/**
 * Clear entire cart
 */
export async function clearCart() {
  try {
    const response = await apiFetch(API_ENDPOINTS.CART, {
      method: 'DELETE'
    });
    
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      throw new Error(errorData.message || 'Failed to clear cart');
    }
    
    return await response.json();
  } catch (error) {
    console.error('Error clearing cart:', error);
    throw error;
  }
}
















